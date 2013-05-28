using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Components.Render
{
    public class Animation : ImageRender
    {
        public Vector2 TileSize;
        public int FramesPerSecond;

        public int CurrentFrame { get; set; }

        public event Timer.TimerEvent LastFrameEvent;

        public Timer FrameTimer;

        public bool HitLastFrame
        {
            get { return (CurrentFrame >= Tiles - 1); }
        }

        public int Tiles
        {
            get { return Texture.Width / (int)TileSize.X; }
        }

        public int MillisecondsPerFrame
        {
            get { return 1000 / FramesPerSecond; }
        }

        public Rectangle CurrentFrameRect
        {
            get
            {
                return new Rectangle(
                    (int)(TileSize.X * CurrentFrame),
                    0,
                    (int)TileSize.X,
                    (int)TileSize.Y);
            }
        }

        public override Rectangle DrawRect
        {
            get
            {
                Vector2 position = Body.Position;
                return new Rectangle(
                    (int)(position.X + Origin.X * Scale.X),
                    (int)(position.Y + Origin.Y * Scale.Y),
                    (int)(TileSize.X * Scale.X),
                    (int)(TileSize.Y * Scale.Y));
            }
        }

        public Animation(Entity e, string name, Texture2D texture, Vector2 tileSize, int framesPerSecond, Body body)
            : base(e, name, texture, body)
        {
            TileSize = tileSize;
            FramesPerSecond = framesPerSecond;

            Origin = new Vector2(TileSize.X / 2.0f, TileSize.Y / 2.0f);

            FrameTimer = new Timer(e, Name + ".FrameTimer") { Milliseconds = MillisecondsPerFrame };
            FrameTimer.LastEvent += AdvanceNextFrame;
        }

        public Animation(Entity e, string name, Body body)
            : base(e, name, body)
        {
            FrameTimer = new Timer(e, Name + ".FrameTimer");
            FrameTimer.LastEvent += AdvanceNextFrame;
        }

        public override void Update(GameTime gt)
        {
            FrameTimer.Update(gt);
            if (HitLastFrame)
            {
                if (LastFrameEvent != null)
                    LastFrameEvent();
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, DrawRect, CurrentFrameRect, Color * Alpha, Body.Angle,
                    Origin, Flip, Layer);
        }

        public void AdvanceNextFrame()
        {
            CurrentFrame++;
            if (CurrentFrame >= Tiles)
                CurrentFrame = 0;
        }

        public void AdvanceLastFrame()
        {
            CurrentFrame--;
            if (CurrentFrame < 0)
                CurrentFrame = Tiles;
        }

        public void Start()
        {
            FrameTimer.Start();
        }

        public void Stop()
        {
            FrameTimer.Stop();
        }
    }
}