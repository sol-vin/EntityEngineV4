using EntityEngineV4.Data;
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

        public event EventHandler DestroyEvent;

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
            if(e != null)
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
            Entity e = Parent as Entity;
            if (e != null)
            {
                e.RemoveComponent(this);
            }
            
            if (DestroyEvent != null)
                DestroyEvent(this);

            EntityGame.Log.Write("Destroyed", this, Alert.Trivial);
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