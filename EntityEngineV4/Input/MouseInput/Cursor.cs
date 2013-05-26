using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;

namespace EntityEngineV4.Input.MouseInput
{
	public class Cursor : Entity
	{
		//Link to the containing service
		public MouseHandler MouseHandler;

		//Hidden because we don't want people messing with it all willy nilly
		protected Body Body;
		public Render Render;
		public TextRender TextRender;

		public Point Position {
			get {
				return new Point ((int)Body.Position.X, (int)Body.Position.Y);
			}
			set {
				Body.Position.X = value.X;
				Body.Position.Y = value.Y;
			}
		}

		public Cursor(EntityState stateref, IComponent parent, string name, MouseHandler mh) 
			: base(stateref, parent, name)
		{
			MouseHandler = mh;

			Body = new Body(this, "Body");

			//Default rendering is a single white pixel.
			Render = new ImageRender(this, "ImageRender", Assets.Pixel, Body);
			Render.Layer = 1f;
			Render.Scale = Vector2.One * 100;

			var body =  new Body(this, "TextRender.Body", new Vector2(200,200));

			TextRender = new TextRender(this, "TextRender", body);
			TextRender.LoadFont(@"TestState/font");
			TextRender.Color = Color.Black;
			TextRender.Text = MouseHandler.GetPosition().ToString();
			TextRender.Layer = .9f;
		}


		public override void Update (GameTime gt)
		{
			TextRender.Text = MouseHandler.GetPosition().ToString() + 
				"\n Mouse Lock:" + MouseHandler.LockMouse.ToString();

			Render.Color = Color.White;
			if (MouseHandler.IsMouseButtonDown(MouseButton.LeftButton))
				Render.Color = Color.Red;

			if (MouseHandler.IsMouseButtonPressed(MouseButton.RightButton))
				MouseHandler.LockMouse = !MouseHandler.LockMouse;
			base.Update (gt);
		}
	}
}

