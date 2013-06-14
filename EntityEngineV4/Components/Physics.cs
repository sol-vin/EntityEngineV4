using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Components
{
    public class Physics : Component
    {
        public float AngularVelocity;
        public float AngularVelocityDrag = 1f;

        /// <summary>
        /// The velcocity if the object measured in px/frame
        /// </summary>
        public Vector2 Velocity = Vector2.Zero;

        public Vector2 LastVelocity { get; private set; }

        public float Drag = 1f;
        public Vector2 Acceleration = Vector2.Zero;
        private Vector2 _force = Vector2.Zero;

        //Dependencies
        private Body _body;

        public Physics(Entity e, string name, Body body)
            : base(e, name)
        {
            _body = body;
        }

        public override void Update(GameTime gt)
        {
            LastVelocity = Velocity;

            Velocity += Acceleration;
            Velocity += _force;
            _force = Vector2.Zero;
            Velocity *= Drag;
            AngularVelocity *= AngularVelocityDrag;

            _body.Position += Velocity;
            _body.Angle += AngularVelocity;
        }

        public void Thrust(float power)
        {
            var angle = _body.Angle;
            Thrust(power, angle);
        }

        public void Thrust(float power, float angle)
        {
            Velocity.X -= (float)Math.Sin(-angle) * power;
            Velocity.Y -= (float)Math.Cos(-angle) * power;
        }

        public void FaceVelocity()
        {
            _body.Angle = (float)Math.Atan2(Velocity.X, -Velocity.Y);
        }

        public void FaceVelocity(Vector2 velocity)
        {
            _body.Angle = (float)Math.Atan2(velocity.X, velocity.Y);
        }

        public void AddForce(Vector2 force)
        {
            _force += force;
        }

        public Physics Clone()
        {
            Physics p = new Physics(Parent, Name, _body);
            p.AngularVelocity = AngularVelocity;
            p.AngularVelocityDrag = AngularVelocityDrag;
            p.Drag = Drag;
            p.Velocity = Velocity;
            p.Acceleration = Acceleration;
            return p;
        }
    }
}