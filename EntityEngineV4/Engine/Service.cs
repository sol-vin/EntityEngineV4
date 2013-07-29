using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    //TODO: Hook up service events for adding objects
    public abstract class Service : IComponent
    {
        public IComponent Parent { get; private set; }
        public EntityState StateRef { get { return Parent as EntityState; } }
        public event Component.EventHandler AddComponentEvent;
        public event Component.EventHandler RemoveComponentEvent;
        public event Entity.EventHandler AddEntityEvent;
        public event Entity.EventHandler RemoveEntityEvent;

        public string Name { get; private set; }
        public uint Id { get; private set; }
        public bool Active { get; private set; }
        public bool Visible { get; private set; }
        public bool Debug { get; set; }

        public virtual void Destroy(IComponent i = null)
        {
            if (DestroyEvent != null)
                DestroyEvent(this);

            EntityGame.Log.Write("Destroyed", this, Alert.Info);
        }

        protected Service(EntityState stateRef, string name)
        {
            Parent = stateRef;
            Name = name;
            Id = EntityGame.GetID();
        }

        public delegate void EventHandler(Service s);

        public event EventHandler DestroyEvent;

        public abstract void Update(GameTime gt);

        public abstract void Draw(SpriteBatch sb);

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