using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Components
{
    public class Timer : Component
    {
        public bool Alive { get; private set; }

        /// <summary>
        /// The event delegate that will be used for event functions
        /// </summary>
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
        /// Milliseconds before the timer should reset.
        /// </summary>
        private int _millseconds;
        public int Milliseconds { get { return _millseconds; } 
            set
            {
                if(value < 0)
                {
                    EntityGame.Log.Write("Milliseconds was attempted to set a negative value!", this, Alert.Warning);
                    _millseconds = 0;
                    return;
                }
                _millseconds = value;
            }
        }

        public int CurrentMilliseconds
        {
           get
           {
               return (int)( EntityGame.GameTime.ElapsedGameTime.TotalMilliseconds - _lastseconds);
           }
        }

        private bool _tr;

        public bool TimeReached
        {
            get { return _tr; }
        }

        public float Progress
        {
            get { return MathHelper.Clamp((float)CurrentMilliseconds/Milliseconds, 0, 1); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        public Timer(Node parent, string name)
            : base(parent, name)
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
                Reset(); //TODO: Ensure that removing this is ok :)
            }
            //TODO: Double check this works for timers still :)
            if (!Alive)
            {
                _lastseconds += gt.ElapsedGameTime.TotalMilliseconds;
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
            while (TickTime > Milliseconds)
            {
                TickTime -= Milliseconds;
                if (LastEvent != null)
                {
                    LastEvent();
                }
            }
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            if (Alive) return;

            Alive = true;
            TickTime = 0;
            _lastseconds = EntityGame.GameTime.TotalGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Pauses the timere
        /// </summary>
        public void Pause()
        {
            //TODO: Fix the pause method, it needs to actually pause it, the timer is still running and
            //when un paused will immeadiately fire
          
            Alive = false;
        }

        /// <summary>
        /// Stops the timer. Can be subscribed to the LastEvent event to stop it once it reaches it's maximum time.
        /// </summary>
        public void Stop()
        {
            Alive = false;
            TickTime = 0;
            _lastseconds = EntityGame.GameTime.TotalGameTime.TotalMilliseconds;
        }

        public override void Reuse(Node parent, string name)
        {
            base.Reuse(parent, name);

            TickEvent = null;
            LastEvent = null;
            TickTime = 0;
            _lastseconds = 0;
            Alive = false;
        }
    }
}