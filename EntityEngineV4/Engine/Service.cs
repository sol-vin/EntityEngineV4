using System;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public abstract class Service : IComponent
    {
        public IComponent Parent { get; private set; }

        public EntityState StateRef { get { return Parent as EntityState; } }
        
        public delegate void EventHandler(Service s);

        public delegate Service ReturnHandler(Type t);

        public event Component.EventHandler AddComponentEvent;
        public event Component.EventHandler RemoveComponentEvent;

        public event Entity.EventHandler AddEntityEvent;
        public event Entity.EventHandler RemoveEntityEvent;

        public event EventHandler AddServiceEvent;
        public event EventHandler RemoveServiceEvent;
        public event ReturnHandler GetServiceEvent;

        public event Engine.EventHandler DestroyEvent;

        public string Name { get; private set; }
        public uint Id { get; private set; }
        public bool Active { get; private set; }
        public bool Visible { get; private set; }
        public bool Debug { get; set; }

        protected Service(EntityState stateRef, string name)
        {
            Parent = stateRef;
            Name = name;
            Id = EntityGame.GetID();

            stateRef.AddService(this);

            Active = true;
            Visible = true;
        }


        public abstract void Update(GameTime gt);

        public abstract void Draw(SpriteBatch sb);

        public virtual void Destroy(IComponent i = null)
        {
            if (DestroyEvent != null)
                DestroyEvent(this);

            StateRef.RemoveService(this);

            //Null out our events
            AddComponentEvent = null;
            RemoveComponentEvent = null;
            AddEntityEvent = null;
            RemoveEntityEvent = null;
            DestroyEvent = null;

            EntityGame.Log.Write("Destroyed", this, Alert.Info);
        }


        public void AddComponent(Component c)
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

        public void RemoveComponent(Component c)
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

        public void AddEntity(Entity c)
        {
            if (AddEntityEvent != null)
            {
                AddEntityEvent(c);
                DestroyEvent += c.Destroy;
            }
            else
            {
                EntityGame.Log.Write("AddEntity called with no methods subscribed", this, Alert.Warning);
            }
        }

        public void RemoveEntity(Entity c)
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
    }
}