using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine.Services
{
    public class DrawService : Service
    {
        private SortedDictionary<float, Component> _drawables = new SortedDictionary<float, Component>();

        public DrawService(EntityState stateRef, string name) : base(stateRef, name)
        {
        }

        public void Initialize()
        {
            
        }

        public override void Update(GameTime gt)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
        }
    }
}
