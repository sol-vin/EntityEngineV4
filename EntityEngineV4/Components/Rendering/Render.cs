using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EntityEngineV4.Engine;

namespace EntityEngineV4.Components.Rendering
{
 public class Render : Component
    {
        public float Alpha = 1f;
        public Color Color = Color.White;
        public SpriteEffects Flip = SpriteEffects.None;
        public float Layer;
        public Vector2 Scale = Vector2.One;
		
		public Vector2 Origin;
        public virtual Rectangle DrawRect { get; set; }
        public virtual Rectangle SourceRect { get; set; }

        public Render(Entity entity, string name)
            : base(entity, name)
        {
        }

        public virtual Render Clone()
        {
            var r = new Render(Parent, Name);
            r.Color = Color;
            r.Alpha = Alpha;
            r.Scale = Scale;
            r.Layer = Layer;
            r.Flip = Flip;
            r.Origin = Origin;
            r.DrawRect = DrawRect;
            return r;
        }
    }
}

