using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine.Debugging
{
    /// <summary>
    /// Node to display debugging info
    /// </summary>
    public class DebugInfo : Label 
    {
        //TODO: Allow users to change position!

        public DebugInfo(Node parent, string name) : base(parent, name)
        {
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            Text = "RAM: " + Math.Floor(EntityGame.RamUsage / 1024 / 1024) + Environment.NewLine;
            Text += "CPU: " + Math.Round(EntityGame.CpuUsage, 1) + @"%" + Environment.NewLine;
            Text += "FPS: " + EntityGame.FrameRate + Environment.NewLine;
            Text += "Active: " + GetState().ActiveNodes + Environment.NewLine;
            Text += "Requests: " + GetState().RequestsProcessed + Environment.NewLine;
            //Change this so the X value is static, create an enum that will decide
            //where to put the debug info
            Body.Position = new Vector2(EntityGame.Viewport.Width - 100,
                                                  EntityGame.Viewport.Height - Body.Bounds.Y - 10);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
