using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4.Input
{
    public sealed class GamepadInput : Input
    {
        private readonly PlayerIndex _pi;
        private Buttons _button;

        public GamepadInput(Entity e, string name, Buttons button, PlayerIndex pi)
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
            }
        }

        public PlayerIndex PlayerIndex
        {
            get { return _pi; }
        }

        //Read-Only

        public override bool Pressed()
        {
            return InputService.ButtonPressed(Button, PlayerIndex);
        }

        public override bool Released()
        {
            return InputService.ButtonReleased(Button, PlayerIndex);
        }

        public override bool Down()
        {
            return InputService.ButtonDown(Button, PlayerIndex);
        }

        public override bool Up()
        {
            return InputService.ButtonUp(Button, PlayerIndex);
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
                    Position = new Vector2(InputService.GamePadStates[(int)PlayerIndex].ThumbSticks.Left.X,
                                           InputService.GamePadStates[(int)PlayerIndex].ThumbSticks.Left.Y);
                    break;

                case Sticks.Right:
                    Position = new Vector2(InputService.GamePadStates[(int)PlayerIndex].ThumbSticks.Right.X,
                                    InputService.GamePadStates[(int)PlayerIndex].ThumbSticks.Right.Y);
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
        public float DownThreshold = .7f;
        public float DeadZone = .1f;

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
            return Up() && _lastvalue > DownThreshold;
        }

        public override bool Pressed()
        {
            return Down() && _lastvalue <= DownThreshold;
        }

        public override bool Down()
        {
            return Value > DownThreshold;
        }

        public override bool Up()
        {
            return Value <= DownThreshold;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            _lastvalue = Value;
            switch (Trigger)
            {
                case Triggers.Left:
                    Value = InputService.GamePadStates[(int)PlayerIndex].Triggers.Left;
                    break;

                case Triggers.Right:
                    Value = InputService.GamePadStates[(int)PlayerIndex].Triggers.Right;
                    break;
            }
        }
    }
}