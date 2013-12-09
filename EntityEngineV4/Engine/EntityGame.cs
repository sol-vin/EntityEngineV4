using System;
using System.Collections.Generic;
using System.Diagnostics;
using EntityEngineV4.Data;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public class EntityGame : Game
    {
        /// <summary>
        /// The active camera
        /// </summary>
        public static Camera ActiveCamera;

        /// <summary>
        /// The active state
        /// </summary>
        public static State ActiveState;

        /// <summary>
        /// If the debug information should be injected into the active state
        /// </summary>
        public static bool ShowDebugInfo;

        private TimeSpan _frameCounterTimer = TimeSpan.Zero;
        private TimeSpan _cpuCounterTimer = TimeSpan.Zero;

        private int _frameCounter;

        public static int FrameRate { get; private set; }

        public static float RamUsage { get { return _ramCounter.NextValue(); } }

        public static float CpuUsage { get; private set; }

        private static List<float> _cpuUsages = new List<float>();

        //Find the cpu and ram usage
        private static Process _selfProcess = Process.GetCurrentProcess();

        private static PerformanceCounter _cpuCounter, _ramCounter;
        private DebugInfo _debugInfo;

        public static DebugInfo DebugInfo { get { return Self._debugInfo; } }

        public static Color BackgroundColor = Color.Silver;

        public static bool Paused { get; protected set; }

        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        public static GameTime GameTime { get; private set; }

        /// <summary>
        /// Used for logging operations
        /// </summary>
        public static Log Log { get; private set; }

        public static SpriteBatch SpriteBatch { get; private set; }

        public static Rectangle Viewport { get; private set; }

        /// <summary>
        /// LastID set through GetID
        /// </summary>
        public static int LastID { get; private set; }

        public delegate void GameEvent(Game game);

        public event GameEvent DestroyEvent;

        public static event State.EventHandler StateChanged;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EntityGame Self { get; private set; }

        public EntityGame(Rectangle viewPort)
        {
            //Game1 settings
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Viewport = viewPort;
            Exiting += (sender, args) => Exit();

            //Inject debug info into active state
            StateChanged += state => LastID = 1;
            StateChanged += state => _debugInfo = new DebugInfo(state, "DebugInfo") { Color = Color.Black };
            StateChanged += state => ActiveCamera = new Camera(state, "EntityEngineDefaultCamera");

            Log = new Log();

            //Start counters

            Process p = Process.GetCurrentProcess();
            _ramCounter = new PerformanceCounter("Process", "Working Set", p.ProcessName);
            _cpuCounter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);

            Self = this;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Assets.LoadConent(this);

            MakeWindow(Viewport);
        }

        public void Destroy(IComponent sender = null)
        {
            GameTime = null;
            ActiveCamera = null;
            Log.Dispose();

            if (DestroyEvent != null)
                DestroyEvent(this);
        }

        protected override void Update(GameTime gt)
        {
            GameTime = gt;
            ActiveCamera.Update(gt);

            if (!ActiveState.Destroyed)
            {
                ActiveState.PreUpdate();
                ActiveState.Update(gt);
                ActiveState.PostUpdate();
            }

            _cpuUsages.Add(_cpuCounter.NextValue());

            _frameCounterTimer += gt.ElapsedGameTime;
            _cpuCounterTimer += gt.ElapsedGameTime;

            if (_frameCounterTimer > TimeSpan.FromSeconds(1))
            {
                _frameCounterTimer -= TimeSpan.FromSeconds(1);
                FrameRate = _frameCounter;
                _frameCounter = 0;
            }

            if (_cpuCounterTimer > TimeSpan.FromSeconds(3))
            {
                _cpuCounterTimer -= TimeSpan.FromSeconds(5);
                _cpuUsages.Clear();
            }
            else
            {
                //average cpu usages
                float average = 0;
                foreach (var cpuUsage in _cpuUsages)
                {
                    average += cpuUsage;
                }
                CpuUsage = average / _cpuUsages.Count;
            }

            if (_debugInfo != null)
            {
                _debugInfo.Visible = ShowDebugInfo;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            GraphicsDevice.Clear(BackgroundColor);

            StartDrawing(ActiveCamera);
            ActiveCamera.Draw(SpriteBatch);
            ActiveState.Draw(SpriteBatch);

            StopDrawing();

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.None, RasterizerState.CullNone);
            SpriteBatch.End();
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

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Log.Write("[EntityGame] Beginning Exit", Alert.Info);
            ActiveState.Destroy();
            Log.Write("[EntityGame] Exited", Alert.Info);
        }

        /// <summary>
        /// Makes a game window of a specified size.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        public static void MakeWindow(Rectangle r)
        {
            if ((r.Width > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) ||
                (r.Height > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)) return;
            GraphicsDeviceManager.PreferredBackBufferWidth = r.Width;
            GraphicsDeviceManager.PreferredBackBufferHeight = r.Height;
            GraphicsDeviceManager.IsFullScreen = false;
            GraphicsDeviceManager.ApplyChanges();

            Log.Write("[EntityGame]Created window with params " + r, Alert.Info);
        }

        /// <summary>
        /// Gets an ID, usually called by Node but, anything can use this to get a state unique ID
        /// </summary>
        /// <returns></returns>
        public static int GetID()
        {
            return LastID++;
        }

        /// <summary>
        /// Switches the states
        /// </summary>
        /// <param name="state"></param>
        public static void SwitchState(State state)
        {
            if (ActiveState != null)
                ActiveState.Destroy();
            ActiveState = state;

            if (StateChanged != null)
                StateChanged(state);
        }
    }
}