using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public Node Parent { get; private set; }
        public virtual bool IsRoot {get { return false; }}

        //Node Events
        public event EventHandler NodeAdded , NodeRemoved; //Called if any node is added below this node in it's tree
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

            node.NodeAdded += NodeAdded;
            node.NodeRemoved += NodeRemoved;

            Add(node);

            if (ChildAdded != null) ChildAdded(node);
            if (NodeAdded != null) NodeAdded(node);
        }

        public virtual bool RemoveChild(Node node)
        {
            if (node == null) throw new NullReferenceException("Can not remove a null node!");

            node.NodeAdded -= NodeAdded;

            if (ChildRemoved != null) ChildRemoved(node);
            if (NodeRemoved != null) NodeRemoved(node);

            node.NodeRemoved -= NodeRemoved;

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

        public void SetParent(Node node)
        {
            if(IsRoot && node != null) throw new Exception("Root cannot have a parent");

            if (Parent != null) //Unhook before we do anything
            {
                Parent.RemoveChild(this);
            }

            if(node != null)
            {
                //If node is null, this is totally fine if this node is the root.
                Parent = node;

                //Add as a child to the parent
                Parent.AddChild(this);
            }
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
            foreach (var child in this.ToArray().Where(c => c.Active))
            {
                child.Update(gt);
                child.UpdateChildren(gt);
            }
        }

        public void DrawChildren(SpriteBatch sb)
        {
            foreach (var child in this.ToArray().Where(c => c.Visible))
            {
                child.Draw(sb);
                child.DrawChildren(sb);
            }
        }

        public virtual void Destroy(IComponent sender = null)
        {
            if (DestroyEvent != null)
                DestroyEvent(sender);

            if(!IsRoot)
                Parent.RemoveChild(this);

            foreach (var child in this.ToArray())
            {
                child.Destroy(this);
            }

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
