using System;
using System.Collections.Generic;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.GUI
{
	public class GUIHandler : Service
	{
		private List<Control> _controls;

		public GUIHandler(EntityState stateref) : base(stateref)
		{
			_controls = new List<Control>();
		}

		public override void Update(GameTime gt)
		{
			
		}

		public override void Draw(SpriteBatch sb)
		{
			
		}
	}
}

