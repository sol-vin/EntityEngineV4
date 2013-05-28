using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public abstract class Service
    {
        public bool Active;
        public bool Visible;
        public EntityState StateRef;

        public Service(EntityState stateref)
        {
            StateRef = stateref;
        }

        public delegate void EventHandler(Service s);

        public event EventHandler DestroyEvent;

        public abstract void Update(GameTime gt);

        public abstract void Draw(SpriteBatch sb);

        public virtual void Destroy()
        {
            if (DestroyEvent != null)
                DestroyEvent(this);
        }
    }
}