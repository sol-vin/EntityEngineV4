using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4.Input.MouseInput
{
    public abstract class Cursor : Entity
    {
        //Hidden because we don't want people messing with it all willy nilly
        protected Body Body;

        public Render Render;

        public Vector2 Position
        {
            get
            {
                return Body.Position;
            }
            set { Body.Position = value; }
        }

        public delegate void CursorEventHandler(Cursor c);

        public event CursorEventHandler GotFocus , LostFocus;

        public bool HasFocus { get; protected set; }

        public abstract bool Down();
        public abstract bool Pressed();
        public abstract bool Released();
        public abstract bool Up();


        public Cursor(IComponent parent, string name)
            : base(parent, name)
        {
            Active = true;
            Visible = false;

            Body = new Body(this, "Body");
            Body.Position = new Vector2(400, 300);

            //Default rendering is a single white pixel.
            Render = new ImageRender(this, "ImageRender", Assets.Pixel, Body);
            Render.Layer = 1f;
            Render.Scale = Vector2.One * 3f;
            Render.Color = Color.Black;

            Body.Bounds = Render.Scale;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public void OnGetFocus(Cursor c = null)
        {
            if (GotFocus != null) GotFocus(this);

            //Get the Cursor's position and set it to our own for seamless changing.
            if(MouseHandler.Cursor != null)
                Position = MouseHandler.Cursor.Position;
            MouseHandler.Cursor = this;
            HasFocus = true;

            Visible = true;

            EntityGame.Log.Write("Got focus!", this, Alert.Info);
        }

        public void OnLostFocus(Cursor newCursor)
        {
            if (LostFocus != null) LostFocus(newCursor);
            HasFocus = false;

            Visible = false;
        }

        public override void Destroy(IComponent i = null)
        {
            base.Destroy(i);

            GotFocus = null;
            LostFocus = null;
            if (HasFocus)
            {
                HasFocus = false;
                MouseHandler.Cursor = null;
            }
        }
    }

    public class ControllerCursor : Cursor
    {
        public GamePadInput SelectKey;
        public GamePadAnalog AnalogStick;

        public GamePadInput UpKey, DownKey, LeftKey, RightKey;
        public Vector2 MovementSpeed = new Vector2(3,3);
        
        public enum MovementInput { Analog, Buttons}

        /// <summary>
        /// What the controlling input should be.
        /// If the Input is Analog, it will use the AnalogStick GamePadInput;
        /// If the Input is Buttons, it will use the UpKey,DownKey, etc, etc, to define it's keys.
        /// </summary>
        public MovementInput Input;

        /// <summary>
        /// Value for controlling if the ControllerCursor should switch if the Inputs if the Analog or Buttons are pressed
        /// </summary>
        public bool AutoSwitchInputs = false;

        /// <summary>
        /// Creates a new ControllerCursor with default values
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public ControllerCursor(IComponent parent, string name)
            : base(parent, name)
        {
            MakeDefault();
        }
        public ControllerCursor(IComponent parent, string name, MovementInput input)
            : base(parent, name)
        {
            switch(input)
            {
                case MovementInput.Analog:
                    break;
                case MovementInput.Buttons:
                    break;
            }

            MakeDefault();
        }

        public override void Update(GameTime gt)
        {
            if (AutoSwitchInputs) //Switch the inputs
                if(Input != MovementInput.Analog && AnalogStickMoved())
                {
                    Input = MovementInput.Analog;
                    EntityGame.Log.Write("Input switched to Analog", this, Alert.Info);
                }
                else if(Input != MovementInput.Buttons && ButtonPressed())
                {
                    Input = MovementInput.Buttons;
                    EntityGame.Log.Write("Input switched to Buttons", this, Alert.Info);
                }
            
            

            switch (Input)
            {
                case MovementInput.Analog:
                    if (!HasFocus && AnalogStickMoved())
                        OnGetFocus(this);
                    if (HasFocus)
                    {
                        Position = new Vector2(Position.X + AnalogStick.Position.X*MovementSpeed.X,
                                               Position.Y - AnalogStick.Position.Y*MovementSpeed.Y);

                        //Move it with the camera.
                        Position += EntityGame.Camera.Delta;

                        //Keep it from leaving the bounds of the window.
                        if (Body.Position.X < EntityGame.Camera.ScreenSpace.Left)
                            Body.Position.X = EntityGame.Camera.ScreenSpace.Left;
                        else if (Body.BoundingRect.Right > EntityGame.Camera.ScreenSpace.Right)
                            Body.Position.X = EntityGame.Camera.ScreenSpace.Right - Body.Bounds.X;

                        if (Body.Position.Y < EntityGame.Camera.ScreenSpace.Top)
                            Body.Position.Y = EntityGame.Camera.ScreenSpace.Top;
                        else if (Body.BoundingRect.Bottom > EntityGame.Camera.ScreenSpace.Bottom)
                            Body.Position.Y = EntityGame.Camera.ScreenSpace.Bottom - Body.Bounds.Y;
                    }
                    break;
                case MovementInput.Buttons:
                    if (!HasFocus && ButtonPressed())
                        OnGetFocus(this);
                    if (HasFocus)
                    {
                        Position = new Vector2(
                            Position.X + ((LeftKey.Down()) ? -MovementSpeed.X : 0) + ((RightKey.Down()) ? MovementSpeed.X : 0),
                            Position.Y + ((UpKey.Down()) ? -MovementSpeed.Y : 0) + ((DownKey.Down()) ? MovementSpeed.Y : 0)
                            );

                        //Move it with the camera.
                        Position += EntityGame.Camera.Delta;

                        //Keep it from leaving the bounds of the window.
                        if (Body.Position.X < EntityGame.Camera.ScreenSpace.Left)
                            Body.Position.X = EntityGame.Camera.ScreenSpace.Left;
                        else if (Body.BoundingRect.Right > EntityGame.Camera.ScreenSpace.Right)
                            Body.Position.X = EntityGame.Camera.ScreenSpace.Right - Body.Bounds.X;

                        if (Body.Position.Y < EntityGame.Camera.ScreenSpace.Top)
                            Body.Position.Y = EntityGame.Camera.ScreenSpace.Top;
                        else if (Body.BoundingRect.Bottom > EntityGame.Camera.ScreenSpace.Bottom)
                            Body.Position.Y = EntityGame.Camera.ScreenSpace.Bottom - Body.Bounds.Y;
                    }
                    break;
            }

            base.Update(gt);
        }

        public override bool Down()
        {
            return SelectKey.Down();
        }

        public override bool Pressed()
        {
            EntityGame.Log.Write("Button pressed", this, Alert.Info);
            return SelectKey.Pressed();
        }

        public override bool Released()
        {
            EntityGame.Log.Write("Button released", this, Alert.Info);
            return SelectKey.Released();
        }

        public override bool Up()
        {
            return SelectKey.Up();
        }

        public bool ButtonPressed()
        {
            return (UpKey.Down() || DownKey.Down() || LeftKey.Down() || RightKey.Down());
        }

        public bool AnalogStickMoved()
        {
            return AnalogStick.Position != Vector2.Zero;
        }

        //Default settings
        public void MakeDefault()
        {
            SelectKey = new GamePadInput(this, "SelectKey", Buttons.A, PlayerIndex.One);
            AnalogStick = new GamePadAnalog(this, "AnalogStick", Sticks.Right, PlayerIndex.One);
            UpKey = new GamePadInput(this, "UpKey", Buttons.DPadUp, PlayerIndex.One);
            DownKey = new GamePadInput(this, "DownKey", Buttons.DPadDown, PlayerIndex.One);
            LeftKey = new GamePadInput(this, "LeftKey", Buttons.DPadLeft, PlayerIndex.One);
            RightKey = new GamePadInput(this, "RightKey", Buttons.DPadRight, PlayerIndex.One);
            Input = MovementInput.Analog;
        }
    }

    public class MouseCursor : Cursor
    {
        public MouseCursor(IComponent parent, string name) : base(parent, name)
        {

        }

        public override void Update(GameTime gt)
        {
            if(!HasFocus && MouseHandler.Delta != Point.Zero)
                OnGetFocus(this);
            if(HasFocus)
            {
                Position = new Vector2(Position.X - MouseHandler.Delta.X, Position.Y - MouseHandler.Delta.Y);

                //Move it with the camera.
                Position += EntityGame.Camera.Delta;

                //Keep it from leaving the bounds of the window.
                if (Body.Position.X < EntityGame.Camera.ScreenSpace.Left) Body.Position.X = EntityGame.Camera.ScreenSpace.Left;
                else if (Body.BoundingRect.Right > EntityGame.Camera.ScreenSpace.Right)
                    Body.Position.X = EntityGame.Camera.ScreenSpace.Right - Body.Bounds.X;

                if (Body.Position.Y < EntityGame.Camera.ScreenSpace.Top) Body.Position.Y = EntityGame.Camera.ScreenSpace.Top;
                else if (Body.BoundingRect.Bottom > EntityGame.Camera.ScreenSpace.Bottom)
                    Body.Position.Y = EntityGame.Camera.ScreenSpace.Bottom - Body.Bounds.Y;
            }

            base.Update(gt);
        }

        public override bool Down()
        {
            return MouseHandler.IsMouseButtonDown(MouseButton.LeftButton);
        }

        public override bool Pressed()
        {
            return MouseHandler.IsMouseButtonPressed(MouseButton.LeftButton);
        }

        public override bool Released()
        {
            return MouseHandler.IsMouseButtonReleased(MouseButton.LeftButton);
        }

        public override bool Up()
        {
            return MouseHandler.IsMouseButtonUp(MouseButton.LeftButton);
        }

        public override void Draw(SpriteBatch sb)
        {            
            base.Draw(sb);
        }
    }
}