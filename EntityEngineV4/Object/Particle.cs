using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Object
{
    public class Particle : Entity
    {
        public int TimeToLive
        {
            get { return TimeToLiveTimer.Milliseconds; }
            set { TimeToLiveTimer.Milliseconds = value; }
        }

        public Emitter Emitter;
        public TileRender TileRender;
        public Body Body;
        public Physics Physics;
        protected Timer TimeToLiveTimer;

        public Particle(EntityState stateref, IComponent parent, Vector2 position, int ttl, Emitter e)
            : base(stateref, parent, e.Name + ".Particle")
        {
            Name = Name + Id;

            Body = new Body(this, "Body", position);

            TileRender = new TileRender(this, "TileRender", e.Texture, e.TileSize, Body);

            Physics = new Physics(this, "Physics", Body);

            Emitter = e;
            

            TimeToLiveTimer = new Timer(this, "Timer");
            TimeToLive = ttl;
            TimeToLiveTimer.Start();
            TimeToLiveTimer.LastEvent += () => Destroy();
        }

        public Particle(EntityState stateref, IComponent parent, int ttl, Emitter e)
            : base(stateref, parent, e.Name + ".Particle")
        {
            Name = Name + Id;

            Body = new Body(this, "Body");

            TileRender = new TileRender(this, "TileRender", e.Texture, e.TileSize, Body);

            Physics = new Physics(this, "Physics", Body);

            Emitter = e;

            TimeToLiveTimer = new Timer(this, "Timer");
            TimeToLive = ttl;
            TimeToLiveTimer.Start();
            TimeToLiveTimer.LastEvent += () => Destroy();
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }



    public class FadeParticle : Particle
    {
        public int FadeAge;

        public FadeParticle(EntityState stateref, IComponent parent, int ttl, Emitter e)
            : base(stateref, parent, ttl, e)
        {
        }

        public FadeParticle(EntityState stateref, IComponent parent, Vector2 position, int fadeage, int ttl, Emitter e)
            : base(stateref, parent,  position, ttl, e)
        {
            FadeAge = fadeage;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if ((int)TimeToLiveTimer.TickTime > FadeAge)
            {
                int totalsteps = TimeToLive - FadeAge;
                int currentstep = (int)TimeToLiveTimer.TickTime - FadeAge;
                if (currentstep > totalsteps) currentstep = totalsteps;
                float step = currentstep/(totalsteps*1f);

                TileRender.Alpha = 1f - 1f*step;
            }
        }
    }

    public class Emitter : Component
    {
        public Texture2D Texture { get; protected set; }

        public Vector2 TileSize { get; protected set; }

        public bool AutoEmit;
        public int AutoEmitAmount = 1;

        protected Body Body;

        public Emitter(Entity parent, string name, Body body) : base(parent, name)
        {
            Body = body;
        }

        public Emitter(Entity e, string name, Texture2D texture, Vector2 tilesize, Body body)
            : base(e, name)
        {
            Texture = texture;
            TileSize = tilesize;
            Body = body;
        }

        public override void Update(GameTime gt)
        {
            if (AutoEmit)
                Emit(AutoEmitAmount);
        }

        protected virtual Particle GenerateNewParticle()
        {
            var p = new Particle(Parent.StateRef, Parent.StateRef, Body.Position / 2, 30, this) { Physics = { Velocity = Vector2.Zero } };
            return p;
        }

        public virtual void Emit(int amount)
        {
            for (var i = 0; i < amount; i++)
                Parent.StateRef.AddEntity(GenerateNewParticle());
        }
    }
}
