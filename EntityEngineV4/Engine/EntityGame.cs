using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Input;
using EntityEngineV4.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
	public class EntityGame
	{
		public bool Paused { get; protected set; }

		public static Game Game { get; private set; }

		public static GameTime GameTime{ get; private set; }

		public SpriteBatch SpriteBatch { get; private set; }

		public EntityState CurrentState;

		public static  Rectangle Viewport { get; set; }

		
		public  Color BackgroundColor = Color.Silver;
		
		/// <summary>
		/// The time in between each physics update
		/// </summary>
		private static int _timebetweenupdate;

		public EntityGame (Game game, SpriteBatch spriteBatch)
		{
			Game = game;
			SpriteBatch = spriteBatch;
			Assets.LoadConent (game);
		}
		   

		public EntityGame(Game game, GraphicsDeviceManager g, SpriteBatch spriteBatch, Rectangle viewport)
		{
			Game = game;
			SpriteBatch = spriteBatch;
			Viewport = viewport;
			Assets.LoadConent(game);

			MakeWindow(g, viewport);
		}

		public virtual void Update (GameTime gt)
		{
			GameTime = gt;
			CurrentState.Update (gt);
			
		}

		public virtual void Draw()
		{
			Game.GraphicsDevice.Clear(BackgroundColor); Game.GraphicsDevice.Clear(BackgroundColor);
			SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp,
							  DepthStencilState.None, RasterizerState.CullNone);

			CurrentState.Draw(SpriteBatch);

			SpriteBatch.End();
		}

		public virtual void Exit()
		{
			CurrentState.Destroy();
		}

		public static void MakeWindow(GraphicsDeviceManager g, Rectangle r)
		{
			if ((r.Width > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) ||
				(r.Height > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)) return;
			g.PreferredBackBufferWidth = r.Width;
			g.PreferredBackBufferHeight = r.Height;
			g.IsFullScreen = false;
			g.ApplyChanges();
		}
	}
}