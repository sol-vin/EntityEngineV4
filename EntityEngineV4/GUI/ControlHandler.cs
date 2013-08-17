using System;
using EntityEngineV4.Data;
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

        public bool UseMouse;

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

        public ControlHandler(EntityState stateref, bool useMouse = true)
            : base(stateref, "ControlHandler")
        {
            _controls = new Control[1, 1];

            UseMouse = useMouse;

            //Check to see if the mouse service is present, if not, log and disable mouse
            if (!stateref.CheckService<MouseHandler>())
            {
                UseMouse = false;
                EntityGame.Log.Write("Mouse was disabled on this state, switching UseMouse to false. Next time, please specify UseMouse in constructor", this, Alert.Warning);
            }
        }

        public override void Update(GameTime gt)
        {
            UpdateMouse();
        }

        public override void Draw(SpriteBatch sb)
        {
        }

        public void AddControl(Control c)
        {
            DestroyEvent += c.Destroy;

            //Resize the collection to the appropriate size
            if (c.TabPosition.X >= MaxTabX && c.TabPosition.Y >= MaxTabY)
                ResizeControlCollection(c.TabPosition.X + 1, c.TabPosition.Y + 1);
            else if (c.TabPosition.X >= MaxTabX)
                ResizeControlCollection(c.TabPosition.X + 1, MaxTabY + 1);
            else if (c.TabPosition.Y >= MaxTabY)
                ResizeControlCollection(MaxTabX + 1, c.TabPosition.Y + 1);

            //Find out if anything is at that space
            if (_controls[c.TabPosition.X, c.TabPosition.Y] != null)
            {
                //We have to move the control to the slot to the right of it.
                //This will move evry control in that row because setting the 
                //Control.TabPosition property triggers an AddControl and RemoveControl.
                //Essentially, this is recurrsive so don't worry about it.
                _controls[c.TabPosition.X, c.TabPosition.Y].TabPosition = new Point(c.TabPosition.X + 1, c.TabPosition.Y);
            }

            FocusChanged += c.OnFocusChange;
            _controls[c.TabPosition.X, c.TabPosition.Y] = c;

            EntityGame.Log.Write("Control " + c.Name + " added", this, Alert.Info);
        }

        public void RemoveControl(Control c)
        {
            FocusChanged -= c.OnFocusChange;
            if (_controls[c.TabPosition.X, c.TabPosition.Y] != null)
            {
                _controls[c.TabPosition.X, c.TabPosition.Y].Destroy();
                _controls[c.TabPosition.X, c.TabPosition.Y] = null;
            }

            RemoveEntity(c);
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

            EntityGame.Log.Write("Focus changed to " + c.Name, this, Alert.Trivial);
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

        public void Press()
        {
            CurrentControl.Press();
        }

        public void Release()
        {
            CurrentControl.Release();
        }

        public void Down()
        {
            CurrentControl.Down();
        }

        public bool TestMouseCollision(Control c)
        {
            return c.Body.BoundingRect.Contains(new Point((int)MouseHandler.Cursor.Position.X, (int)MouseHandler.Cursor.Position.Y));
        }

        public void UpdateMouse()
        {
            //TODO: Fix this so the controller can use the menus!
             if (!UseMouse) return;
            foreach (var control in _controls)
            {
                if (control == null || !control.Enabled || !control.Selectable) continue;

                if (TestMouseCollision(control) )
                {
                    if(CurrentControl != null)
                        CurrentControl.OnFocusLost(CurrentControl);
                    _currentcontrol = control.TabPosition;
                    control.OnFocusGain(control);
                    if (MouseHandler.Cursor.Pressed())
                        control.Press();
                    else if (MouseHandler.Cursor.Released())
                        control.Release();

                    if (MouseHandler.Cursor.Down())
                        control.Down();
                }
                else if (CurrentControl == control) //If control is the current control and it lost focus
                {
                    CurrentControl.OnFocusLost(control);
                }
            }
        }

        public Control GetControl(Point tabPosition)
        {
            return GetControl(tabPosition.X, tabPosition.Y);
        }

        public Control GetControl(int x, int y)
        {
            if(x < 0 || y < 0) throw new IndexOutOfRangeException();

            return _controls[x, y];
        }
    }
}