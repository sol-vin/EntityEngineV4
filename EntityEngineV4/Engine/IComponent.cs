using System;
using EntityEngineV4.Engine.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public delegate void EventHandler(IComponent c);
    public interface IComponent
    {
        IComponent Parent { get; }

        string Name { get; }

        uint Id { get; }

        bool Active { get; }

        bool Visible { get; }

        bool Debug { get; set; }

        /// <summary>
        /// If the Initialize() method has been called since it's creation
        /// </summary>
        bool IsInitialized { get;}

        event Component.EventHandler AddComponentEvent, RemoveComponentEvent;

        event Entity.EventHandler AddEntityEvent, RemoveEntityEvent;

        event Service.EventHandler AddServiceEvent , RemoveServiceEvent;
        event Service.ReturnHandler GetServiceEvent;

        event EventHandler DestroyEvent;

        void Initialize();
        void Update(GameTime gt);
        void Draw(SpriteBatch sb);

        void Destroy(IComponent i = null);

        void AddComponent(Component c);
        //TODO: Add get component through type of component
        //T GetComponent<T>() where T : Component;
        void RemoveComponent(Component c);

        void AddEntity(Entity c);
        void RemoveEntity(Entity c);

        void AddService(Service s);
        void RemoveService(Service s);
        T GetService<T>() where T : Service;
        Service GetService(Type t);
    }
}