using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Input.MouseInput
{
    public class Cursor : Entity
    {
        //Hidden because we don't want people messing with it all willy nilly
        protected Body Body;

        public Render Render;

        public Vector2 Position
        {
            get
            {
                return Body.Position;
            }
            set { Body.Position = value; }
        }

        public Cursor(EntityState stateref, string name)
            : base(stateref, stateref, name)
        {
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
            Position = new Vector2(Position.X - MouseHandler.Delta.X, Position.Y - MouseHandler.Delta.Y);

            //Move it with the camera.
            Position += EntityGame.CurrentCamera.Delta;

            //Keep it from leaving the bounds of the window.
            if (Body.Position.X < EntityGame.CurrentCamera.ScreenSpace.Left) Body.Position.X = EntityGame.CurrentCamera.ScreenSpace.Left;
            else if (Body.BoundingRect.Right > EntityGame.CurrentCamera.ScreenSpace.Right)
                Body.Position.X = EntityGame.CurrentCamera.ScreenSpace.Right - Body.Bounds.X;

            if (Body.Position.Y < EntityGame.CurrentCamera.ScreenSpace.Top) Body.Position.Y = EntityGame.CurrentCamera.ScreenSpace.Top;
            else if (Body.BoundingRect.Bottom > EntityGame.CurrentCamera.ScreenSpace.Bottom)
                Body.Position.Y = EntityGame.CurrentCamera.ScreenSpace.Bottom - Body.Bounds.Y;


            base.Update(gt);
        }
    }
}