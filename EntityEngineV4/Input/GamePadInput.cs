using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4.Input
{
    public sealed class GamePadInput : Input
    {
        private readonly PlayerIndex _pi;
        private Buttons _button;

        public GamePadInput(Entity e, string name, Buttons button, PlayerIndex pi)
            : base(e, name)
        {
            _button = button;
            _pi = pi;
        }

        public Buttons Button
        {
            get { return _button; }

            set
            {
                _button = value;
                InputHandler.Flush();
            }
        }

        public PlayerIndex PlayerIndex
        {
            get { return _pi; }
        }

        //Read-Only

        public override bool Pressed()
        {
            return InputHandler.ButtonPressed(Button, PlayerIndex);
        }

        public override bool Released()
        {
            return InputHandler.ButtonReleased(Button, PlayerIndex);
        }

        public override bool Down()
        {
            return InputHandler.ButtonDown(Button, PlayerIndex);
        }

        public override bool Up()
        {
            return InputHandler.ButtonUp(Button, PlayerIndex);
        }
    }

    public enum Sticks
    {
        Left,
        Right
    }

    public class GamePadAnalog : Component
    {
        public PlayerIndex PlayerIndex;
        public Sticks Stick;
        public float Threshold;



        public GamePadAnalog(Entity parent, string name, Sticks stick, PlayerIndex pi)
            : base(parent, name)
        {
            Stick = stick;
            PlayerIndex = pi;
        }

        public Vector2 Delta { get { return LastPosition - Position; } }

        public Vector2 LastPosition { get; private set; }
        public Vector2 Position { get; private set; }

        public bool Left
        {
            get { return (Position.X > Threshold); }
        }

        public bool Right
        {
            get { return (Position.X < -Threshold); }
        }

        public bool Up
        {
            get { return (Position.Y > Threshold); }
        }

        public bool Down
        {
            get { return (Position.Y < -Threshold); }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            LastPosition = Position;

            switch (Stick)
            {
                case Sticks.Left:
                    Position = new Vector2(InputHandler.GamePadStates[(int)PlayerIndex].ThumbSticks.Left.X,
                                           InputHandler.GamePadStates[(int)PlayerIndex].ThumbSticks.Left.Y);
                    break;

                case Sticks.Right:
                    Position = new Vector2(InputHandler.GamePadStates[(int)PlayerIndex].ThumbSticks.Right.X,
                                    InputHandler.GamePadStates[(int)PlayerIndex].ThumbSticks.Right.Y);
                    break;
            }
        }
    }

    public enum Triggers
    {
        Left,
        Right
    }

    public class GamePadTrigger : Input
    {
        public PlayerIndex PlayerIndex;
        public Triggers Trigger;
        public float Threshold = .7f;

        public float Value { get; private set; }

        private float _lastvalue;

        public GamePadTrigger(Entity entity, string name)
            : base(entity, name)
        {
        }

        public GamePadTrigger(Entity entity, string name, Triggers trigger, PlayerIndex pi)
            : base(entity, name)
        {
            Trigger = trigger;
            PlayerIndex = pi;
        }

        public override bool Released()
        {
            return Up() && _lastvalue > Threshold;
        }

        public override bool Pressed()
        {
            return Down() && _lastvalue <= Threshold;
        }

        public override bool Down()
        {
            return Value > Threshold;
        }

        public override bool Up()
        {
            return Value <= Threshold;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            _lastvalue = Value;
            switch (Trigger)
            {
                case Triggers.Left:
                    Value = InputHandler.GamePadStates[(int)PlayerIndex].Triggers.Left;
                    break;

                case Triggers.Right:
                    Value = InputHandler.GamePadStates[(int)PlayerIndex].Triggers.Right;
                    break;
            }
        }
    }
}