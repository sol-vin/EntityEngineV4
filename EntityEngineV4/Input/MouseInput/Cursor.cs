using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Input.MouseInput
{
    public abstract class Cursor : Entity
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

        public delegate void CursorEventHandler(Cursor c);

        public event CursorEventHandler GotFocus , LostFocus;

        public bool HasFocus { get; protected set; }

        public Cursor(IComponent parent, string name)
            : base(parent, name)
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

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public void OnGetFocus(Cursor c = null)
        {
            if (GotFocus != null) GotFocus(this);
            MouseHandler.Cursor = this;
            HasFocus = true;
        }

        public void OnLostFocus(Cursor newCursor)
        {
            if (LostFocus != null) LostFocus(newCursor);
            HasFocus = false;
        }

        public override void Destroy(IComponent i = null)
        {
            base.Destroy(i);

            GotFocus = null;
            LostFocus = null;
            if (HasFocus)
            {
                HasFocus = false;
                MouseHandler.Cursor = null;
            }
        }
    }

    public class ControllerCursor : Cursor
    {
        public ControllerCursor(IComponent parent, string name)
            : base(parent, name)
        {

        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }

    public class MouseCursor : Cursor
    {
        public MouseCursor(IComponent parent, string name) : base(parent, name)
        {

        }

        public override void Update(GameTime gt)
        {
            if(!HasFocus && MouseHandler.Delta != Point.Zero)
                OnGetFocus(this);
            if(HasFocus)
            {
                Position = new Vector2(Position.X - MouseHandler.Delta.X, Position.Y - MouseHandler.Delta.Y);

                //Move it with the camera.
                Position += EntityGame.Camera.Delta;

                //Keep it from leaving the bounds of the window.
                if (Body.Position.X < EntityGame.Camera.ScreenSpace.Left) Body.Position.X = EntityGame.Camera.ScreenSpace.Left;
                else if (Body.BoundingRect.Right > EntityGame.Camera.ScreenSpace.Right)
                    Body.Position.X = EntityGame.Camera.ScreenSpace.Right - Body.Bounds.X;

                if (Body.Position.Y < EntityGame.Camera.ScreenSpace.Top) Body.Position.Y = EntityGame.Camera.ScreenSpace.Top;
                else if (Body.BoundingRect.Bottom > EntityGame.Camera.ScreenSpace.Bottom)
                    Body.Position.Y = EntityGame.Camera.ScreenSpace.Bottom - Body.Bounds.Y;
            }

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {            
            //Stop the draw method to render this on screen without camera interference!
            EntityGame.StopDrawing();
            EntityGame.StartDrawing();
            base.Draw(sb);
            EntityGame.StopDrawing();
            EntityGame.StartDrawing(EntityGame.Camera);
        }
    }
}