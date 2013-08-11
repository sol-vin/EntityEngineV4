using System;
using EntityEngineV4.Data;
using EntityEngineV4.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class EntityGame : IComponent
    {
        public static Camera Camera;
        public static EntityState ActiveState;
        public static bool ShowFPS;
        private readonly Label _fpslabel;
        public Color BackgroundColor = Color.Silver;
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private int _frameCounter;

        public static bool Paused { get; protected set; }

        public static Game Game { get; private set; }

        public static GameTime GameTime { get; private set; }
        public static Log Log { get; private set; }

        public static SpriteBatch SpriteBatch { get; private set; }
        public static Rectangle Viewport { get; set; }
        public int FrameRate { get; private set; }

        public static uint LastID { get; private set; }
        public IComponent Parent { get; private set; }

        public event Component.EventHandler AddComponentEvent;
        public event Component.EventHandler RemoveComponentEvent;
        public event Entity.EventHandler AddEntityEvent;
        public event Entity.EventHandler RemoveEntityEvent;
        public event Service.EventHandler AddServiceEvent;
        public event Service.EventHandler RemoveServiceEvent;
        public event Service.ReturnHandler GetServiceEvent;

        public event EventHandler DestroyEvent;

        public string Name { get; private set; }
        public uint Id { get; private set; }
        public bool Active { get; private set; }
        public bool Visible { get; private set; }
        public bool Debug { get; set; }

        public static EntityGame Self { get; private set; }

        private EntityGame(Game game, SpriteBatch spriteBatch)
        {
            Game = game;
            Game.Exiting += (sender, args) => Exit();

            SpriteBatch = spriteBatch;
            Assets.LoadConent(game);

            _fpslabel = new Label(null, "FPSLabel");
            _fpslabel.Visible = false;

            Log = new Log();
        }

        private EntityGame(Game game, GraphicsDeviceManager g, SpriteBatch spriteBatch, Rectangle viewport)
        {
            Game = game;
            Game.Exiting += (sender, args) => Exit();

            SpriteBatch = spriteBatch;
            Viewport = viewport;
            Assets.LoadConent(game);

            _fpslabel = new Label(null, "FPSLabel");
            _fpslabel.Visible = false;

            Camera = new Camera(null, "EntityEngineDefaultCamera");

            Log = new Log();

            MakeWindow(g, viewport);
        }

        public static void MakeGame(Game game, GraphicsDeviceManager g, SpriteBatch spriteBatch, Rectangle viewport)
        {
            Self = new EntityGame(game, g, spriteBatch, viewport);
        }

        public static void MakeGame(Game game, SpriteBatch spriteBatch)
        {
            Self = new EntityGame(game, spriteBatch);
        }

        public void Destroy(IComponent i = null)
        {
            Game = null;
            GameTime = null;
            Camera = null;
            Log.Dispose();

            if (DestroyEvent != null)
                DestroyEvent(this);
        }


        public virtual void Update(GameTime gt)
        {
            GameTime = gt;
            Camera.Update();
            ActiveState.Update(gt);

            _elapsedTime += gt.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                FrameRate = _frameCounter;
                _frameCounter = 0;
            }

            _fpslabel.Visible = ShowFPS;
            if (ShowFPS)
            {
                _fpslabel.Update(gt);
                _fpslabel.Text = FrameRate.ToString();
                _fpslabel.Body.Position = new Vector2(Viewport.Width - _fpslabel.Body.Bounds.X - 10,
                                                      Viewport.Height - _fpslabel.Body.Bounds.Y - 10);
            }
        }

        public virtual void Draw(SpriteBatch sb = null)
        {
            _frameCounter++;

            Game.GraphicsDevice.Clear(BackgroundColor);
            Game.GraphicsDevice.Clear(BackgroundColor);

            StartDrawing();
            ActiveState.Draw(SpriteBatch);

            StopDrawing();

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.None, RasterizerState.CullNone);

            if (_fpslabel.Visible)
                _fpslabel.Draw(SpriteBatch);
            SpriteBatch.End();
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
            ActiveState.AddEntity(c);
            if (AddEntityEvent != null)
            {
                AddEntityEvent(c);
            }
        }

        public void RemoveEntity(Entity c)
        {
            ActiveState.RemoveEntity(c);
            if (RemoveEntityEvent != null)
            {
                RemoveEntityEvent(c);
            }
        }


        public void AddService(Service s)
        {
            if (AddServiceEvent != null)
            {
                AddServiceEvent(s);
            }
            ActiveState.AddService(s);
        }

        public void RemoveService(Service s)
        {
            if (RemoveServiceEvent != null)
            {
                RemoveServiceEvent(s);
            }
            ActiveState.RemoveService(s);
        }

        public T GetService<T>() where T : Service
        {
            return ActiveState.GetService<T>();
        }
        public Service GetService(Type t)
        {
            return ActiveState.GetService(t);
        }

        public static void StartDrawing(Camera camera)
        {
            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.None, RasterizerState.CullNone, null, camera.Transform);
        }

        public static void StartDrawing()
        {
            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.None, RasterizerState.CullNone, null);
        }

        public static void StopDrawing()
        {
            SpriteBatch.End();
        }

        public static void Exit()
        {
            Log.Write("Beginning Exit", Self, Alert.Info);
            ActiveState.Destroy();
            Game.Exit();
            Log.Write("Exited", Self, Alert.Info);
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

        public static void SwitchState(EntityState state)
        {
            ActiveState = state;
            ActiveState.Show();
        }
    }
}