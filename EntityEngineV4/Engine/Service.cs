using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public abstract class Service : Node
    {
        public delegate void EventHandler(Service s);

        protected Service(State state, string name)
            : base(state, name)
        {
            //Subscribe to the pre update ensuring it will initialize the component before hand,if not
            //it will defer intialization until the first Update.
            state.PreUpdateEvent += SubscribePreUpdate;
        }

        private void SubscribePreUpdate()
        {
            //Initialize
            if (!Initialized) Initialize();
            GetRoot<State>().PreUpdateEvent -= SubscribePreUpdate; //Unsubscribe
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public virtual void Destroy(IComponent sender = null)
        {
            base.Destroy();

            //Unsubscribe to the pre update ensuring it will not initialize the component
            if (EntityGame.ActiveState != null)
                EntityGame.ActiveState.PreUpdateEvent -= SubscribePreUpdate;

            EntityGame.Log.Write("Destroyed", this, Alert.Info);
        }
    }
}