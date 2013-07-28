using System;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{
    public abstract class Control : Entity
    {
        public Body Body;

        /// <summary>
        /// This is the position where the GUI control resdies on the page when tabbing through.
        /// It is a point because you can go up, down, left, and right.
        /// </summary>
        private Point _tabposition;
        public Point TabPosition
        {
            get { return _tabposition; }
            set
            {
                //Sanity Check
                if (value.X < 0 || value.Y < 0)
                    throw new IndexOutOfRangeException(Name + ".TabPosition cannot be less than 0!");
                _tabposition = value;
            }
        }

        public bool HasFocus;
        public bool Enabled = true;
        public bool Selectable = true;

        private TabStop _tabstop;

        public TabStop TabStop
        {
            get { return _tabstop; }
            set
            {
                if ((value.HasFlag(TabStop.Left) && value.HasFlag(TabStop.Right)) ||
                    (value.HasFlag(TabStop.Up) && value.HasFlag(TabStop.Down)))
                    throw new Exception("Can't have a value be both X/Y TabStops!");
                _tabstop = value;
            }
        }

        //Easy access area
        public float X { get { return Body.Position.X; } set { Body.Position.X = value; } }
        public float Y { get { return Body.Position.Y; } set { Body.Position.Y = value; } }
        public float Width { get { return Body.Bounds.X; } set { Body.Bounds.X = value; } }
        public float Height { get { return Body.Bounds.X; } set { Body.Bounds.Y= value; } }

        public event ControlEventHandler Selected;

        protected Control(EntityState stateref, string name)
            : base(stateref, stateref, name)
        {
            Body = new Body(this, "Body");
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public virtual void OnFocusChange(Control c)
        {
            if (c.Equals(this))
                OnFocusGain(this);
            else
                OnFocusLost(c);
        }

        public virtual void OnFocusLost(Control c)
        {
            HasFocus = false;
        }

        public virtual void OnFocusGain(Control c)
        {
            HasFocus = true;
        }

        public virtual void Select()
        {
            if (!Selectable) return;
            if (Selected != null)
                Selected(this);
        }
    }

    [Flags]
    public enum TabStop
    {
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8
    }
}