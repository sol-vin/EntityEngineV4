using System;
using System.Collections.Generic;
using System.Linq;

//using System.Threading;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class Node : HashSet<Node>, IComponent
    {
        public delegate void EventHandler(Node node);

        //IComponent fields
        public string Name { get; protected set; }

        public int Id { get; private set; }

        public bool Active { get; set; }

        public bool Visible { get; set; }

        public bool Debug { get; set; }

        public bool Initialized { get; private set; }

        public bool Destroyed { get; private set; }

        public float Layer { get; set; }

        public float Order { get; set; }

        public event Engine.EventHandler DestroyEvent;

        //Node fields
        /// <summary>
        /// Parent node to this Node, if IsRoot is true this will be null!
        /// </summary>
        public Node Parent { get; private set; }

        /// <summary>
        /// Whether or not this node is the root of it's tree
        /// </summary>
        public virtual bool IsRoot { get { return false; } }

        /// <summary>
        /// Whether or not this node should be in the Update/Draw pool.
        /// </summary>
        public virtual bool IsObject { get { return false; } }

        /// <summary>
        /// Whether or not this node is recycled
        /// </summary>
        public bool Recycled { get; private set; }

        /// <summary>
        /// Finds if this node will have it's update called
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (IsObject) return Active; //If it is an object then it's activeness is determined by itself
                if (!Active) return false;   //If this node isn't active, none of it's children are, unless they are objects
                if (Parent != null)           //ensure the parent isn't null
                {
                    return Parent.IsActive;  //Recurrsivly call up the parental chain
                }
                return true; //if we ever get here then the entire parental chain has been evaluated and all were active.
            }
        }

        /// <summary>
        /// Finds if this node will have it's draw called
        /// </summary>
        public bool IsVisible
        {
            get
            {
                if (IsObject) return Visible; //If it is an object then it's activeness is determined by itself
                if (!Visible)
                    return false; //If this node isn't active, none of it's children are, unless they are objects
                if (Parent != null) //ensure the parent isn't null
                {
                    return Parent.IsVisible; //Recurrsivly call up the parental chain
                }
                return true; //For some reason parent was null and was not root so therefore this isn't (or shouldnt) be drawn
            }
        }

        /// <summary>
        /// If the children of this node are being updated. Used to prevent collection modification during a foreach
        /// </summary>
        public bool UpdatingChildren { get; private set; }

        /// <summary>
        /// Called when AddChild and RemoveChild are called respectively.
        /// </summary>
        public event EventHandler ChildAdded, ChildRemoved; //Called only if this node had AddChild called

        public Node(Node parent, string name)
        {
            Id = EntityGame.GetID();
            Name = name;
            Active = true;
            Visible = true;
            SetParent(parent);

            Recycled = false;
        }

        /// <summary>
        /// Finalizer for node. Only called if the GC wills it.
        /// </summary>
        ~Node()
        {
            this.Destroy();
        }

        /// <summary>
        /// Adds a child node to this node.
        /// </summary>
        /// <param name="node"></param>
        public virtual void AddChild(Node node)
        {
            if (node == null) throw new NullReferenceException("Node can not be null when adding as a child!");
            if (node.IsRoot) throw new Exception("Child node cannot be a root node!");
            if (node.IsObject)
            {
                GetRoot<State>().AddObject(node); //Will opt out if node is already in the set (recycled).
            }

            if (UpdatingChildren)
            {
                //File a request
                GetRoot<State>().Requests.Push(new ActionRequest(this, node, NodeAction.AddChild));
            }
            else
                Add(node);

            if (ChildAdded != null) ChildAdded(node);
        }

        /// <summary>
        /// Removes a child node from this node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual bool RemoveChild(Node node)
        {
            if (node == null) throw new NullReferenceException("Can not remove a null node!");
            if (ChildRemoved != null) ChildRemoved(node);

            if (node.Parent != this) return false;

            if (UpdatingChildren)
            {
                //File a request
                GetRoot<State>().Requests.Push(new ActionRequest(this, node, NodeAction.RemoveChild));
                return false;
            }
            else
            {
                node.Parent = null;
                return Remove(node);
            }
        }

        /// <summary>
        /// Removes a child node by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveChild(int id)
        {
            return RemoveChild(GetChild(id));
        }

        /// <summary>
        /// Removes achild node by it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveChild(string name)
        {
            return RemoveChild(GetChild(name)); //Counts the number of removed nodes, should only be 1
        }

        /// <summary>
        /// Gets a child by it's unique id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Node GetChild(int id)
        {
            Node node = this.FirstOrDefault(c => c.Id == id);
            if (node == null) throw new Exception("Node's id was not found in children!");
            return node;
        }

        /// <summary>
        /// Gets a child by it's name. If multiple nodes with the same name exist, it returns the first one it finds.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Node GetChild(string name)
        {
            Node node = this.FirstOrDefault(c => c.Name == name);
            if (node == null) throw new Exception("Node's name was not found in children!");
            return node;
        }

        public T GetChild<T>(int id) where T : Node
        {
            Node node = this.FirstOrDefault(c => c.Id == id);
            if (node == null) throw new Exception("Node's id was not found in children!");
            return (T)node;
        }

        public T GetChild<T>(string name) where T : Node
        {
            Node node = this.FirstOrDefault(c => c.Name == name);
            if (node == null) throw new Exception("Node's name was not found in children!");
            return (T)node;
        }

        public T GetChild<T>() where T : Node
        {
            Node node = this.FirstOrDefault(c => c.GetType() == typeof(T));
            if (node == null) throw new Exception("Node of type " + typeof(T) + " was not found in children!");
            return (T)node;
        }

        public void SetParent(Node node)
        {
            if (IsRoot && node != null) throw new Exception("Root cannot have a parent");
            if (IsRoot) return;
            if (node == null) throw new NullReferenceException("Parent node cannot be null!");

            if (Parent != null) //Unhook before we do anything
            {
                Parent.RemoveChild(this);
            }

            Parent = node;

            //Add as a child to the parent
            Parent.AddChild(this);
        }

        public Node GetRoot()
        {
            if (IsRoot) return this; //We found the root, return it.
            if (Parent == null) throw new Exception(String.Format("{0} is not root but, has a null parent.", Name));
            return Parent.GetRoot(); //Resursively call up the chain until we find the root
        }

        public T GetRoot<T>() where T : Node
        {
            if (IsRoot) return this as T; //We found the root, return it.
            if (Parent == null) throw new Exception(String.Format("{0} is not root but, has a null parent.", Name));
            return Parent.GetRoot<T>(); //Resursively call up the chain until we find the root
        }

        /// <summary>
        /// Gets first object or root in the parental chain.
        /// </summary>
        /// <returns>Container</returns>
        public Node GetContainer()
        {
            return IsRoot || IsObject ? this : Parent.GetContainer(); 
        }

        public virtual void Initialize()
        {
            Initialized = true;
        }

        public virtual void Reset()
        {
            Initialized = false;

            Clear();
        }
        
        public virtual void Update(GameTime gt)
        {
            if (!Initialized && !Destroyed) Initialize();
        }

        public virtual void Draw(SpriteBatch sb)
        {
        }

        public void UpdateChildren(GameTime gt)
        {
            if (Count == 0) return;
            UpdatingChildren = true;
            foreach (var child in this.Where(c => c.Active && !c.IsObject && !c.Destroyed))
            {
                child.Update(gt);
                child.UpdateChildren(gt);
            }
            UpdatingChildren = false;
        }

        public void DrawChildren(SpriteBatch sb)
        {
            if (Count == 0) return;
            foreach (var child in this.Where(c => c.Visible && !c.IsObject && !c.Destroyed))
            {
                child.Draw(sb);
                child.DrawChildren(sb);
            }
        }

        public virtual void Destroy(IComponent sender = null)
        {
            if (Destroyed) return; //Dont let destroyed nodes be destroyed again.
            //Prevent recycled nodes from being destroyed.
            if (!Recycled)
            {
                if (UpdatingChildren)
                {
                    GetRoot<State>().Requests.Push(new ActionRequest(null, this, NodeAction.Destroy));
                    return;
                }

                DestroyChildren();

                if (IsObject)
                    GetRoot<State>().RemoveObject(this);
                if (DestroyEvent != null)
                    DestroyEvent(this);

                //Null out events
                ChildAdded = null;
                ChildRemoved = null;

                Destroyed = true;

                if (!IsRoot) Reset();

                EntityGame.Log.Write("Destroyed", this, Alert.Trivial);
            }

            //Always remove from parent to prevent memory leaks
            if (!IsRoot && !IsObject)
            {
                if (Parent.UpdatingChildren)
                {
                    GetRoot<State>().Requests.Push(new ActionRequest(Parent, this, NodeAction.RemoveChild));
                }
                else
                {
                    Parent.RemoveChild(this);
                }
            }

            //Stop our finalizer from running, impacting performance.
            GC.SuppressFinalize(this);
        }

        public void DestroyChildren()
        {
            foreach (var child in this.ToArray())
            {
                child.Destroy(this);
            }
        }

        /// <summary>
        /// Sets the
        /// </summary>
        public virtual void Recycle()
        {
            if (IsRoot) throw new Exception("Cannot call Recycle on root node!");

            foreach (var child in this.ToArray().Where(c => !c.IsObject))
            {
                child.Recycle();
            }

            Recycled = true;
        }

        public void Reuse()
        {
            Reuse(Parent, Name);
        }

        public void Reuse(Node parent)
        {
            Reuse(parent, Name);
        }

        /// <summary>
        /// Called when a node is recycled to put it back into use
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public virtual void Reuse(Node parent, string name)
        {
            //if (!Recyclable) throw new Exception("Cannot call Reuse on a non-recyclable node!");

            Name = name;
            if (parent != Parent)
                SetParent(parent); //Doesn't re-add the object because Recycled == true at the time of calling.
            //Reuse/reset all child nodes
            foreach (var child in this.ToArray().Where(c => !c.IsObject))
            {
                child.Reuse(this);
            }

            Recycled = false; //Now we can safely set Recycled because the parent operations have already completed.
        }

        //object Methods
        public override int GetHashCode()
        {
            return Id; //Id is the same as hashcode,this will help to minimize hashset collisions, as well as prevent dupes
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj.GetHashCode() == this.GetHashCode();
        }

        public static bool operator ==(Node a, Node b)
        {
            if ((object)a == null && (object)b == null) return true;
            if ((object)a == null || (object)b == null) return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(Node a, Node b)
        {
            return !(a == b);
        }
    }
}