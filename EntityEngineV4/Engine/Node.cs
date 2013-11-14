using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading;
using EntityEngineV4.Engine.Debugging;
using EntityEngineV4.GUI;
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
        public virtual bool IsRoot {get { return false; }}
        /// <summary>
        /// Whether or not this node should be in the Update/Draw pool.
        /// </summary>
        public virtual bool IsObject {get { return false; }}
        public bool UpdatingChildren { get; private set; }

        public event EventHandler ChildAdded , ChildRemoved; //Called only if this node had AddChild called

        public Node(Node parent, string name)
        {
            Name = name;
            Active = true;
            Visible = true;
            SetParent(parent);
        }

        public virtual void AddChild(Node node)
        {
            if(node == null) throw new NullReferenceException("Node can not be null when adding as a child!");
            if (node.IsRoot) throw new Exception("Child node cannot be a root node!");
            if (node.IsObject)
            {
                GetState().AddObject(node);
            }

            if (UpdatingChildren)
            {
                //File a request
                GetState().Requests.Push(new ActionRequest(this, node, NodeAction.AddChild));
            }
            else
                Add(node);

            if (ChildAdded != null) ChildAdded(node);
        }

        public virtual bool RemoveChild(Node node)
        {
            if (node == null) throw new NullReferenceException("Can not remove a null node!");
            if (ChildRemoved != null) ChildRemoved(node);

            if (UpdatingChildren)
            {
                //File a request
                GetState().Requests.Push(new ActionRequest(this, node, NodeAction.RemoveChild));
                return false;
            }
            else
                return Remove(node);
        }

        public bool RemoveChild(int id)
        {
            return RemoveChild(GetChild(id));
        }

        public bool RemoveChild(string name)
        {
            return RemoveChild(GetChild(name)); //Counts the number of removed nodes, should only be 1
        }

        public Node GetChild(int id)
        {
            Node node = this.FirstOrDefault(c => c.Id == id);
            if(node == null) throw new Exception("Node's id was not found in children!");
            return node;
        }

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
            Node node = this.FirstOrDefault(c => c.GetType() == typeof (T));
            if (node == null) throw new Exception("Node of type " + typeof (T) + " was not found in children!");
            return (T)node;
        }

        public void SetParent(Node node)
        {
            if(IsRoot && node != null) throw new Exception("Root cannot have a parent");
            if (IsRoot) return;
            if(node == null) throw new NullReferenceException("Parent node cannot be null!");

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

        public T GetRoot<T>()where T : Node
        {
            if (IsRoot) return this as T; //We found the root, return it.
            if (Parent == null) throw new Exception(String.Format("{0} is not root but, has a null parent.", Name));
            return Parent.GetRoot<T>(); //Resursively call up the chain until we find the root
        }

        public State GetState()
        {
            if (IsRoot) return this as State; //We found the root, return it.
            if(Parent == null) throw new Exception(String.Format("{0} is not root but, has a null parent.", Name));
            return Parent.GetState(); //Resursively call up the chain until we find the root
        }

        public T GetState<T>() where T : State
        {
            if (IsRoot) return (T)this; //We found the root, return it.
            return (T)Parent.GetState(); //Resursively call up the chain until we find the root
        }

        public virtual void Initialize()
        {
            Initialized = true;
        }

        public virtual void Reset()
        {
            Initialized = false;
        }

        public virtual void Update(GameTime gt)
        {
            if (!Initialized) Initialize();
        }

        public virtual void Draw(SpriteBatch sb)
        {
        }

        public void UpdateChildren(GameTime gt)
        {
            if (Count == 0) return;
            UpdatingChildren = true;
            foreach (var child in this.Where(c => c.Active && !c.IsObject))
            {
                child.Update(gt);
                child.UpdateChildren(gt);
            }
            UpdatingChildren = false;
        }

        public void DrawChildren(SpriteBatch sb)
        {
            foreach (var child in this.Where(c => c.Visible && !c.IsObject))
            {
                child.Draw(sb);
                child.DrawChildren(sb);
            }
        }

        public virtual void Destroy(IComponent sender = null)
        {
            if (UpdatingChildren)
            {
                GetState().Requests.Push(new ActionRequest(null, this, NodeAction.Destory));
                return;
            }

            foreach (var child in this.ToArray())
            {
                child.Destroy(this);
            }

            if (!IsRoot)
                Parent.RemoveChild(this);

            if (IsObject)
                GetState().RemoveObject(this);
            if (DestroyEvent != null)
                DestroyEvent(this);

            Destroyed = true;
            EntityGame.Log.Write("Destroyed", this, Alert.Trivial);
        }

        //Object methods
        public override int GetHashCode()
        {
            return Id; //Id is the same as hashcode,this will help to minimize hashset collisions, as well as prevent dupes
        }
    }
}
