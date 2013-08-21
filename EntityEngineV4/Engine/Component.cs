using System;
using EntityEngineV4.Data;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class Component : IComponent
    {
        public IComponent Parent { get; private set; }

        public delegate void EventHandler(Component i);

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
        }

        public virtual void Update(GameTime gt)
        {
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
    }
}