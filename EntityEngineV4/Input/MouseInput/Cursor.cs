using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Input.MouseInput
{
    public class Cursor : Entity
    {
        //Link to the containing service
        public MouseHandler MouseHandler;

        //Hidden because we don't want people messing with it all willy nilly
        protected Body Body;

        public Render Render;

        public Point Position
        {
            get
            {
                return new Point((int)Body.Position.X, (int)Body.Position.Y);
            }
            set
            {
                Body.Position.X = value.X;
                Body.Position.Y = value.Y;
            }
        }

        public Cursor(EntityState stateref, string name, MouseHandler mh)
            : base(stateref, stateref, name)
        {
            MouseHandler = mh;

            Body = new Body(this, "Body");
            Body.Position = new Vector2(400, 300);

            //Default rendering is a single white pixel.
            Render = new ImageRender(this, "ImageRender", Assets.Pixel, Body);
            Render.Layer = 1f;
            Render.Scale = Vector2.One * 3f;
            Render.Color = Color.Black;

            Body.Bounds = Render.Scale;
        }

        public override void Update(GameTime gt)
        {
            Position = new Point(Position.X - MouseHandler.Delta.X, Position.Y - MouseHandler.Delta.Y);

            //Keep it from leaving the bounds of the window.
            if (Body.Position.X < 0) Body.Position.X = 0;
            else if (Body.BoundingRect.Right > EntityGame.Viewport.Width)
                Body.Position.X = EntityGame.Viewport.Width - Body.Bounds.X;

            if (Body.Position.Y < 0) Body.Position.Y = 0;
            else if (Body.BoundingRect.Bottom > EntityGame.Viewport.Height)
                Body.Position.Y = EntityGame.Viewport.Height - Body.Bounds.Y;

            base.Update(gt);
        }
    }
}