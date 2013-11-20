using System;
using System.Collections.Generic;
using System.Linq;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Components;
using EntityEngineV4.Data;
using EntityEngineV4.Engine;
using EntityEngineV4.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Collision
{
    public class Collision : Component
    {
        //Delegates and events
        public delegate void CollisionEventHandler(Manifold m);

        public event CollisionEventHandler CollideEvent;

        public Bitmask CollisionDirection = new Bitmask();

        public Bitmask AllowCollisionDirection = new Bitmask(CollisionHandler.ALL);

        private List<Collision> _collidedWith = new List<Collision>();

        public List<Collision> CollidedWith
        {
            get { return _collidedWith.ToList(); }
        }

        public bool IsColliding { get { return CollidedWith.Count > 0; } }

        /// <summary>
        /// The group mask is the bit mask used to determine which groups the component is a part of.
        /// The CollisionHandler will pair all components with the same pair mask.
        /// </summary>
        /// <value>
        /// The group mask.
        /// </value>
        public Bitmask GroupMask { get; protected set; }

        /// <summary>
        /// The pair mask is the bit mask used to determine which groups the component will pair with.
        /// The CollisionHandler will only pair components whose group mask matches the pair mask.
        /// </summary>
        /// <value>
        /// The pair mask.
        /// </value>
        public Bitmask PairMask { get; protected set; }

        /// <summary>
        /// The resolution mask is the bit mask which will determine which groups will physically collide with each other
        /// </summary>
        public Bitmask ResolutionGroupMask { get; protected set; }

        public Color DebugColor = Color.Magenta;

        private bool _enabled = true;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return; //Opt out if the value isn't actually changing

                _enabled = value;
                _collisionHandler.ReconfigurePairs(this);
            }
        }

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


        //Dependencies
        private CollisionHandler _collisionHandler;

        public Collision(Node parent, string name)
            : base(parent, name)
        {
            _collisionHandler = GetRoot<State>().GetService<CollisionHandler>();

            GetRoot<State>().PreUpdateEvent += _collidedWith.Clear;

            GroupMask = new Bitmask();
            GroupMask.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            PairMask = new Bitmask();
            PairMask.BitmaskChanged += bm => _collisionHandler.ReconfigurePairs(this);

            ResolutionGroupMask = new Bitmask();

            _collisionHandler.AddCollision(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            try
            {
                GetDependency<Physics>(DEPENDENCY_PHYSICS);
            }
            catch
            {
                throw new Exception("Physics does not exist in the dependency list for " + Name);
            }

            try
            {
                 GetDependency<Shape>(DEPENDENCY_SHAPE);
            }
            catch (Exception)
            {
                throw new Exception("Shape does not exist in the dependency list for " + Name);

            }
            //Make sure shape and physics are on the same body.
            if (GetDependency(DEPENDENCY_PHYSICS).GetDependency(Physics.DEPENDENCY_BODY).Id !=
                GetDependency(DEPENDENCY_SHAPE).GetDependency(Shape.DEPENDENCY_BODY).Id)
            {
                EntityGame.Log.Write("Shape and Physics dependencies do not have the same body dependency", this, Alert.Error);
                throw new Exception("Shape and Physics do not share the same body");
            }

            LinkDependency(DEPENEDENCY_BODY, GetDependency(DEPENDENCY_PHYSICS).GetDependency(Physics.DEPENDENCY_BODY));
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