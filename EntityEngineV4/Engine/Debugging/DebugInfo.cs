using System;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine.Debugging
{
    /// <summary>
    /// Node to display debugging info
    /// </summary>
    public class DebugInfo : Node
    {
        //TODO: Allow users to change position!
        private Body _body;
        private TextRender _render;

        public Color Color { get { return _render.Color; } set { _render.Color = value; } }
        public string Text { get { return _render.Text; } private set { _render.Text = value; } }
        public DebugInfo(Node parent, string name)
            : base(parent, name)
        {
            _body = new Body(this, "Body");
            _render = new TextRender(this, "Render", Assets.Font, "");
            _render.LinkDependency(TextRender.DEPENDENCY_BODY, _body);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            Text = "RAM: " + Math.Floor(EntityGame.RamUsage / 1024 / 1024) + Environment.NewLine;
            Text += "CPU: " + Math.Round(EntityGame.CpuUsage, 1) + @"%" + Environment.NewLine;
            Text += "FPS: " + EntityGame.FrameRate + Environment.NewLine;
            Text += "Active: " + GetRoot<State>().ActiveObjects + Environment.NewLine;
            Text += "Requests: " + GetRoot<State>().RequestsProcessed + Environment.NewLine;
            //Change this so the X value is static, create an enum that will decide
            //where to put the debug info
            _body.Position = new Vector2(EntityGame.Viewport.Width - 100,
                                                  EntityGame.Viewport.Height - _body.Bounds.Y - 10);
            _body.Bounds = _render.Bounds;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
