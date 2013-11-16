using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Components
{
    public class Physics : Component
    {
        public float AngularVelocity;
        public float AngularDrag = 1f;

        /// <summary>
        /// The velcocity if the object measured in px/frame
        /// </summary>
        public Vector2 Velocity = Vector2.Zero;

        public float Drag = 1f;
        public Vector2 Acceleration = Vector2.Zero;
        private Vector2 _force = Vector2.Zero;

        public Vector2 Force
        {
            get { return _force; }
            set { _force = value; }
        }

        private float _angularForce = 0f;
        public float AngularForce
        {
            get { return _angularForce; }
            set { _angularForce = value; }
        }

        //// <summary>
        /// Backing field for Mass.
        /// </summary>
        private float _mass = 1f;

        /// <summary>
        /// The mass of the object.
        /// </summary>
        /// <value>
        /// The mass.
        /// </value>
        public float Mass
        {
            get { return _mass; }
            set
            {
                if (value < 0) throw new Exception("Mass cannot be less than zero!");
                _mass = value;

                if (Math.Abs(value - 0) < .00001f)
                    InvertedMass = 0;
                else
                    InvertedMass = 1 / _mass;
            }
        }

        /// <summary>
        /// Gets one divided by mass (1/mass).
        /// </summary>
        /// <value>
        /// The inverted mass.
        /// </value>
        public float InvertedMass { get; private set; }

        /// <summary>
        /// Bounciness of this object
        /// </summary>
        public float Restitution = 0f;


        
        public Physics(Node parent, string name)
            : base(parent, name)
        {
        }

        public override void Update(GameTime gt)
        {
            Velocity += Acceleration;
            Velocity += _force;
            _force = Vector2.Zero;
            Velocity *= Drag;

            AngularVelocity += _angularForce;
            AngularVelocity *= AngularDrag;
            _angularForce = 0f;

            GetDependency<Body>(DEPENDENCY_BODY).Position += Velocity;
            GetDependency<Body>(DEPENDENCY_BODY).Angle += AngularVelocity;
        }

        public void Thrust(float power)
        {
            var angle = GetDependency<Body>(DEPENDENCY_BODY).Angle;
            Thrust(power, angle);
        }

        public void Thrust(float power, float angle)
        {
            Velocity.X -= (float)Math.Sin(-angle) * power;
            Velocity.Y -= (float)Math.Cos(-angle) * power;
        }

        public void FaceVelocity()
        {
            GetDependency<Body>(DEPENDENCY_BODY).Angle = (float)Math.Atan2(Velocity.X, -Velocity.Y);
        }

        public void FaceVelocity(Vector2 velocity)
        {
            GetDependency<Body>(DEPENDENCY_BODY).Angle = (float)Math.Atan2(velocity.X, velocity.Y);
        }

        public void AddForce(Vector2 force)
        {
            _force += force;
        }

        public Physics Clone()
        {
            Physics p = new Physics(Parent, Name);
            p.AngularVelocity = AngularVelocity;
            p.AngularDrag = AngularDrag;
            p.Drag = Drag;
            p.Velocity = Velocity;
            p.Acceleration = Acceleration;
            p.LinkDependency(DEPENDENCY_BODY, GetDependency(DEPENDENCY_BODY));
            return p;
        }

        public void AddAngularForce(float force)
        {
            _angularForce += force;
        }

        public override void Reuse(Node parent, string name)
        {
            base.Reuse(parent, name);
            Velocity = Vector2.Zero;
            AngularVelocity = 0;
            Drag = 1;
            AngularDrag = 1;
            AngularForce = 0;
            Force = Vector2.Zero;
            Acceleration = Vector2.Zero;
            Mass = 1;
            Restitution = 0;
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
        public override void CreateDependencyList()
        {
            base.CreateDependencyList();
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }
    }
}