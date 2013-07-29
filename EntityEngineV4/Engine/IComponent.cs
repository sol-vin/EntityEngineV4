using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public interface IComponent
    {
        IComponent Parent { get; }

        string Name { get; }

        uint Id { get; }

        bool Active { get; }

        bool Visible { get; }

        bool Debug { get; set; }
        
        event Component.EventHandler AddComponentEvent, RemoveComponentEvent;

        event Entity.EventHandler AddEntityEvent, RemoveEntityEvent;

        void Update(GameTime gt);

        void Draw(SpriteBatch sb);

        void Destroy(IComponent i = null);

        void AddComponent(Component c);

        void RemoveComponent(Component c);

        void AddEntity(Entity c);

        void RemoveEntity(Entity c);


    }
}