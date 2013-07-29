using System;
using EntityEngineV4.Data;
using EntityEngineV4.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class EntityGame : IComponent
    {
        public IComponent Parent { get; private set; }
        public event Component.EventHandler AddComponentEvent;
        public event Component.EventHandler RemoveComponentEvent;
        public event Entity.EventHandler AddEntityEvent;
        public event Entity.EventHandler RemoveEntityEvent;

        public string Name { get; private set; }
        public uint Id { get; private set; }
        public bool Active { get; private set; }
        public bool Visible { get; private set; }
        public bool Debug { get; set; }

        public void Destroy(IComponent i = null)
        {
            Game = null;
            GameTime = null;
            CurrentCamera = null;
            Log.Dispose();
        }

        public static bool Paused { get; protected set; }

        public static Game Game { get; private set; }

        public static GameTime GameTime { get; private set; }

        public static Camera CurrentCamera;

        public static Log Log { get; private set; }

        public SpriteBatch SpriteBatch { get; private set; }

        public static EntityState CurrentState;

        public static Rectangle Viewport { get; set; }

        public Color BackgroundColor = Color.Silver;

        public int FrameRate { get; private set; }

        public static uint LastID { get; private set; }

        private int _frameCounter = 0;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private Label _fpslabel;

        public static bool ShowFPS;

        public EntityGame(Game game, SpriteBatch spriteBatch)
        {
            Game = game;
            SpriteBatch = spriteBatch;
            Assets.LoadConent(game);

            _fpslabel = new Label(null, "FPSLabel");
            _fpslabel.Visible = false;

            Log = new Log();
        }

        public EntityGame(Game game, GraphicsDeviceManager g, SpriteBatch spriteBatch, Rectangle viewport)
        {
            Game = game;
            SpriteBatch = spriteBatch;
            Viewport = viewport;
            Assets.LoadConent(game);

            _fpslabel = new Label(null, "FPSLabel");
            _fpslabel.Visible = false;

            CurrentCamera = new Camera(this, "DefaultCamera");

            Log = new Log();

            MakeWindow(g, viewport);
        }

        public virtual void Update(GameTime gt)
        {
            GameTime = gt;
            CurrentCamera.Update();
            CurrentState.Update(gt);

            _elapsedTime += gt.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                FrameRate = _frameCounter;
                _frameCounter = 0;
            }

            _fpslabel.Visible = ShowFPS;
            _fpslabel.Update(gt);
            _fpslabel.Text = FrameRate.ToString();
            _fpslabel.Body.Position = new Vector2(Viewport.Width - _fpslabel.Body.Bounds.X - 10, Viewport.Height - _fpslabel.Body.Bounds.Y - 10);
        }

        public virtual void Draw(SpriteBatch sb = null)
        {
            _frameCounter++;

            Game.GraphicsDevice.Clear(BackgroundColor); Game.GraphicsDevice.Clear(BackgroundColor);
            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp,
                           DepthStencilState.None, RasterizerState.CullNone, null, CurrentCamera.Transform); 

            CurrentState.Draw(SpriteBatch);

            SpriteBatch.End();

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp,
                            DepthStencilState.None, RasterizerState.CullNone);

            if (_fpslabel.Visible)
                _fpslabel.Draw(SpriteBatch);
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

            Log.Write("Created window with params " + r.ToString(), Alert.Info);
        }

        public static uint GetID()
        {
            return LastID++;
        }

        public void AddComponent(Component c)
        {
            if (AddComponentEvent != null)
            {
                AddComponentEvent(c);
            }
            else
            {
                Log.Write("AddComponent called with no methods subscribed", this, Alert.Warning);
            }
        }

        public void RemoveComponent(Component c)
        {
            if (RemoveComponentEvent != null)
            {
                RemoveComponentEvent(c);
            }
            else
            {
                Log.Write("RemoveComponent called with no methods subscribed", this, Alert.Warning);
            }
        }

        public void AddEntity(Entity c)
        {
            CurrentState.AddEntity(c);
            if (AddEntityEvent != null)
            {
                AddEntityEvent(c);
            }
        }

        public void RemoveEntity(Entity c)
        {
            CurrentState.RemoveEntity(c);
            if (RemoveEntityEvent != null)
            {
                RemoveEntityEvent(c);
            }
        }
    }
}