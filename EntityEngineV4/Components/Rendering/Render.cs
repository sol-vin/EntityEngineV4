using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering
{
    public abstract class Render : Component
    {
        public float Alpha = 1f;
        //TODO: Change tom RGBColor
        public Color Color = Color.White;
        public SpriteEffects Flip = SpriteEffects.None;
        public Vector2 Scale = Vector2.One;
        public Vector2 Offset = Vector2.Zero;

        //public Vector2 Origin;

        /// <summary>
        /// Handy rectangle for getting the drawing position
        /// </summary>
        public virtual Rectangle DrawRect { get; set; }

        /// <summary>
        /// Source rectangle of the texture if there is one
        /// </summary>
        public Rectangle SourceRect { get; set; }

        /// <summary>
        /// Bounds of the DrawRect
        /// </summary>
        public virtual Vector2 Bounds { get; set; }

        protected Render(Node parent, string name)
            : base(parent, name)
        {
            Layer = 0.5f;
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
                var r = new DrawingTools.Rectangle(DrawRect.X, DrawRect.Y, DrawRect.Width, DrawRect.Height);
                r.Color = Color.Red;
                r.Thickness = 1;
                r.Draw(sb);

                var origin = new DrawingTools.Point((int)(GetDependency<Body>(DEPENDENCY_BODY).Origin.X + DrawRect.X), (int)(GetDependency<Body>(DEPENDENCY_BODY).Origin.Y + DrawRect.Y));
                origin.Color = Color.Orange;
                origin.Draw(sb);
            }
        }

        public override void Reuse(Node parent, string name)
        {
            base.Reuse(parent, name);
            Alpha = 1f;
            Color = Color.White;
            Flip = SpriteEffects.None;
            Scale = Vector2.One;
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }
    }
}