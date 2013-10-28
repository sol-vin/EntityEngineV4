using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Data
{
    public class Camera : Entity
    {
        //TODO: Add rotation to camera
        //TODO: Add collision detection to OBB for camera 
        //      ^to test if it should render an object or not

        public readonly Vector2 DEFAULTVIEW = new Vector2(EntityGame.Viewport.Width / 2f, EntityGame.Viewport.Height / 2f);

        public Vector2 LastPosition { get; private set; }

        public Vector2 Delta { get { return Position - LastPosition; } }

        public Vector2 Position = Vector2.Zero;
        public float Zoom = 1f;

        public Rectangle DeadZone;

        public enum FollowStyle
        {
            LockOn, Lerp, None
        }

        public FollowStyle FollowType = FollowStyle.None;
        public Vector2 FollowOffset = Vector2.Zero;

        public Body Target { get; private set; }

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(EntityGame.Viewport.Width * 0.5f, EntityGame.Viewport.Height * 0.5f, 0)); ;
            }
        }

        public bool IsActive
        {
            get { return EntityGame.Camera.Equals(this); }
        }

        public Rectangle ScreenSpace
        {
            get
            {
                var r = new Rectangle
                    {
                        X = (int)(Position.X - EntityGame.Viewport.Width / 2f),
                        Y = (int)(Position.Y - EntityGame.Viewport.Height / 2f),
                        Width = (int)(EntityGame.Viewport.Width * 1f/Zoom),
                        Height = (int)(EntityGame.Viewport.Height * 1f/Zoom)
                    };
                return r;
            }
        }

        public Camera(IComponent parent, string name)
            : base(parent, name)
        {
            Position = new Vector2(EntityGame.Viewport.Width / 2f, EntityGame.Viewport.Height / 2f);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            if (FollowType != FollowStyle.None && Target != null)
            {
                if (FollowType == FollowStyle.LockOn)
                    Position = Target.Position + FollowOffset;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            LastPosition = Position;
            base.Draw(sb);
        }

        public void View()
        {
            EntityGame.Camera = this;
        }

        public bool Intersects(Rectangle rect)
        {
            //TODO: Add OBB collision code here
            return ScreenSpace.Intersects(rect);
        }

        public void FollowPoint(Body b)
        {
            Target = b;
            FollowType = FollowStyle.LockOn;
        }

        public void Flash()
        {
        }
    }
}