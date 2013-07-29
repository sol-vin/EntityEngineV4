using EntityEngineV4.Engine;

namespace EntityEngineV4.Components
{
    public class Health : Component
    {
        public int HitPoints { get; set; }

        public bool Alive
        {
            get { return !(HitPoints <= 0); }
        }

        public event Entity.EventHandler HurtEvent;

        public event Entity.EventHandler DiedEvent;

        public Health(IComponent e, string name)
            : base(e, name)
        {
        }

        public Health(IComponent e, string name, int hp)
            : base(e, name)
        {
            HitPoints = hp;
        }

        public void Hurt(int points)
        {
            if (!Alive) return;

            HitPoints -= points;
            if (HurtEvent != null)
                HurtEvent(Parent as Entity);

            if (!Alive)
            {
                if (DiedEvent != null)
                    DiedEvent(Parent as Entity);
            }
        }
    }
}