using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class Component : IComponent
    {
        public Entity Parent { get; private set; }

        public delegate void EventHandler(Component i);

        public event EventHandler DestroyEvent;

        public string Name { get; private set; }

        public int Id { get; private set; }

        public bool Active { get; set; }

        public bool Visible { get; set; }

        public Component(Entity parent, string name)
        {
            Parent = parent;
            Name = name;

            parent.AddComponent(this);
        }

        public virtual void Update(GameTime gt)
        {
        }

        public virtual void Draw(SpriteBatch sb)
        {
        }

        public virtual void Destroy(IComponent i = null)
        {
            if (DestroyEvent != null)
                DestroyEvent(this);
        }
    }
}