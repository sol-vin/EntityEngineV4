using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace EntityEngineV4.GUI
{
    public class Page : Node
    {
        public delegate void ControlEventHandler(Control c);
        private Control[,] _controls = new Control[1,1];

        public int MaxTabX
        {
            get { return _controls.GetUpperBound(0); }
        }

        public int MaxTabY
        {
            get { return _controls.GetUpperBound(1); }
        }

        public bool HasFocus;

        private Point _lastControl = Point.Zero;
        private Control LastControl
        {
            get { return _controls[_lastControl.X, _lastControl.Y]; }
        }
        private Point _currentcontrol = Point.Zero;

        public Control CurrentControl
        {
            get { return _controls[_currentcontrol.X, _currentcontrol.Y]; }
        }

        private Stack<Control> _processContols = new Stack<Control>(); 

        public Page(Node parent, string name) : base(parent, name)
        {
            //Check is ControlHandler is in this state, if not add it
            if (!GetRoot<State>().CheckService<ControlHandler>())
                new ControlHandler(GetRoot<State>());
        }

        public override void AddChild(Node node)
        {
            base.AddChild(node);

            if (node is Control)
            {
                _processContols.Push(node as Control);
            }
        }

        private void AddControl(Control c)
        {
            //Resize the collection to the appropriate size
            if (c.TabPosition.X >= MaxTabX && c.TabPosition.Y >= MaxTabY)
                ResizeCollection(c.TabPosition.X + 1, c.TabPosition.Y + 1);
            else if (c.TabPosition.X >= MaxTabX)
                ResizeCollection(c.TabPosition.X + 1, MaxTabY + 1);
            else if (c.TabPosition.Y >= MaxTabY)
                ResizeCollection(MaxTabX + 1, c.TabPosition.Y + 1);

            //FocusChanged += c.OnFocusChange;
            _controls[c.TabPosition.X, c.TabPosition.Y] = c;

            EntityGame.Log.Write("Control " + c.Name + " added", this, Alert.Info);
                
        }

        public override bool RemoveChild(Node node)
        {
            bool wasRemoved =  base.RemoveChild(node);
            if (wasRemoved && node is Control)
            {
                var c = node as Control;
                _controls[c.TabPosition.X, c.TabPosition.Y] = null;
            }
            return wasRemoved;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            ProcessControls();
        }

        public void ProcessControls()
        {
            while (_processContols.Count != 0)
            {
                AddControl(_processContols.Pop());
            }
        }

        /// <summary>
        /// Called when the focus has changed
        /// </summary>
        private void OnFocusChange()
        {
            if (_lastControl == _currentcontrol)
            {
                CurrentControl.OnFocusGain();
            }
            else
            {
                if(LastControl != null)
                    LastControl.OnFocusLost(CurrentControl);
                CurrentControl.OnFocusGain();
            }
            
        }

        /// <summary>
        /// Resizes _controls to the specified size
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ResizeCollection(int x, int y)
        {
            if (x >= _controls.GetUpperBound(1) || y >= _controls.GetUpperBound(1))
            {
                var clone = (Control[,])_controls.Clone();
                _controls = new Control[x,y];

                foreach (var c in clone)
                {
                    if (c == null) continue;
                    _controls[c.TabPosition.X, c.TabPosition.Y] = c;
                }
            }
        }

        /// <summary>
        /// Attempts to move the focus to the up control, follows any modifiers
        /// </summary>
        public void MoveFocusUp()
        {
            _lastControl = _currentcontrol;
            do
            {
                _currentcontrol.Y--;
                if (_currentcontrol.Y < 0)
                    _currentcontrol.Y = MaxTabY;
                if (CurrentControl.GetType().IsSubclassOf(typeof(TabModifier)))//Find if our control is a TabModifier
                {
                    //Find if it is blocking our path
                    TabModification tm = (CurrentControl as TabModifier).Modifier;
                    if ((tm & TabModification.UpBlock) > 0)
                    {
                        MoveFocusDown();
                        break; //Break out to stop it from coninuing it's path

                    }

                    if ((tm & TabModification.Insanity) > 0)
                    {
                        FollowTabModifier(tm); //Follow the mod
                        break; //Break out to stop it from coninuing it's path
                    }

                    //If it doesn't have a modifier, continue following...
                    
                }
            } while (CurrentControl == null || !CurrentControl.Selectable);
            OnFocusChange();
        }

        /// <summary>
        /// Attempts to move the focus to the down control, follows any modifiers
        /// </summary>
        public void MoveFocusDown()
        {
            _lastControl = _currentcontrol;

            do
            {
                _currentcontrol.Y++;
                if (_currentcontrol.Y > MaxTabY)
                    _currentcontrol.Y = 0;
                if (CurrentControl.GetType().IsSubclassOf(typeof(TabModifier)))//Find if our control is a TabModifier
                {
                    //Find if it is blocking our path
                    TabModification tm = (CurrentControl as TabModifier).Modifier;
                    if ((tm & TabModification.DownBlock) > 0)
                    {
                        MoveFocusUp();
                        break; //Break out to stop it from coninuing it's path
                    }

                    if ((tm & TabModification.Insanity) > 0)
                    {
                        FollowTabModifier(tm); //Follow the mod
                        break; //Break out to stop it from coninuing it's path
                    }

                }

                
            } while (CurrentControl == null || !CurrentControl.Selectable);
            OnFocusChange();
        }

        /// <summary>
        /// Attempts to move the focus to the left control, follows any modifiers
        /// </summary>
        public void MoveFocusLeft()
        {
            _lastControl = _currentcontrol;

            do
            {
                _currentcontrol.X--;

                if (_currentcontrol.X < 0)
                    _currentcontrol.X = MaxTabX;
                if (CurrentControl.GetType().IsSubclassOf(typeof(TabModifier)))//Find if our control is a TabModifier
                {
                    //Find if it is blocking our path
                    TabModification tm = (CurrentControl as TabModifier).Modifier;
                    if ((tm & TabModification.LeftBlock) > 0)
                    {
                        MoveFocusRight();
                        break; //Break out to stop it from coninuing it's path

                    }

                    if ((tm & TabModification.Insanity) > 0)
                    {
                        FollowTabModifier(tm); //Follow the mod
                        break; //Break out to stop it from coninuing it's path
                    }

                    //If it doesn't have a modifier, continue following...

                }
            } while (CurrentControl == null || !CurrentControl.Selectable);
            OnFocusChange();
        }

        /// <summary>
        /// Attempts to move the focus to the right control, follows any modifiers
        /// </summary>
        public void MoveFocusRight()
        {
            _lastControl = _currentcontrol;

            do
            {
                _currentcontrol.X++;
                if (_currentcontrol.X > MaxTabX)
                    _currentcontrol.X = 0;
                if (CurrentControl.GetType().IsSubclassOf(typeof(TabModifier)))//Find if our control is a TabModifier
                {
                    //Find if it is blocking our path
                    TabModification tm = (CurrentControl as TabModifier).Modifier;
                    if ((tm & TabModification.RightBlock) > 0)
                    {
                        MoveFocusLeft();
                        break; //Break out to stop it from coninuing it's path

                    }

                    if ((tm & TabModification.Insanity) > 0)
                    {
                        FollowTabModifier(tm); //Follow the mod
                        break; //Break out to stop it from coninuing it's path
                    }

                    //If it doesn't have a modifier, continue following...

                }

                
            } while (CurrentControl == null || !CurrentControl.Selectable);
            OnFocusChange();
        }

        /// <summary>
        /// Reads the TabModification field in TabModifier and does the move action specified
        /// </summary>
        /// <param name="tm"></param>
        private void FollowTabModifier(TabModification tm)
        {
            if ((tm & TabModification.MoveUp) > 0)
            {
                MoveFocusUp();
            }
            else if ((tm & TabModification.MoveDown) > 0)
            {
                MoveFocusDown();
            }
            else if ((tm & TabModification.MoveRight) > 0)
            {
                MoveFocusLeft();
            }
            else if ((tm & TabModification.MoveLeft) > 0)
            {
                MoveFocusRight();
            }
        }

        public void FocusOn(Control c)
        {
            if(c.Parent != this) throw new Exception("Control cannot tell a page that is not it's parent to focus on it");
            _lastControl = _currentcontrol;
            _currentcontrol = c.TabPosition;
            OnFocusChange();
        }

        /// <summary>
        /// Presses the current control
        /// </summary>
        public void Press()
        {
            CurrentControl.Press();
        }

        /// <summary>
        /// Releases the current control
        /// </summary>
        public void Release()
        {
            CurrentControl.Release();
        }
        
        /// <summary>
        /// Holds down the current control
        /// </summary>
        public void Down()
        {
            CurrentControl.Down();
        }

        public Control GetControl(Point tabPosition)
        {
            return GetControl(tabPosition.X, tabPosition.Y);
        }

        public Control GetControl(int x, int y)
        {
            if (x < 0 || y < 0) throw new IndexOutOfRangeException();

            return _controls[x, y];
        }

        public void Show()
        {
            GetRoot<State>().GetService<ControlHandler>().NotifyFocusChange(this);
            Active = true;
            HasFocus = true;
        }


        public virtual void Hide()
        {
            Active = false;
            HasFocus = false;
        }
    }
}
