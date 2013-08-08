using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.PowerTools
{
    public class Particle : Entity
    {
        public int TimeToLive
        {
            get { return TimeToLiveTimer.Milliseconds; }
            set { TimeToLiveTimer.Milliseconds = value; }
        }

        public Emitter Emitter;
        protected Timer TimeToLiveTimer;

        public Particle(Emitter e, int ttl)
            : base(e, e.Name + ".Particle")
        {
            Name = Name + Id;

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

        public override void Destroy(IComponent i = null)
        {
            base.Destroy(i);
        }
    }

    public class FadeParticle : Particle
    {
        public int FadeAge;
        public Render Render;

        public FadeParticle(Emitter e, int ttl)
            : base(e, ttl)
        {
        }

        public FadeParticle(Emitter e, int fadeage, int ttl)
            : base(e, ttl)
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
        public bool AutoEmit;
        public int AutoEmitAmount = 1;

        private delegate void ParticleHandler(Entity p);

        public Emitter(IComponent parent, string name)
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

        public virtual void Emit(int amount = 1)
        {
            for (var i = 0; i < amount; i++)
            {
                AddEntity(GenerateNewParticle());
            }
        }
    }
}