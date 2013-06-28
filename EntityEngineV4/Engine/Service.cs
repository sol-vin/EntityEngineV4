using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public abstract class Service : IComponent
    {
        public EntityState StateRef;

        public string Name { get; private set; }
        public uint Id { get; private set; }
        public bool Active { get; private set; }
        public bool Visible { get; private set; }
        public bool Debug { get; set; }

        public virtual void Destroy(IComponent i = null)
        {
            StateRef.Services.Remove(this);
            if (DestroyEvent != null)
                DestroyEvent(this);

            EntityGame.Log.Write("Destroyed", this, Alert.Info);
        }

        protected Service(EntityState stateRef, string name)
        {
            StateRef = stateRef;
            Name = name;
            Id = stateRef.GetId();
        }

        public delegate void EventHandler(Service s);

        public event EventHandler DestroyEvent;

        public virtual void Update(GameTime gt)
        {
            
        }

        public virtual void Draw(SpriteBatch sb)
        {
            
        }
    }
}