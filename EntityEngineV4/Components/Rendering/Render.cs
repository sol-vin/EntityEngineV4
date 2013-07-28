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

        public virtual Rectangle DrawRect { get; set; }

        public virtual Rectangle SourceRect { get; set; }

        protected Render(Entity entity, string name)
            : base(entity, name)
        {
        }
    }
}