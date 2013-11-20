using System;
using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{
    public abstract class Control : Node
    {
        public delegate void EventHandler(Control b);

        public override bool IsObject
        {
            get { return true; }
        }

        public Body Body;
        public ControlHandler ControlHandler { get; private set; }
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

                //Check to see if it actually has changed
                if (_tabposition == value)
                    return;

                //It's different, lets swap it around.
                
                //First copy if it was attached or not
                bool wasAttached = Attached;

                if(wasAttached)
                    ControlHandler.RemoveControl(this);

                _tabposition = value;

                if(wasAttached)
                    ControlHandler.AddControl(this);
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

        public float Height { get { return Body.Bounds.Y; } set { Body.Bounds.Y = value; } }

        public bool Attached
        {
            get
            {
                if (ControlHandler == null) return false;
                return ControlHandler.GetControl(TabPosition).Equals(this);
            }
        }

        protected Control(Node parent, string name)
            : base(parent, name)
        {
            Visible = true;
            Body = new Body(this, "Body");

            //Add our service if we have a parent, if not, we will let them update and draw
            if (parent != null && parent.GetType() != typeof (ControlHandler))
            {
                ControlHandler = GetRoot<State>().GetService<ControlHandler>();
                if (ControlHandler == null)
                {
                    EntityGame.Log.Write("ControlHandler was not found! Cannot attach to Service", this, Alert.Error);
                    //throw new Exception("ControlHandler was not found!");
                }
            }
            else if (parent != null && parent.GetType() == typeof(ControlHandler))
            {
                ControlHandler = parent as ControlHandler;
            }
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
            if (c == this)
                OnFocusGain(this);
            else
                OnFocusLost(c);
        }

        public event EventHandler FocusLost;
        public virtual void OnFocusLost(Control c)
        {
            HasFocus = false;
            if(FocusLost != null)
                FocusLost(c); 
        }

        public event EventHandler FocusGain;
        public virtual void OnFocusGain(Control c)
        {
            HasFocus = true;
            if(FocusGain != null)
                FocusGain(c);
        }

        public event EventHandler OnPressed;
        public virtual void Press()
        {
            if (OnPressed != null) OnPressed(this);
        }

        public event EventHandler OnReleased;
        public virtual void Release()
        {
            if(OnReleased != null) OnReleased(this);
        }

        public event EventHandler OnDown;
        public virtual void Down()
        {
            if (OnDown != null) OnDown(this);
        }

        public void AttachToControlHandler()
        {
            if (ControlHandler == null)
            {
                //dont attach to a null service
                EntityGame.Log.Write("Control attempted to attach to a null service!", Alert.Warning);
                return;
            }
            ControlHandler.AddControl(this);
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