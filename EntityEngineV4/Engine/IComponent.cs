using System;
using EntityEngineV4.Engine.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public delegate void EventHandler(IComponent c);
    public interface IComponent
    {
        string Name { get; }

        int Id { get; }

        bool Active { get; set; }

        bool Visible { get; set; }

        bool Debug { get; set; }

        bool Destroyed { get; }

        float Order { get; set; }
        float Layer { get; set; }



        /// <summary>
        /// If the Initialize() method has been called since it's creation
        /// </summary>
        bool Initialized { get;}

        event EventHandler DestroyEvent;

        void Initialize();
        void Reset();
        void Update(GameTime gt);
        void Draw(SpriteBatch sb);

        void Destroy(IComponent sender = null);
    }
}