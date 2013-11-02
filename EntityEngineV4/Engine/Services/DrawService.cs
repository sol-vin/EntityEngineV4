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
        private List<Node> _drawables = new List<Node>();

        public DrawService(State state, string name) : base(state, name)
        {
            state.NodeAdded += AddDraw;
            state.NodeRemoved += RemoveDraw;
        }

        private void RemoveDraw(Node node)
        {
        }

        private void AddDraw(Node node)
        {
            if (_drawables.Count == 0)
            {
                _drawables.Add(node);
                return;
            }

            for (int i = 0; i < _drawables.Count; i++)
            {
                if (_drawables[i].Layer >= node.Layer) _drawables.Insert(i+1, node);
                else if (_drawables[i].Layer < node.Layer) _drawables.Insert(i, node);

            }
        }

        public void Initialize()
        {
            
        }

        public override void Update(GameTime gt)
        {
        }

        public override void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < _drawables.Count; i++)
            {
                _drawables[i].Draw(sb);
            }
        }
    }
}
