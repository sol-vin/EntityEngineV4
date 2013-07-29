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
        public Body Body;
        protected Timer TimeToLiveTimer;

        public Particle(EntityState stateref, Vector2 position, int ttl, Emitter e)
            : base(e, e.Name + ".Particle")
        {
            Name = Name + Id;

            Body = new Body(this, "Body", position);

            Emitter = e;

            TimeToLiveTimer = new Timer(this, "Timer");
            TimeToLive = ttl;
            TimeToLiveTimer.Start();
            TimeToLiveTimer.LastEvent += () => Destroy();
        }

        public Particle(Emitter e, int ttl)
            : base(e, e.Name + ".Particle")
        {
            Name = Name + Id;

            Body = new Body(this, "Body");

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
        public Render Render;

        public FadeParticle( Emitter e, int ttl)
            : base(e, ttl)
        {
        }

        public FadeParticle(EntityState stateref, Vector2 position, int fadeage, int ttl, Emitter e)
            : base(stateref, position, ttl, e)
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
                float step = currentstep / (totalsteps * 1f);

                Render.Alpha = 1f - 1f * step;
            }
        }
    }

    public class Emitter : Component
    {
        //public Texture2D Texture { get; protected set; }

        public Vector2 TileSize { get; protected set; }

        public bool AutoEmit;
        public int AutoEmitAmount = 1;

        private delegate void ParticleHandler(Entity p);
        public Emitter(Entity parent, string name)
            : base(parent, name)
        {
        }

        public override void Update(GameTime gt)
        {
            if (AutoEmit)
                Emit(AutoEmitAmount);
        }

        protected virtual Particle GenerateNewParticle()
        {
            var p = new Particle(this, 30);
            return p;
        }

        public virtual void Emit(int amount)
        {
            
            for (var i = 0; i < amount; i++)
            {
                Particle p = GenerateNewParticle();
                AddEntity(p);
            }
        }
    }
}