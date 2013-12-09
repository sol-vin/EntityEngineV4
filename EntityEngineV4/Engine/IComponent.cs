using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Engine
{
    public delegate void EventHandler(IComponent c);
    public interface IComponent
    {
        /// <summary>
        /// Name of the IComponent
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Id number,should be set using EntityGame.GetID()
        /// </summary>
        int Id { get; }

        /// <summary>
        /// If ths object should be updated or not.
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// If this object should be drawn or not
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Toggles debug information
        /// </summary>
        bool Debug { get; set; }

        /// <summary>
        /// Whether or not this Node has been destroyed
        /// </summary>
        bool Destroyed { get; }

        float Order { get; set; }
        float Layer { get; set; }



        /// <summary>
        /// If the Initialize() method has been called since it's creation
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Called when destroyed
        /// </summary>
        event EventHandler DestroyEvent;

        /// <summary>
        /// Initalizes this 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Resets this
        /// </summary>
        void Reset();

        /// <summary>
        /// Updates this, not called if Active is false but, can always be called manually
        /// </summary>
        /// <param name="gt"></param>
        void Update(GameTime gt);

        /// <summary>
        /// Draws this, not called if Visible is false but, can always be caled manually
        /// </summary>
        /// <param name="sb"></param>
        void Draw(SpriteBatch sb);

        /// <summary>
        /// Destroys this
        /// </summary>
        /// <param name="sender"></param>
        void Destroy(IComponent sender = null);
    }
}