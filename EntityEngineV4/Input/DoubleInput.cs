using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4.Input
{
    public sealed class DoubleInput : Input
    {
        public KeyboardInput Key;
        public GamepadInput Button;

        public DoubleInput(Entity parent, string name, Keys key, Buttons button, PlayerIndex pi)
            : base(parent, name)
        {
            Key = new KeyboardInput(parent, name + "key", key);
            Button = new GamepadInput(parent, name + "gamepad", button, pi);
        }

        public override bool Pressed()
        {
            return Key.Pressed() || Button.Pressed();
        }

        public override bool Released()
        {
            return Key.Released() || Button.Released();
        }

        public override bool Down()
        {
            return Key.Down() || Button.Down();
        }

        public override bool Up()
        {
            return Key.Up() && Button.Up();
        }
    }
}