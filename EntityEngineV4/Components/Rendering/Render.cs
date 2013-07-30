using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Rendering
{
    public abstract class Render : Component
    {
        public float Alpha = 1f;
        public Color Color = Color.White;
        public SpriteEffects Flip = SpriteEffects.None;
        public float Layer;
        public Vector2 Scale = Vector2.One;

        public Vector2 Origin;

        /// <summary>
        /// Handy rectangle for getting the drawing position
        /// </summary>
        public virtual Rectangle DrawRect { get; set; }

        /// <summary>
        /// Source rectangle of the texture
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
    }
}