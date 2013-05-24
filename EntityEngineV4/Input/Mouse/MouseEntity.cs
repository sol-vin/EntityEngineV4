using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;

namespace EntityEngineV4.Input.MouseInput
{
	public class MouseEntity : Entity
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

				//If the mouse isn't locked to the screen then we should change it's position as well 
				if(!MouseHandler.LockMouse)
					MouseHandler.SetPositon(value);
			}
		}

		public MouseEntity(EntityState stateref, IComponent parent, string name, MouseHandler mh) 
			: base(stateref, parent, name)
		{
			MouseHandler = mh;

			Body = new Body(this, "Body");

			//Default rendering is a single black pixel.
			Render = new ImageRender(this, "ImageRender", Assets.Pixel, Body);
			Render.Layer = 1f;
			Render.Scale = Vector2.One * 100;

			TextRender = new TextRender(this, "TextRender", new Body(this, "TextRender.Body", new Vector2(400,400)));
			TextRender.LoadFont(@"TestState/font");
			TextRender.Color = Color.Black;
		}


		public override void Update (GameTime gt)
		{
			base.Update (gt);
		}
	}
}

