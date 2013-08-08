using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public abstract class Service : IComponent
    {
        public IComponent Parent { get; private set; }

        public EntityState StateRef { get { return Parent as EntityState; } }

        public event Component.EventHandler AddComponentEvent;
        public event Component.EventHandler RemoveComponentEvent;

        public event Entity.EventHandler AddEntityEvent;
        public event Entity.EventHandler RemoveEntityEvent;

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
        }

        public delegate void EventHandler(Service s);

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

    }
}