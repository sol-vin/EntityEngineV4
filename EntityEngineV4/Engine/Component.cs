using System;
using System.Collections.Generic;
using EntityEngineV4.Data;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class Component : IComponent
    {
        /// <summary>
        /// Controls the order of updating this component
        /// </summary>
        public float Order;
        /// <summary>
        /// Controls the order and depth of drawing this component
        /// </summary>
        public float Layer;

        /// <summary>
        /// If the Initialize() method has been called since it's creation
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Parent of this component
        /// </summary>
        public IComponent Parent { get; private set; }

        public delegate void EventHandler(Component i);

        /// <summary>
        /// Used to subscribe this to it's parent's AddComponent()
        /// </summary>
        public event EventHandler AddComponentEvent;

        public event EventHandler RemoveComponentEvent;

        public event Entity.EventHandler AddEntityEvent;

        public event Entity.EventHandler RemoveEntityEvent; 

        public event Service.EventHandler AddServiceEvent;
        public event Service.EventHandler RemoveServiceEvent;
        public event Service.ReturnHandler GetServiceEvent;

        public event Engine.EventHandler DestroyEvent;

        public string Name { get; private set; }

        public uint Id { get; private set; }

        public bool Active { get; set; }

        public bool Visible { get; set; }

        public bool Debug { get; set; }

        public Component(Entity parent, string name)
        {
            Parent = parent;
            Name = name;
            Active = true;
            Visible = true;
            parent.AddComponent(this);
            Id = EntityGame.GetID();

            CreateDependencyList();
        }

        public Component(IComponent parent, string name)
        {
            Parent = parent;
            Name = name;
            Active = true;
            Visible = true;
            Entity e = Parent as Entity;
            if (e != null)
            {
                e.AddComponent(this);
            }
            Id = EntityGame.GetID();

            CreateDependencyList();
        }

        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        public virtual void Update(GameTime gt)
        {
            if(!IsInitialized) Initialize();
        }

        public virtual void Draw(SpriteBatch sb = null)
        {
        }

        public virtual void Destroy(IComponent i = null)
        {
            if (DestroyEvent != null)
                DestroyEvent(this);

            RemoveComponent(this);

            //Null out our events
            AddComponentEvent = null;
            RemoveComponentEvent = null;
            AddEntityEvent = null;
            RemoveEntityEvent = null;
            DestroyEvent = null;

            EntityGame.Log.Write("Destroyed", this, Alert.Trivial);
        }

        public virtual void AddComponent(Component c)
        {
            if (AddComponentEvent != null)
            {
                AddComponentEvent(c);
            }
            else
            {
                EntityGame.Log.Write("AddComponent called with no methods subscribed", this, Alert.Warning);
            }
        }

        public virtual void RemoveComponent(Component c)
        {
            if (RemoveComponentEvent != null)
            {
                RemoveComponentEvent(c);
            }
            else
            {
                EntityGame.Log.Write("RemoveComponent called with no methods subscribed", this, Alert.Warning);
            }
        }

        public virtual void AddEntity(Entity c)
        {
            if (AddEntityEvent != null)
            {
                AddEntityEvent(c);
            }
            else
            {
                EntityGame.Log.Write("AddEntity called with no methods subscribed", this, Alert.Warning);
            }
        }

        public virtual void RemoveEntity(Entity c)
        {
            if (RemoveEntityEvent != null)
            {
                RemoveEntityEvent(c);
            }
            else
            {
                EntityGame.Log.Write("RemoveEntity called with no methods subscribed", this, Alert.Warning);
            }
        }

        public void AddService(Service s)
        {
            if (AddServiceEvent != null)
            {
                AddServiceEvent(s);
            }
            else
            {
                EntityGame.Log.Write("AddService called with no methods subscribed", this, Alert.Warning);
            }
        }

        public void RemoveService(Service s)
        {
            if (RemoveServiceEvent != null)
            {
                RemoveServiceEvent(s);
            }
            else
            {
                EntityGame.Log.Write("RemoveService called with no methods subscribed", this, Alert.Warning);
            }
        }

        public T GetService<T>() where T : Service
        {
            if (GetServiceEvent != null)
            {
                return (T) GetServiceEvent(typeof (T));
            }
            EntityGame.Log.Write("GetService was called with no methods subscribed!", this, Alert.Warning);
            return null;
        }

        public Service GetService(Type t)
        {
            if (GetServiceEvent != null)
            {
                return GetServiceEvent(t);
            }
            return null;
        }

        //Dependencies

        /// <summary>
        /// List of links to this component
        /// </summary>
        private Dictionary<int,Component> _links = new Dictionary<int, Component>();
 
        /// <summary>
        /// The last component which was grabbed using GetLink. Useful for faster returns on the same object
        /// </summary>
        private Component _cachedLink;

        /// <summary>
        /// The last index accessed using GetLink. Useful for faster returns on the same object
        /// </summary>
        private int _cachedIndex = -1;

        /// <summary>
        /// Links a component to this component
        /// </summary>
        /// <param name="index"></param>
        /// <param name="component"></param>
        public void Link(int index, Component component)
        {
            if(index < 0) throw new IndexOutOfRangeException();
            //First check if the type of component is the expected type
            if (!(component.GetType() == GetExpectedDependencyType(index) ||
                component.GetType().IsSubclassOf(GetExpectedDependencyType(index))))
            {
                throw new Exception("Component type " + component.GetType() + " is not of expected type " + GetExpectedDependencyType(index));
            }

            //It passed so now we add the link
            _links.Add(index, component);
        }

        /// <summary>
        /// Unlinks a component
        /// </summary>
        /// <param name="index"></param>
        public void Unlink(int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            _links.Remove(index);
        }

        /// <summary>
        /// Gets the link of a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetLink<T>(int index) where T : Component
        {
            if(index < 0) throw new IndexOutOfRangeException();
            //Check our link cache
            if (index == _cachedIndex) return _cachedLink as T;

            var component = _links[index] as T;
            if(component == null) throw new Exception("Component does not exist or is not of this type");

            //Change out cache
            _cachedLink = component;
            _cachedIndex = index;
            return component;
        }

        public Component GetLink(int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();
            //Check our link cache
            if (index == _cachedIndex) return _cachedLink;

            var component = _links[index];
            if (component == null) throw new Exception("Component does not exist or is not of this type");

            //Change out cache
            _cachedLink = component;
            _cachedIndex = index;
            return component;
        }

        /// <summary>
        /// Holds the expected types for each parameter
        /// </summary>
        private Dictionary<int, Type> _linkTypes = new Dictionary<int, Type>();

        /// <summary>
        /// Adds a new possible link to the component
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        protected void AddLinkType(int index, Type type)
        {
            _linkTypes.Add(index, type);
        }

        /// <summary>
        /// Removes a possible link for a component
        /// </summary>
        /// <param name="index"></param>
        protected void RemoveLinkType(int index)
        {
            _linkTypes.Remove(index);
        }

        public Type GetExpectedDependencyType(int index)
        {
            if(_linkTypes[index] == null) throw new IndexOutOfRangeException();
            return _linkTypes[index];
        }

        public virtual void CreateDependencyList()
        {
            
        }
    }
}