using EntityEngineV4.Engine;
using EntityEngineV4.Input.MouseInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{
    public delegate void ControlEventHandler(Control c);

    public class ControlHandler : Service
    {
        private Control[,] _controls;

        public bool UseMouse = true;

        public int MaxTabX
        {
            get { return _controls.GetUpperBound(0); }
        }

        public int MaxTabY
        {
            get { return _controls.GetUpperBound(1); }
        }

        private Point _currentcontrol = Point.Zero;

        public Control CurrentControl
        {
            get { return _controls[_currentcontrol.X, _currentcontrol.Y]; }
        }

        public event ControlEventHandler FocusChanged;

        public ControlHandler(EntityState stateref)
            : base(stateref)
        {
            _controls = new Control[1, 1];
        }

        public override void Update(GameTime gt)
        {
            foreach (var control in _controls)
            {
                if (control == null || !control.Active) continue;

                control.Update(gt);
                if (control.Selectable && TestMouseCollision(control))
                {
                    CurrentControl.OnFocusLost(CurrentControl);
                    _currentcontrol = control.TabPosition;
                    control.OnFocusGain(control);
                    if (MouseHandler.IsMouseButtonReleased(MouseButton.LeftButton))
                        control.Select();
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            foreach (var c in _controls)
            {
                if (c != null && c.Visible)
                    c.Draw(sb);
            }
        }

        public void AddControl(Control c)
        {
            if (c.TabPosition.X >= MaxTabX && c.TabPosition.Y >= MaxTabY)
                ResizeControlCollection(c.TabPosition.X + 1, c.TabPosition.Y + 1);
            else if (c.TabPosition.X >= MaxTabX)
                ResizeControlCollection(c.TabPosition.X + 1, MaxTabY + 1);
            else if (c.TabPosition.Y >= MaxTabY)
                ResizeControlCollection(MaxTabX + 1, c.TabPosition.Y + 1);

            FocusChanged += c.OnFocusChange;
            _controls[c.TabPosition.X, c.TabPosition.Y] = c;
        }

        public void RemoveControl(Control c)
        {
            FocusChanged -= c.OnFocusChange;
            _controls[c.TabPosition.X, c.TabPosition.Y] = null;
        }

        private void ResizeControlCollection(int x, int y)
        {
            var controlcopy = (Control[,])_controls.Clone();
            _controls = new Control[x, y];

            foreach (var c in controlcopy)
            {
                if (c == null)
                    continue;
                _controls[c.TabPosition.X, c.TabPosition.Y] = c;
            }
        }

        public void OnFocusChange(Control c)
        {
            if (FocusChanged != null)
                FocusChanged(c);
        }

        public void UpControl()
        {
            if (CurrentControl.TabStop.HasFlag(TabStop.Up))
                return;
            do
            {
                _currentcontrol.Y--;
                if (_currentcontrol.Y < 0)
                    _currentcontrol.Y = MaxTabY;
            } while (CurrentControl == null || !CurrentControl.Selectable);
            OnFocusChange(CurrentControl);
        }

        public void DownControl()
        {
            if (CurrentControl.TabStop.HasFlag(TabStop.Down))
                return;
            do
            {
                _currentcontrol.Y++;
                if (_currentcontrol.Y > MaxTabY)
                    _currentcontrol.Y = 0;
            } while (CurrentControl == null || !CurrentControl.Selectable);
            OnFocusChange(CurrentControl);
        }

        public void LeftControl()
        {
            if (CurrentControl.TabStop.HasFlag(TabStop.Left))
                return;
            do
            {
                _currentcontrol.X--;
                if (_currentcontrol.X < 0)
                    _currentcontrol.X = MaxTabX;
            } while (CurrentControl == null || !CurrentControl.Selectable);
            OnFocusChange(CurrentControl);
        }

        public void RightControl()
        {
            if (CurrentControl.TabStop.HasFlag(TabStop.Right))
                return;
            do
            {
                _currentcontrol.X++;
                if (_currentcontrol.X > MaxTabX)
                    _currentcontrol.X = 0;
            } while (CurrentControl == null || !CurrentControl.Selectable);
            OnFocusChange(CurrentControl);
        }

        public void Select()
        {
            CurrentControl.Select();
        }

        public void MouseCollisionUpdate()
        {
            foreach (var control in _controls)
            {
                if (control.Selectable && TestMouseCollision(control))
                {
                    control.OnFocusGain(control);
                    if (MouseHandler.IsMouseButtonReleased(MouseButton.LeftButton))
                        control.Select();
                    break;
                }
                if (control.Selectable && control.HasFocus)
                    control.OnFocusLost(control);
            }
        }

        public bool TestMouseCollision(Control c)
        {
            return c.Body.BoundingRect.Contains(new Point((int)MouseHandler.Cursor.Position.X, (int)MouseHandler.Cursor.Position.Y));
        }
    }
}