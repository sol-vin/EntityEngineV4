using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components
{
    public class Body : Component
    {
        public float Angle;
        public Vector2 LastPosition { get; private set; }
        public Vector2 Position;
        public Vector2 Bounds;

        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Bounds.X, (int)Bounds.Y);
            }
            set
            {
                Position = new Vector2(value.X, value.Y);
                Bounds = new Vector2(value.Width, value.Height);
            }
        }
        public Vector2 Delta { get { return Position - LastPosition; } }

        public float Top { get { return Position.Y; } }

        public float Left { get { return Position.X; } }

        public float Right { get { return Position.X + Bounds.X; } }

        public float Bottom { get { return Position.Y + Bounds.Y; } }

        public float X 
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public float Width
        {
            get { return Bounds.X; }
            set { Bounds.X = value; }
        }

        public float Height
        {
            get { return Bounds.Y; }
            set { Bounds.Y = value; }
        }


        public Color DebugColor = Color.Yellow;

        public Body(IComponent e, string name)
            : base(e, name)
        {
        }

        public Body(IComponent e, string name, Vector2 position)
            : base(e, name)
        {
            Position = position;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            LastPosition = Position;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if(Debug)
            {
//Draw our debug bounds.
                Rectangle drawwindow;
                //Draw top
                drawwindow = new Rectangle(BoundingRect.X, BoundingRect.Y, BoundingRect.Width, 1);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw bottom
                drawwindow = new Rectangle(BoundingRect.X, BoundingRect.Bottom, BoundingRect.Width, 1);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw left
                drawwindow = new Rectangle(BoundingRect.X, BoundingRect.Y, 1, BoundingRect.Height);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw right
                drawwindow = new Rectangle(BoundingRect.Right, BoundingRect.Y, 1, BoundingRect.Height);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);
            }
        }

        public Body Clone()
        {
            var b = new Body(Parent, Name);
            b.Position = Position;
            b.Angle = Angle;
            return b;
        }
    }
}