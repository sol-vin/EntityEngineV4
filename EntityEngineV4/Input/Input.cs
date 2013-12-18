using EntityEngineV4.CollisionEngine;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Input
{
    public class Input : Component
    {
        private int _holdtime;
        private int _rapidfire;

        public delegate void EventHandler(Input i);

        public event EventHandler PressedEvent, ReleasedEvent, DownEvent, UpEvent;

        public Input(Node node, string name)
            : base(node, name)
        {
            if (!GetRoot<State>().CheckService<InputService>())
                new InputService(GetRoot<State>());

        }

        /// <summary>
        /// Returns true if the input has just been released
        /// </summary>
        /// <returns></returns>
        public virtual bool Released()
        {
            return false;
        }

        /// <summary>
        /// Returns true if the input has just been pressed
        /// </summary>
        /// <returns></returns>
        public virtual bool Pressed()
        {
            return false;
        }

        /// <summary>
        /// Returns true if the input is down.
        /// </summary>
        /// <returns></returns>
        public virtual bool Down()
        {
            return false;
        }

        /// <summary>
        /// Returns true of the input is up.
        /// </summary>
        /// <returns></returns>
        public virtual bool Up()
        {
            return false;
        }

        /// <summary>
        /// Will return true if the button is down and a certian amount of time has passed
        /// </summary>
        /// <param name="milliseconds">The milliseconds between firing.</param>
        /// <returns></returns>
        public virtual bool RapidFire(int milliseconds)
        {
            _rapidfire = milliseconds;

            if (Pressed()) //if the button has just been pressed
            {
                if (_holdtime == 0) //if the timer has not been started
                {
                    _holdtime = 1;
                    return true;
                }
            }

            else if (Down())
            {
                if (_holdtime == 0 || _holdtime > _rapidfire)
                {
                    _holdtime = 1;
                    return true;
                }
            }
            return false;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (_holdtime != 0)
            {
                _holdtime += gt.ElapsedGameTime.Milliseconds;
                if (_holdtime > _rapidfire)
                {
                    _holdtime = 0;
                }
            }

            //Checks to fire events
            if (Up())
            {
                if (UpEvent != null) UpEvent(this);
            }
            else
                if (DownEvent != null) DownEvent(this);

            if (Pressed())
            {
                if (PressedEvent != null) PressedEvent(this);
            }
            else if (Released())
            {
                if (ReleasedEvent != null) ReleasedEvent(this);
            }

        }
    }
}