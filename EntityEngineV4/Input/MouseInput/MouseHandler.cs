using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4.Input.MouseInput
{
	public class MouseHandler : Service
	{
		public Cursor Cursor;
		private static MouseState _mousestate;
		public static MouseState MouseState {
			get { return _mousestate;}
		}
		private static MouseState _lastmousestate;
		public static MouseState LastMouseState {
			get { return _lastmousestate;}
		}

	    public bool LockMouse;

		public MouseHandler (EntityState stateref) : base(stateref)
		{
			Cursor = new Cursor(stateref, stateref, "Cursor", this);
		}

		public override void Update(GameTime gt)
		{
			Flush();
			_mousestate = Mouse.GetState();

			if (LockMouse)
			{
				int lockx = EntityGame.Viewport.Width/2;
				int locky = EntityGame.Viewport.Height/2;
				
				//After we get our states, we can reset the Mouse with no problems!
				SetPosition(lockx, locky);
				Point distance = new Point(lockx - _mousestate.X, locky - _mousestate.Y);
				Cursor.Position = new Point(Cursor.Position.X - distance.X, Cursor.Position.Y - distance.Y);
			}
			else
			{
				//Calc our distance and add it to our cursor.
				Point distance = new Point(_lastmousestate.X - _mousestate.X, _lastmousestate.Y - _mousestate.Y);
				Cursor.Position = new Point(Cursor.Position.X - distance.X, Cursor.Position.Y - distance.Y);
			}
			Cursor.Update(gt);
		}

		public override void Draw(SpriteBatch sb)
		{
			Cursor.Draw(sb);
		}

		public Point GetPosition()
		{
			return new Point(_mousestate.X, _mousestate.Y);
		}

		public void SetPosition(int x, int y)
		{
			Mouse.SetPosition(x, y);
		}

		public void Flush()
		{
			_lastmousestate = _mousestate;
		}

		public bool IsMouseButtonDown(MouseButton mb)
		{
			switch(mb)
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

		public bool IsMouseButtonPressed (MouseButton mb)
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

		public bool IsMouseButtonUp(MouseButton mb)
		{
			switch(mb)
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

		public bool IsMouseButtonReleased (MouseButton mb)
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

