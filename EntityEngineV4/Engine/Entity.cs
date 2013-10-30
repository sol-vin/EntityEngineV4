using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Data;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    [Serializable]
    public class Entity : HashSet<IComponent>, IComponent
    {
        public IComponent Parent { get; private set; }

        public delegate void EventHandler(Entity e);

        public event Engine.EventHandler DestroyEvent;

        public event Component.EventHandler AddComponentEvent;
        public event Component.EventHandler RemoveComponentEvent;

        public event EventHandler AddEntityEvent;
        public event EventHandler RemoveEntityEvent;

        public event Service.EventHandler AddServiceEvent;
        public event Service.EventHandler RemoveServiceEvent;
        public event Service.ReturnHandler GetServiceEvent;

        public string Name { get; protected set; }
        public uint Id { get; private set; }
        public bool Active { get; set; }
        public bool Visible { get; set; }
        public bool Debug { get; set; }
        public bool IsInitialized { get; private set; }

        public Entity(IComponent parent, string name)
        {
            Parent = parent;
            Name = name;
            Id = EntityGame.GetID();
            Active = true;
            Visible = true;
            
            //Subscribe to the pre update ensuring it will initialize the component
            if (EntityGame.ActiveState != null)
                EntityGame.ActiveState.PreUpdateEvent += SubscribePreUpdate;

            if(parent != null)
                parent.AddEntity(this);
        }

        public virtual void Initialize()
        {
            IsInitialized = true;
        }

        private void SubscribePreUpdate()
        {
            //Initialize
            if (!IsInitialized) Initialize();
            EntityGame.ActiveState.PreUpdateEvent -= SubscribePreUpdate; //Unsubscribe
        }

        public virtual void Update(GameTime gt)
        {
            if(!IsInitialized) Initialize();
            foreach (var component in this.ToList().Where(c => c.Active))
            {
                component.Update(gt);
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            foreach (var component in this.ToList().Where(c => c.Visible))
            {
                component.Draw(sb);
            }
        }

        public virtual void Destroy(IComponent i = null)
        {
            if (DestroyEvent != null)
                DestroyEvent(this);

            RemoveEntity(this);

            //Null out our events
            AddComponentEvent = null;
            RemoveComponentEvent = null;
            AddEntityEvent = null;
            RemoveEntityEvent = null;
            DestroyEvent = null;

            //Unsubscribe to the pre update ensuring it will not initialize the component
            if (EntityGame.ActiveState != null)
                EntityGame.ActiveState.PreUpdateEvent -= SubscribePreUpdate;

            EntityGame.Log.Write("Destroyed", this, Alert.Trivial);
        }

        public T GetComponent<T>(string name) where T : IComponent
        {
            if (name == "") return GetComponent<T>();
            var result = this.FirstOrDefault(c => c.Name == name);
            if (result == null)
            {
                EntityGame.Log.Write("Component " + name + " does not exist", this, Alert.Warning);
                throw new Exception("Component " + name + " does not exist in " + Name + ".");
            }
            return (T)result;
        }

        public T GetComponent<T>(int id) where T : IComponent
        {
            var result = this.FirstOrDefault(c => c.Id == id);
            if (result == null)
            {
                EntityGame.Log.Write("Component " + id + " does not exist", this, Alert.Warning);
                throw new Exception("Component ID:" + id + " does not exist in " + Name + ".");
            }
            return (T)result;
        }

        public T GetComponent<T>() where T : IComponent
        {
            var result = this.FirstOrDefault(c => c is T);
            if (result == null)
            {
                EntityGame.Log.Write("Component " + typeof(T) + " does not exist", this, Alert.Warning);
                throw new Exception("Component of type " + typeof(T) + " does not exist in " + Name + ".");
            }
            return (T)result;
        }

        public virtual void AddComponent(Component c)
        {
            if (this.Any(component => c.Name == component.Name))
            {
                EntityGame.Log.Write("Component " + c.Name + " already exists", this, Alert.Warning);
                throw new Exception(c.Name + " already exists in " + Name + "\'s list!");
            }

            Add(c);

            c.AddEntityEvent += AddEntity;
            c.RemoveEntityEvent += RemoveEntity;
            c.RemoveComponentEvent += RemoveComponent;
            c.AddComponentEvent += AddComponent;
            c.AddServiceEvent += AddService;
            c.RemoveServiceEvent += RemoveService;
            c.GetServiceEvent += GetService;

            DestroyEvent += c.Destroy;

            if (AddComponentEvent != null)
                AddComponentEvent(c);
        }

        public virtual void RemoveComponent(Component c)
        {
            Remove(c);

            c.AddEntityEvent -= AddEntity;
            c.RemoveEntityEvent -= RemoveEntity;
            c.RemoveComponentEvent -= RemoveComponent;
            c.AddComponentEvent -= AddComponent;

            if (RemoveComponentEvent != null)
                RemoveComponentEvent(c);
        }

        public virtual void AddEntity(Entity c)
        {
            if (AddEntityEvent != null)
            {
                AddEntityEvent(c);
            }
            else
            {
                EntityGame.Log.Write("AddEntity was called but no methods were subscribed", this, Alert.Warning);
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
                EntityGame.Log.Write("RemoveEntity was called but no methods were subscribed", this, Alert.Warning);
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
                return (T)GetServiceEvent(typeof(T));
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

        public void Reset()
        {
            //Run our components destroys to ensure they are properly disposed.
            if (DestroyEvent != null)
                DestroyEvent(this);
            //Clear it out just in case.
            Clear();
        }
    }
}