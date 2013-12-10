using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Components;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class State : Node
    {
        public override bool IsRoot
        {
            get { return true; }
        }

        /// <summary>
        /// Object pool for nodes with Node.IsObject true, this will provide faster Update and Draw times on objects,
        /// bypassing the scene tree's slow iteration time.
        /// </summary>
        private HashSet<Node> _objects = new HashSet<Node>();

        public Stack<ActionRequest> Requests = new Stack<ActionRequest>();

        public event Timer.TimerEvent PreUpdateEvent;

        public event Timer.TimerEvent PostUpdateEvent;

        public event Service.EventHandler ServiceAdded;

        public event Service.EventHandler ServiceRemoved;

        public delegate void EventHandler(State state);

        public event EventHandler ShownEvent;

        public HashSet<Service> Services = new HashSet<Service>();

        public enum InitializeAction
        {
            OncePerShow, OnceInLife
        }

        public InitializeAction InitializeActionOnShow = InitializeAction.OnceInLife;

        public int ActiveObjects
        {
            get
            {
                return _objects.Count(o => o.Active && !o.Recycled);
            }
        }

        /// <summary>
        /// Creates a new State object.
        /// </summary>
        /// <param name="name">Name of State</param>
        public State(string name)
            : base(null, name)
        {
            Active = true;
            Visible = true;
        }

        /// <summary>
        /// Adds a new child to this state.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public override void AddChild(Node node)
        {
            //Check if it is a service first
            Service s = node as Service;
            if (s != null) //If it is a service, add it to Services and not as a child
            {
                if (UpdatingServices)
                {
                    Requests.Push(new ActionRequest(null, node, NodeAction.AddService));
                }
                else
                {
                    if (Services.Count(service => service.GetType() == s.GetType()) != 0)
                        throw new Exception("Service " + s.GetType() + " already exists in this state!");
                    Services.Add(s);
                }
                if (ServiceAdded != null) ServiceAdded(s);
            }
            else
            {
                base.AddChild(node); //Add normally
            }
        }

        /// <summary>
        /// Removes a child from this state.
        /// </summary>
        /// <param name="node">The node to remove</param>
        /// <returns>If the node was removed. If it wasn't, it never existed in the first place.</returns>
        public override bool RemoveChild(Node node)
        {
            Service s = node as Service;
            if (s != null)
            {
                if (UpdatingServices)
                {
                    Requests.Push(new ActionRequest(null, node, NodeAction.RemoveService));
                    return false;
                }
                bool removed = Services.Remove(s);
                if (ServiceRemoved != null) ServiceRemoved(s);
                return removed;
            }
            return base.RemoveChild(node);
        }

        /// <summary>
        /// Adds an object to this state's object pool.
        /// </summary>
        /// <param name="n">The object node to add.</param>
        public void AddObject(Node n)
        {
            if (!n.IsObject) throw new Exception("Node tried to AddObject despite having Node.IsObject being false.");

            //Decide whether object is already in list if recycleable
            //If it is there we need to return to save performance
            if (_objects.Contains(n)) return;

            if (UpdatingObjects)
            {
                Requests.Push(new ActionRequest(null, n, NodeAction.AddObject));
            }
            else
            {
                _objects.Add(n);
            }
        }

        /// <summary>
        /// Remove an object from this state's object pool.
        /// </summary>
        /// <param name="n">The object to remove.</param>
        /// <returns>If the object was removed. If it wasn't, it never existed in the first place.</returns>
        public bool RemoveObject(Node n)
        {
            if (!n.IsObject) throw new Exception("Node tried to RemoveObject despite having Node.IsObject == false.");
            if (UpdatingObjects)
            {
                Requests.Push(new ActionRequest(null, n, NodeAction.RemoveObject));
                return false;
            }
            else
            {
                return _objects.Remove(n);
            }
        }

        public Node GetObject(int id)
        {
            return _objects.FirstOrDefault(o => o.Id == id);
        }

        public Node GetObject(string name)
        {
            return _objects.FirstOrDefault(o => o.Name == name);
        }

        public T GetObject<T>() where T : Node
        {
            return (T)_objects.FirstOrDefault(o => o.GetType() == typeof(T));
        }

        public T GetObject<T>(int id) where T : Node
        {
            return (T)_objects.FirstOrDefault(o => o.Id == id && o.GetType() == typeof(T));
        }

        public T GetObject<T>(string name) where T : Node
        {
            return (T)_objects.FirstOrDefault(o => o.Name == name && o.GetType() == typeof(T));
        }

        /// <summary>
        /// Gets the next recycled object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetNextRecycled<T>(Node parent, string name) where T : Node
        {
            var n = _objects.FirstOrDefault(o => o.Recycled && o.GetType() == typeof(T));
            if (n == null) return null;
            n.Reuse(parent, name);
            return (T)n;
        }

        /// <summary>
        /// Cleans recycled objects out of the object pool.
        /// </summary>
        public void CleanRecycled()
        {
            foreach (var o in _objects.ToArray().Where(o => o.Recycled))
            {
                o.Destroy();
            }
        }

        public void CleanRecycled<T>()
        {
            foreach (var o in _objects.ToArray().Where(o => o.Recycled && o.GetType() == typeof(T)))
            {
                o.Destroy();
            }
        }

        public T GetService<T>() where T : Service
        {
            var result = Services.FirstOrDefault(service => service.GetType() == typeof(T));
            if (result == null)
            {
                EntityGame.Log.Write("Service " + typeof(T) + " does not exist!", this, Alert.Warning);
                return null;
            }
            return (T)result;
        }

        public Service GetService(Type t)
        {
            var result = Services.FirstOrDefault(service => service.GetType() == t);
            if (result == null)
            {
                EntityGame.Log.Write("Service " + t + " does not exist!", this, Alert.Warning);
                return null;
            }
            return result;
        }

        public bool CheckService<T>() where T : Service
        {
            var result = Services.FirstOrDefault(service => service.GetType() == typeof(T));
            return result != null;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public virtual void Show()
        {
            EntityGame.SwitchState(this);

            if (ShownEvent != null)
                ShownEvent(this);

            switch (InitializeActionOnShow)
            {
                case InitializeAction.OnceInLife:
                    if (!Initialized) Initialize();
                    break;

                case InitializeAction.OncePerShow:
                    Initialize();
                    break;
            }

            EntityGame.Log.Write("Shown", this, Alert.Info);
        }

        public override void Reset()
        {
            base.Reset();
            //foreach (var child in this.ToArray())
            //{
            //    child.Destroy(this);
            //}
            Services.Clear();
            _objects.Clear();
            Requests.Clear();
        }

        public bool UpdatingServices { get; private set; }

        public bool UpdatingObjects { get; private set; }

        public virtual void PreUpdate()
        {
            if (PreUpdateEvent != null) PreUpdateEvent();
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            UpdatingServices = true;
            foreach (var service in Services.Where(s => s.Active))
            {
                service.Update(gt);
                service.UpdateChildren(gt);
            }
            UpdatingServices = false;

            UpdateChildren(gt);

            UpdatingObjects = true;

            //Only update active and non-recycled objects
            foreach (var obj in _objects.Where(o => o.Active && !o.Recycled && !o.Destroyed))
            {
                obj.Update(gt);
                obj.UpdateChildren(gt);
            }
            UpdatingObjects = false;
        }

        /// <summary>
        /// Called after the Update has been completed.
        /// This method processes requests ffor additions/deletions
        /// when the collections are not being accessed by a foreach
        /// </summary>
        public virtual void PostUpdate()
        {
            RequestsProcessed = 0;
            while (Requests.Count != 0)
            {
                ActionRequest request = Requests.Pop();
                switch (request.Action)
                {
                    case NodeAction.AddChild:
                        request.Parent.AddChild(request.Child);
                        break;

                    case NodeAction.RemoveChild:
                        request.Parent.RemoveChild(request.Child);
                        break;

                    case NodeAction.AddObject:
                        AddObject(request.Child);
                        break;

                    case NodeAction.RemoveObject:
                        RemoveObject(request.Child);
                        break;

                    case NodeAction.AddService:
                        AddChild(request.Child);
                        break;

                    case NodeAction.RemoveService:
                        AddChild(request.Child);
                        break;

                    case NodeAction.Destroy:
                        request.Child.Destroy(request.Parent);
                        break;
                }

                RequestsProcessed++;
            }
            if (PostUpdateEvent != null) PostUpdateEvent();
        }

        public int RequestsProcessed { get; private set; }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            foreach (var service in Services.Where(s => s.Visible))
            {
                service.Draw(sb);
                service.DrawChildren(sb);
            }

            DrawChildren(sb);

            //Only draw visible and non-recycled objects
            foreach (var o in _objects.Where(o => o.Visible && !o.Recycled && !o.Destroyed))
            {
                o.Draw(sb);
                o.DrawChildren(sb);
            }
        }

        public override void Destroy(IComponent sender = null)
        {
            base.Destroy(sender);

            PostUpdateEvent += Reset;
            GC.Collect();
        }
    }

    public struct ActionRequest
    {
        public readonly Node Parent;
        public readonly Node Child;
        public readonly NodeAction Action;

        public ActionRequest(Node parent, Node child, NodeAction action)
        {
            Parent = parent;
            Child = child;
            Action = action;
        }
    }

    public enum NodeAction
    {
        AddChild, RemoveChild, AddObject, RemoveObject, AddService, RemoveService, Destroy
    }
}