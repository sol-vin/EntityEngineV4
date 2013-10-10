using EntityEngineV4.Engine;
using Microsoft.Xna.Framework.Input;

namespace EntityEngineV4.Input
{
    public sealed class KeyboardInput : Input
    {
        private Keys _key;

        public Keys Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
                InputService.Flush();
            }
        }

        public KeyboardInput(Entity parent, string name, Keys key)
            : base(parent, name)
        {
            _key = key;
        }

        public override bool Pressed()
        {
            return InputService.KeyPressed(Key);
        }

        public override bool Released()
        {
            return InputService.KeyReleased(Key);
        }

        public override bool Down()
        {
            return InputService.KeyDown(Key);
        }

        public override bool Up()
        {
            return InputService.KeyUp(Key);
        }
    }
}