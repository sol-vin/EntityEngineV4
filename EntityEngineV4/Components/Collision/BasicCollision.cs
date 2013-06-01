using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Collision
{
    public class BasicCollision : Component
    {
        public List<Entity> Partners = new List<Entity>();
        public List<Entity> CollidedWith = new List<Entity>();
        public bool Debug;
        public Color DebugColor = Color.PowderBlue;

        public bool Colliding
        {
            get { return (CollidedWith.Count > 0); }
        }

        public Rectangle BoundingRect { get { return _body.BoundingRect; } }

        public event Entity.EventHandler CollideEvent;

        //Dependencies
        private Body _body;

        public BasicCollision(Entity e, string name, Body Body)
            : base(e, name)
        {
            _body = Body;
        }

        public override void Update(GameTime gt)
        {
            //Erase the collided with list every frame
            CollidedWith.Clear();
            foreach (var p in Partners.ToArray().Where(TestCollision))
            {
                CollidedWith.Add(p);
                if (CollideEvent != null)
                    CollideEvent(p);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (Debug)
            {
                Rectangle drawwindow;
                //Draw top
                drawwindow = new Rectangle(_body.BoundingRect.X, _body.BoundingRect.Y, _body.BoundingRect.Width, 1);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw bottom
                drawwindow = new Rectangle(_body.BoundingRect.X, _body.BoundingRect.Bottom, _body.BoundingRect.Width, 1);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw left
                drawwindow = new Rectangle(_body.BoundingRect.X, _body.BoundingRect.Y, 1, _body.BoundingRect.Height);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);

                //Draw right
                drawwindow = new Rectangle(_body.BoundingRect.Right, _body.BoundingRect.Y, 1, _body.BoundingRect.Height);
                sb.Draw(Assets.Pixel, drawwindow, null, DebugColor, 0, Vector2.Zero, SpriteEffects.None, 1f);
            }
        }

        public override void Destroy(IComponent i = null)
        {
            Partners = new List<Entity>();
        }

        virtual public bool TestCollision(Entity e)
        {
            return e.Name != Name && 
                (_body.BoundingRect.Intersects(e.GetComponent<BasicCollision>().BoundingRect));
        }

        public void AddPartner(Entity e)
        {
            Partners.Add(e);
            e.DestroyEvent += RemovePartner;
        }

        public void RemovePartner(Entity e)
        {
            Partners.Remove(e);
        }
    }
}
