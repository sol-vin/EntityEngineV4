using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{
    public abstract class Control : Node
    {
        public delegate void EventHandler(Control b);

        public Body Body;

        /// <summary>
        /// This is the position where the GUI control resdies on the page when tabbing through.
        /// It is a point because you can go up, down, left, and right.
        /// </summary>
        private Point _tabPosition;

        public Point TabPosition
        {
            get { return _tabPosition; }
        }

        public bool HasFocus;
        public bool Enabled = true;
        public bool Selectable = true;

        //Easy access area
        public float X { get { return Body.Position.X; } set { Body.Position.X = value; } }

        public float Y { get { return Body.Position.Y; } set { Body.Position.Y = value; } }

        public float Width { get { return Body.Bounds.X; } set { Body.Bounds.X = value; } }

        public float Height { get { return Body.Bounds.Y; } set { Body.Bounds.Y = value; } }

        protected Control(Page parent, string name, Point tabPosition)
            : base(parent, name)
        {
            _tabPosition = tabPosition;

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

        public event EventHandler FocusLost;

        public virtual void OnFocusLost(Control c)
        {
            HasFocus = false;
            if (FocusLost != null)
                FocusLost(c);
        }

        public event EventHandler FocusGain;

        public virtual void OnFocusGain()
        {
            HasFocus = true;
            if (FocusGain != null)
                FocusGain(this);
        }

        public event EventHandler OnPressed;

        public virtual void Press()
        {
            if (OnPressed != null) OnPressed(this);
        }

        public event EventHandler OnReleased;

        public virtual void Release()
        {
            if (OnReleased != null) OnReleased(this);
        }

        public event EventHandler OnDown;

        public virtual void Down()
        {
            if (OnDown != null) OnDown(this);
        }
    }
}