using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering
{
    public abstract class Render : Component
    {
        public float Alpha = 1f;
        public Color Color = Color.White;
        public SpriteEffects Flip = SpriteEffects.None;
        public float Layer = .5f;
        public Vector2 Scale = Vector2.One;

        public Vector2 Origin;

        /// <summary>
        /// Handy rectangle for getting the drawing position
        /// </summary>
        public virtual Rectangle DrawRect { get; set; }

        /// <summary>
        /// Source rectangle of the texture if there is one
        /// </summary>
        public virtual Rectangle SourceRect { get; set; }

        /// <summary>
        /// Bounds of the DrawRect
        /// </summary>
        public virtual Vector2 Bounds { get; set; }

        protected Render(IComponent parent, string name)
            : base(parent, name)
        {
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb = null)
        {
            base.Draw(sb);

            if (Debug)
            {
                //Draw a bounding rect around it's drawrect
                DrawingTools.Rectangle r = new DrawingTools.Rectangle(DrawRect.X, DrawRect.Y, DrawRect.Width, DrawRect.Height);
                r.Color = Color.Red;
                r.Thickness = 1;
                r.Draw(sb);

                DrawingTools.Point origin = new DrawingTools.Point((int)(Origin.X + DrawRect.X), (int)(Origin.Y + DrawRect.Y));
                origin.Color = Color.Orange;
                origin.Draw(sb);
            }
        }
    }
}