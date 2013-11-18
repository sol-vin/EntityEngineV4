using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4.Input
{
    public class MouseService : Service
    {
        public static Cursor Cursor { get; set; }
        private static MouseState _mousestate;

        public static MouseState MouseState
        {
            get { return _mousestate; }
        }

        private static MouseState _lastmousestate;

        public static MouseState LastMouseState
        {
            get { return _lastmousestate; }
        }

        public static bool LockMouse = true;
        private readonly int _lockx = EntityGame.Viewport.Width / 2;
        private readonly int _locky = EntityGame.Viewport.Height / 2;

        public static Point Delta { get; private set; }

        public MouseService(State stateref, bool useDefaultCursors = false)
            : base(stateref, "MouseService")
        {
            if (!useDefaultCursors)
            {
                AddDefaultCursors();
            }

            _mousestate = Mouse.GetState();
            Flush();
        }

        public void AddDefaultCursors()
        {
            MouseCursor mc = new MouseCursor(this, "MouseService.MouseCursor");
            mc.GetFocus(mc);
            
            var cc = new ControllerCursor(this, "MouseService.ControllerCursor", ControllerCursor.MovementInput.Buttons);
            cc.Visible = false;
        }

        public void Initialize()
        {
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            Flush();
            _mousestate = Mouse.GetState();

            if (LockMouse && EntityGame.Game.IsActive)
            {
                //After we get our states, we can reset the Mouse with no problems!
                SetPosition(_lockx, _locky);
                //Be sure to calc delta by using the
                Delta = new Point(_lockx - _mousestate.X, _locky - _mousestate.Y);
            }
            else
            {
                //Calc our Delta
                Delta = new Point(_lastmousestate.X - _mousestate.X, _lastmousestate.Y - _mousestate.Y);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public static Vector2 GetPosition()
        {
            return new Vector2(_mousestate.X, _mousestate.Y);
        }

        public static void SetPosition(int x, int y)
        {
            Mouse.SetPosition(x, y);
        }

        public static void Flush()
        {
            _lastmousestate = _mousestate;
        }

        public override void Destroy(IComponent sender = null)
        {
            base.Destroy(sender);

            Flush();

            Cursor = null;
        }

        public static bool IsMouseButtonDown(MouseButton mb)
        {
            switch (mb)
            {
                case MouseButton.LeftButton:
                    return _mousestate.LeftButton == ButtonState.Pressed;
                case MouseButton.MiddleButton:
                    return _mousestate.MiddleButton == ButtonState.Pressed;
                case MouseButton.RightButton:
                    return _mousestate.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return _mousestate.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return _mousestate.XButton2 == ButtonState.Pressed;
            }
            throw new Exception("MouseButton does not exist!");
        }

        public static bool IsMouseButtonPressed(MouseButton mb)
        {
            switch (mb)
            {
                case MouseButton.LeftButton:
                    return _mousestate.LeftButton == ButtonState.Pressed && _lastmousestate.LeftButton == ButtonState.Released;
                case MouseButton.RightButton:
                    return _mousestate.RightButton == ButtonState.Pressed && _lastmousestate.RightButton == ButtonState.Released;
                case MouseButton.MiddleButton:
                    return _mousestate.MiddleButton == ButtonState.Pressed && _lastmousestate.MiddleButton == ButtonState.Released;
                case MouseButton.XButton1:
                    return _mousestate.XButton1 == ButtonState.Pressed && _lastmousestate.XButton1 == ButtonState.Released;
                case MouseButton.XButton2:
                    return _mousestate.XButton2 == ButtonState.Pressed && _lastmousestate.XButton2 == ButtonState.Released;
            }
            throw new Exception("MouseButton does not exist!");
        }

        public static bool IsMouseButtonUp(MouseButton mb)
        {
            switch (mb)
            {
                case MouseButton.LeftButton:
                    return _mousestate.LeftButton == ButtonState.Released;
                case MouseButton.MiddleButton:
                    return _mousestate.MiddleButton == ButtonState.Released;
                case MouseButton.RightButton:
                    return _mousestate.RightButton == ButtonState.Released;
                case MouseButton.XButton1:
                    return _mousestate.XButton1 == ButtonState.Released;
                case MouseButton.XButton2:
                    return _mousestate.XButton2 == ButtonState.Released;
            }
            throw new Exception("MouseButton does not exist!");
        }

        public static bool IsMouseButtonReleased(MouseButton mb)
        {
            switch (mb)
            {
                case MouseButton.LeftButton:
                    return _mousestate.LeftButton == ButtonState.Released && _lastmousestate.LeftButton == ButtonState.Pressed;
                case MouseButton.RightButton:
                    return _mousestate.RightButton == ButtonState.Released && _lastmousestate.RightButton == ButtonState.Pressed;
                case MouseButton.MiddleButton:
                    return _mousestate.MiddleButton == ButtonState.Released && _lastmousestate.MiddleButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return _mousestate.XButton1 == ButtonState.Released && _lastmousestate.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return _mousestate.XButton2 == ButtonState.Released && _lastmousestate.XButton2 == ButtonState.Pressed;
            }
            throw new Exception("MouseButton does not exist!");
        }
    }

    public enum MouseButton
    {
        LeftButton, RightButton, MiddleButton, XButton1, XButton2
    }
}