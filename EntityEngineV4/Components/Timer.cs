using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Components
{
    public class Timer : Component
    {
        public bool Alive { get; private set; }

        /// <summary>
        /// The event delegate that will be used for event functions
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void TimerEvent();

        /// <summary>
        /// The snapshot of the total game seconds to figure out _curtime
        /// </summary>
        private double _lastseconds = 0;

        /// <summary>
        /// Occurs on every tick
        /// </summary>
        public event TimerEvent TickEvent;

        /// <summary>
        /// Occurs once the timer has reached it's limit.
        /// </summary>
        public event TimerEvent LastEvent;

        /// <summary>
        /// The current time since this timer had it's Tick() method called. Read-Only.
        /// </summary>
        public double TickTime { get; protected set; }

        /// <summary>
        /// Milliseconds before the timer shoudl reset.
        /// </summary>
        public int Milliseconds;

        private bool _tr;

        public bool TimeReached
        {
            get { return _tr; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClxTimer"/> class.
        /// </summary>
        public Timer(Entity e, string name)
            : base(e, name)
        {
            Alive = false;
        }

        public void Reset()
        {
            TickTime = 0;
            _lastseconds = EntityGame.GameTime.TotalGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Ticks the timer and checks t see if we have reached our time limit.
        /// </summary>
        /// <param name="milliseconds">Total milliseconds for this timer to be active.</param>
        virtual public void Tick(GameTime gt)
        {
            TickTime = gt.TotalGameTime.TotalMilliseconds - _lastseconds;
            OnTick();
            if (Alive && TickTime >= Milliseconds)
            {
                _tr = true;
                OnLast();
                Reset();
            }
        }

        public override void Update(GameTime gt)
        {
            _tr = false;
            if (Alive)
                Tick(gt);
        }

        /// <summary>
        /// Tick event method
        /// </summary>
        protected virtual void OnTick()
        {
            if (TickEvent != null)
            {
                TickEvent();
            }
        }

        /// <summary>
        /// Timer up event method.
        /// </summary>
        protected virtual void OnLast()
        {
            if (LastEvent != null)
            {
                LastEvent();
            }
        }

        public void Start()
        {
            if (Alive) return;

            Alive = true;
            TickTime = 0;
            _lastseconds = EntityGame.GameTime.TotalGameTime.TotalMilliseconds;
        }

        public void Pause()
        {
            Alive = false;
        }

        public void Stop()
        {
            Alive = false;
            TickTime = 0;
            _lastseconds = EntityGame.GameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}