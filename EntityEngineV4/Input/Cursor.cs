using System.Collections.Generic;
using EntityEngineV4.CollisionEngine;
using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Debugging;
using EntityEngineV4.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4.Input
{
    public abstract class Cursor : Node
    {
        public const int MouseCollisionGroup = 31; //Max group number in bit mask.
    
        //Hidden because we don't want people messing with it all willy nilly
        protected Body Body;

        public Render Render;

        public Collision Collision;
        private AABB _boundingBox;
        private Body _collisionBody;

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

        public bool HasFocus
        {
            get; protected set;
        }

        public abstract bool Down();
        public abstract bool Pressed();
        public abstract bool Released();
        public abstract bool Up();


        public Cursor(Node parent, string name)
            : base(parent, name)
        {
            Active = true;
            Visible = false;

            Body = new Body(this, "Body");
            Body.Position = new Vector2(400, 300);

            //Default rendering is a single white pixel.
            Render = new ImageRender(this, "ImageRender", Assets.Pixel);
            Render.LinkDependency(ImageRender.DEPENDENCY_BODY, Body);
            Render.Layer = 1f;
            Render.Scale = Vector2.One * 3f;
            Render.Color = Color.Black;

            Body.Bounds = Render.Scale;

            _collisionBody = new Body(this, "CollisionBody");
            _collisionBody.Bounds = Vector2.One;

            _boundingBox = new AABB(this, "AABB");
            _boundingBox.LinkDependency(AABB.DEPENDENCY_BODY, _collisionBody);

            Collision = new Collision(this, "Collision");
            Collision.Group.AddMask(MouseCollisionGroup);
            Collision.Pair.AddMask(MouseCollisionGroup);
            Collision.LinkDependency(Collision.DEPENDENCY_SHAPE, _boundingBox);
            _boundingBox.LinkDependency(AABB.DEPENDENCY_COLLISION, Collision);

        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            _collisionBody.Position = Body.Position;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public void GetFocus(Cursor c)
        {
            if (GotFocus != null) GotFocus(this);

            //Get the Cursor's position and set it to our own for seamless changing.
            if(MouseService.Cursor != null)
            {
                Position = MouseService.Cursor.Position;
                MouseService.Cursor.LoseFocus();
            }

            MouseService.Cursor = this;

            HasFocus = true;

            Visible = true;

            EntityGame.Log.Write("Got focus!", this, Alert.Info);
        }

        public void LoseFocus()
        {
            if (LostFocus != null) LostFocus(this);
            HasFocus = false;

            Visible = false;

            EntityGame.Log.Write("Lost focus!", this, Alert.Info);
        }

        public override void Destroy(IComponent sender = null)
        {
            base.Destroy(sender);

            GotFocus = null;
            LostFocus = null;
            if (HasFocus)
            {
                HasFocus = false;
                MouseService.Cursor = null;
            }
        }
    }

    public class ControllerCursor : Cursor
    {
        public GamepadInput SelectKey;
        public GamePadAnalog AnalogStick;

        public GamepadInput UpKey, DownKey, LeftKey, RightKey;
        public Vector2 MovementSpeed = new Vector2(3,3);
        
        public enum MovementInput { Analog, Buttons}

        /// <summary>
        /// What the controlling input should be.
        /// If the Input is Analog, it will use the AnalogStick GamepadInput;
        /// If the Input is Buttons, it will use the UpKey,DownKey, etc, etc, to define it's keys.
        /// </summary>
        public MovementInput Input;

        /// <summary>
        /// Value for controlling if the ControllerCursor should switch if the Inputs if the Analog or Buttons are pressed
        /// </summary>
        public bool AutoSwitchInputs;

        /// <summary>
        /// Creates a new ControllerCursor with default values
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public ControllerCursor(MouseService parent, string name)
            : base(parent, name)
        {
            Input = MovementInput.Analog;
            MakeDefault();
        }
        public ControllerCursor(Node parent, string name, MovementInput input)
            : base(parent, name)
        {
            Input = input;

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
                    if (!HasFocus && (AnalogStickMoved() || SelectKey.Down()))
                        GetFocus(this);
                    if (HasFocus)
                    {
                        //TODO: Use normalized positition for this.
                        Position = new Vector2(Position.X + AnalogStick.Position.X*MovementSpeed.X,
                                               Position.Y - AnalogStick.Position.Y*MovementSpeed.Y);

                        //Move it with the camera.
                        //Position += EntityGame.Camera.Delta;

                        //Keep it from leaving the bounds of the window.
                        if (Body.Position.X < EntityGame.ActiveCamera.ScreenSpace.Left)
                            Body.Position.X = EntityGame.ActiveCamera.ScreenSpace.Left;
                        else if (Body.BoundingRect.Right > EntityGame.ActiveCamera.ScreenSpace.Right)
                            Body.Position.X = EntityGame.ActiveCamera.ScreenSpace.Right - Body.Bounds.X;

                        if (Body.Position.Y < EntityGame.ActiveCamera.ScreenSpace.Top)
                            Body.Position.Y = EntityGame.ActiveCamera.ScreenSpace.Top;
                        else if (Body.BoundingRect.Bottom > EntityGame.ActiveCamera.ScreenSpace.Bottom)
                            Body.Position.Y = EntityGame.ActiveCamera.ScreenSpace.Bottom - Body.Bounds.Y;
                    }
                    break;
                case MovementInput.Buttons:
                    if (!HasFocus && ButtonPressed() || SelectKey.Down())
                        GetFocus(this);
                    if (HasFocus)
                    {
                        Position = new Vector2(
                            Position.X + ((LeftKey.Down()) ? -MovementSpeed.X : 0) + ((RightKey.Down()) ? MovementSpeed.X : 0),
                            Position.Y + ((UpKey.Down()) ? -MovementSpeed.Y : 0) + ((DownKey.Down()) ? MovementSpeed.Y : 0)
                            );

                        //Move it with the camera.
                        Position += EntityGame.ActiveCamera.Delta;

                        //Keep it from leaving the bounds of the window.
                        if (Body.Position.X < EntityGame.ActiveCamera.ScreenSpace.Left)
                            Body.Position.X = EntityGame.ActiveCamera.ScreenSpace.Left;
                        else if (Body.BoundingRect.Right > EntityGame.ActiveCamera.ScreenSpace.Right)
                            Body.Position.X = EntityGame.ActiveCamera.ScreenSpace.Right - Body.Bounds.X;

                        if (Body.Position.Y < EntityGame.ActiveCamera.ScreenSpace.Top)
                            Body.Position.Y = EntityGame.ActiveCamera.ScreenSpace.Top;
                        else if (Body.BoundingRect.Bottom > EntityGame.ActiveCamera.ScreenSpace.Bottom)
                            Body.Position.Y = EntityGame.ActiveCamera.ScreenSpace.Bottom - Body.Bounds.Y;
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
            return SelectKey.Pressed();
        }

        public override bool Released()
        {
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
            SelectKey = new GamepadInput(this, "SelectKey", Buttons.A, PlayerIndex.One);
            AnalogStick = new GamePadAnalog(this, "AnalogStick", Sticks.Right, PlayerIndex.One);
            UpKey = new GamepadInput(this, "UpKey", Buttons.DPadUp, PlayerIndex.One);
            DownKey = new GamepadInput(this, "DownKey", Buttons.DPadDown, PlayerIndex.One);
            LeftKey = new GamepadInput(this, "LeftKey", Buttons.DPadLeft, PlayerIndex.One);
            RightKey = new GamepadInput(this, "RightKey", Buttons.DPadRight, PlayerIndex.One);
            Input = MovementInput.Analog;
        }
    }

    public class MouseCursor : Cursor
    {
        public MouseCursor(Node parent, string name) : base(parent, name)
        {

        }

        public override void Update(GameTime gt)
        {
            if(!HasFocus && (MouseService.Delta != Point.Zero || MouseService.IsMouseButtonDown(MouseButton.LeftButton)))
                GetFocus(this);
            if(HasFocus)
            {
                Position = new Vector2(Position.X - MouseService.Delta.X, Position.Y - MouseService.Delta.Y);

                //Move it with the camera.
                Position += EntityGame.ActiveCamera.Delta;

                //Keep it from leaving the bounds of the window.
                if (Body.Position.X < EntityGame.ActiveCamera.ScreenSpace.Left) Body.Position.X = EntityGame.ActiveCamera.ScreenSpace.Left;
                else if (Body.BoundingRect.Right > EntityGame.ActiveCamera.ScreenSpace.Right)
                    Body.Position.X = EntityGame.ActiveCamera.ScreenSpace.Right - Body.Bounds.X;

                if (Body.Position.Y < EntityGame.ActiveCamera.ScreenSpace.Top) Body.Position.Y = EntityGame.ActiveCamera.ScreenSpace.Top;
                else if (Body.BoundingRect.Bottom > EntityGame.ActiveCamera.ScreenSpace.Bottom)
                    Body.Position.Y = EntityGame.ActiveCamera.ScreenSpace.Bottom - Body.Bounds.Y;
            }

            base.Update(gt);
        }

        public override bool Down()
        {
            return MouseService.IsMouseButtonDown(MouseButton.LeftButton);
        }

        public override bool Pressed()
        {
            return MouseService.IsMouseButtonPressed(MouseButton.LeftButton);
        }

        public override bool Released()
        {
            return MouseService.IsMouseButtonReleased(MouseButton.LeftButton);
        }

        public override bool Up()
        {
            return MouseService.IsMouseButtonUp(MouseButton.LeftButton);
        }

        public override void Draw(SpriteBatch sb)
        {            
            base.Draw(sb);
        }
    }
}