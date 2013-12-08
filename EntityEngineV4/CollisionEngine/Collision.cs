using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.CollisionEngine.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.CollisionEngine
{
    public class Collision : Component
    {
        //Delegates and events
        public delegate void CollisionEventHandler(Manifold m);

        public event CollisionEventHandler CollideEvent;

        private List<Collision> _collidedWith = new List<Collision>();

        public List<Collision> CollidedWith
        {
            get { return _collidedWith.ToList(); }
        }

        public bool AllowResolution = true;

        public bool IsColliding { get { return CollidedWith.Count > 0; } }

        /// <summary>
        /// The group mask is the bit mask used to determine which groups the component is a part of.
        /// The CollisionHandler will pair all components with the same pair mask.
        /// </summary>
        /// <value>
        /// The group mask.
        /// </value>
        public Bitmask Group { get; protected set; }

        /// <summary>
        /// The pair mask is the bit mask used to determine which groups the component will pair with.
        /// The CollisionHandler will only pair components whose group mask matches the pair mask.
        /// </summary>
        /// <value>
        /// The pair mask.
        /// </value>
        public Bitmask Pair { get; protected set; }

        /// <summary>
        /// The resolution mask is the bit mask which will determine which groups will physically collide with each other
        /// </summary>
        public Bitmask ResolutionGroup { get; protected set; }


        /// <summary>
        /// Decides how the resolution will work, if there will be any at all.
        /// </summary>
        public bool Immovable;

        //Dependency properties
        public Vector2 Position
        {
            get { return GetDependency<Body>(DEPENEDENCY_BODY).Position; }
            set { GetDependency<Body>(DEPENEDENCY_BODY).Position = value; }

        }

        public Vector2 Velocity
        {
            get { return GetDependency<Physics>(DEPENDENCY_PHYSICS).Velocity; }
            set { GetDependency<Physics>(DEPENDENCY_PHYSICS).Velocity = value; }

        }

        public float Restitution { get { return GetDependency<Physics>(DEPENDENCY_PHYSICS).Restitution; } }
        public float Mass { get { return GetDependency<Physics>(DEPENDENCY_PHYSICS).Mass; }}
        public float InvertedMass { get { return GetDependency<Physics>(DEPENDENCY_PHYSICS).InvertedMass; } }
        public Vector2 Delta { get { return GetDependency<Body>(DEPENEDENCY_BODY).Delta; } }


        public HashSet<Collision> Exclusions = new HashSet<Collision>();

        //Dependencies
        private CollisionHandler _collisionHandler;

        public Collision(Node parent, string name)
            : base(parent, name)
        {
            if (!GetRoot<State>().CheckService<CollisionHandler>())
                _collisionHandler = new CollisionHandler(GetRoot<State>()); //If the collision handler doesn't exist add it.
            else
                _collisionHandler = GetRoot<State>().GetService<CollisionHandler>();
            

            GetRoot<State>().PreUpdateEvent += _collidedWith.Clear;

            Group = new Bitmask();
            Group.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            Pair = new Bitmask();
            Pair.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            ResolutionGroup = new Bitmask();

            _collisionHandler.AddCollision(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            try
            {
                 GetDependency<Shape>(DEPENDENCY_SHAPE);
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("Shape does not exist in the dependency list for " + Name);
            }
            LinkDependency(DEPENEDENCY_BODY, GetDependency(DEPENDENCY_SHAPE).GetDependency(Shape.DEPENDENCY_BODY));

            try
            {
                GetDependency<Physics>(DEPENDENCY_PHYSICS);
            }
            catch (KeyNotFoundException)
            {
                AllowResolution = false;
            }
        }

        public override void Destroy(IComponent sender = null)
        {
            base.Destroy(sender);
            _collisionHandler.RemoveCollision(this);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public void OnCollision(Manifold m)
        {
            _collidedWith.Add(m.A != this ? m.A : m.B);
            if (CollideEvent != null)
                CollideEvent(m);
        }

        //Public dependencies
        public const int DEPENDENCY_PHYSICS = 0;
        public const int DEPENDENCY_SHAPE = 1;

        //Private Dependencies
        /// <summary>
        /// Body of the shape. Fufilled by Shape's body internally, ensuring the body this uses and the body shape uses are the same.
        /// </summary>
        private const int DEPENEDENCY_BODY = 2;

        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_PHYSICS, typeof(Physics));
            AddLinkType(DEPENDENCY_SHAPE, typeof(Shape));
            AddLinkType(DEPENEDENCY_BODY, typeof(Body));
        }
    }
}