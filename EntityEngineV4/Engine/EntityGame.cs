using System;
using EntityEngineV4.Data;
using EntityEngineV4.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class EntityGame
    {
        public static bool Paused { get; protected set; }

        public static Game Game { get; private set; }

        public static GameTime GameTime { get; private set; }

        public SpriteBatch SpriteBatch { get; private set; }

        public EntityState CurrentState;

        public static Rectangle Viewport { get; set; }

        public Color BackgroundColor = Color.Silver;

        public int FrameRate { get; private set; }
        private int _frameCounter = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private Label _fpslabel;

        public bool ShowFPS
        {
            get { return _fpslabel.Visible; }
            set { _fpslabel.Visible = value; }
        }


        /// <summary>
        /// The time in between each physics update
        /// </summary>
        private static int _timebetweenupdate;

        public EntityGame(Game game, SpriteBatch spriteBatch)
        {
            Game = game;
            SpriteBatch = spriteBatch;
            Assets.LoadConent(game);

            _fpslabel = new Label(null,"FPSLabel");
            _fpslabel.Visible = false;
        }

        public EntityGame(Game game, GraphicsDeviceManager g, SpriteBatch spriteBatch, Rectangle viewport)
        {
            Game = game;
            SpriteBatch = spriteBatch;
            Viewport = viewport;
            Assets.LoadConent(game);

            _fpslabel = new Label(new EntityState(this, "FakeState"), "FPSLabel");
            _fpslabel.Visible = false;

            MakeWindow(g, viewport);
        }

        public virtual void Update(GameTime gt)
        {
            GameTime = gt;
            CurrentState.Update(gt);
            
            _elapsedTime += gt.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                FrameRate = _frameCounter;
                _frameCounter = 0;
            }

            _fpslabel.Update(gt);
            _fpslabel.Text = FrameRate.ToString();
            _fpslabel.Body.Position = new Vector2(Viewport.Width - _fpslabel.Body.Bounds.X - 10, Viewport.Height - _fpslabel.Body.Bounds.Y - 10);
        }

        public virtual void Draw()
        {
            _frameCounter++;

            Game.GraphicsDevice.Clear(BackgroundColor); Game.GraphicsDevice.Clear(BackgroundColor);
            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.None, RasterizerState.CullNone);
            if(_fpslabel.Visible)
                _fpslabel.Draw(SpriteBatch);
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