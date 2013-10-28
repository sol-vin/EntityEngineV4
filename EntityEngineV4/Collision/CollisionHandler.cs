using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.AccessControl;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Engine;
using EntityEngineV4.PowerTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Collision
{
    /// <summary>
    /// A state-side service for handling collisions and dealing with the resolution of those collisions.
    /// </summary>
    public class CollisionHandler : Service
    {
        //TODO: Fix AABBvsAABB collision bug
        public const int OVERLAP_BIAS = 4;
        public const int NONE = 0x0;
        public const int UP = 0x1;
        public const int DOWN = 0x10;
        public const int LEFT = 0x100;
        public const int RIGHT = 0x1000;
        public const int ALL = 0x1111;

        /// <summary>
        /// List of colliding members on this state.
        /// </summary>
        private List<Collision> _collideables;

        /// <summary>
        /// Pairs to be sent in for testing.
        /// </summary>
        private HashSet<Pair> _pairs;

        /// <summary>
        /// The pairs that have already collided and generated a manifold as a result.
        /// </summary>
        private HashSet<Manifold> _manifolds;

        public CollisionHandler(EntityState stateref)
            : base(stateref, "CollisionHandler")
        {
            _collideables = new List<Collision>();
            _pairs = new HashSet<Pair>();
            _manifolds = new HashSet<Manifold>();
        }

        public override void Initialize()
        {
            
        }

        public override void Update(GameTime gt)
        {
            BroadPhase();
            foreach (var manifold in _manifolds)
            {
                manifold.A.OnCollision(manifold.B);
                manifold.B.OnCollision(manifold.A);

                //TODO: Fix Resolution!

                if (CanObjectsResolve(manifold.A, manifold.B) || CanObjectsResolve(manifold.B, manifold.A))
                {
                    ResolveCollision(manifold);
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            _manifolds.Clear();
            foreach (var collideable in _collideables)
            {
                collideable.CollisionDirection.Mask = NONE;
            }
        }

        public void AddCollision(Collision c)
        {
            //Check if the Collision is already in the list.
            if (Enumerable.Contains(_collideables, c)) return;
            _collideables.Add(c);

            //Generate our pairs
            ReconfigurePairs(c);
        }

        public void RemoveCollision(Collision c)
        {
            if (!Enumerable.Contains(_collideables, c)) return;
            _collideables.Remove(c);

            _pairs.RemoveWhere(pair => pair.A.Equals(c) || pair.B.Equals(c));
        }

        public IEnumerable<Collision> GetColliding()
        {
            var output = new HashSet<Collision>();

            foreach (var manifold in  _manifolds)
            {
                output.Add(manifold.A);
                output.Add(manifold.B);
            }

            return output;
        }

        /// <summary>
        /// Reconfigures the pairs for a Collision c
        /// </summary>
        /// <param name="c">A collision.</param>
        public void ReconfigurePairs(Collision c)
        {
            //Remove pairs with this collision in it
            foreach (var pair in _pairs.ToArray().Where(pair => pair.A.Equals(c) || pair.B.Equals(c)))
            {
                _pairs.Remove(pair);
            }

            //Recalculate pairs with this new collision
            foreach (var other in _collideables)
            {
                if (c.Equals(other)) continue;
                if (CanObjectsPair(c, other))
                {
                    var p = new Pair(c, other);
                    _pairs.Add(p);
                }
            }
        }

        /// <summary>
        /// Generates the pairs used for testing collision.
        /// </summary>
        public void GeneratePairs()
        {
            if (_collideables.Count() <= 1) return;

            _pairs.Clear();

            foreach (var a in _collideables)
            {
                foreach (var b in _collideables)
                {
                    if (a.Equals(b)) continue;
                    if (CanObjectsPair(a, b))
                    {
                        var p = new Pair(a, b);
                        _pairs.Add(p);
                    }
                }
            }
        }

        public void BroadPhase()
        {
            //Do a basic SAT test
            //foreach (var pair in _pairs)
            //{
            //    Manifold m = AABBvsAABB(AABB.Create(pair.A), AABB.Create(pair.B));
            //    if (m.AreColliding)
            //    {
            //        //Do our real test now.
            //        if (pair.A.Shape is AABB && pair.B.Shape is AABB)
            //            //If the shapes are both AABB's, skip the check, we already have it
            //            _manifolds.Add(m);
            //        else
            //        {
            //            m = CheckCollision(pair.A.Shape, pair.B.Shape);
            //            if (m.AreColliding)
            //                _manifolds.Add(m);
            //        }
            //    }
            //}

            foreach (var pair in _pairs)
            {
                Manifold m = CheckCollision(pair.A, pair.B);
                if (m.AreColliding)
                    _manifolds.Add(m);
            }
        }

        //Static methods

        /// <summary>
        /// Compares the masks and checks to see if they should be allowed to form a pair.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Whether or not the the two objects should be paired</returns>
        public static bool CanObjectsPair(Collision a, Collision b)
        {
            return (a.GroupMask.HasMatchingBit(b.PairMask) || //Compare the pair masks to the group masks.
                    a.PairMask.HasMatchingBit(b.GroupMask)) && a.Enabled && b.Enabled;
        }

        public static bool CanObjectsResolve(Collision resolver, Collision other)
        {
            return resolver.ResolutionGroupMask.HasMatchingBit(other.ResolutionGroupMask) //Compare the pair mask one sided.
                && resolver.Enabled && other.Enabled && !resolver.Immovable;
        }

        public static void ResolveCollision(Manifold m)
        {
            Vector2 relVelocity = m.B.Velocity - m.A.Velocity;
            //Finds out if the objects are moving towards each other.
            //We only need to resolve collisions that are moving towards, not away.
            float velAlongNormal = relVelocity.X * m.Normal.X + relVelocity.Y * m.Normal.Y;
            if (velAlongNormal > 0)
                return;
            float e = Math.Min(m.A.Restitution, m.B.Restitution);

            float j = -(1 + e) * velAlongNormal;
            j /= m.A.InvertedMass + m.B.InvertedMass;

            Vector2 impulse = j * m.Normal;
            if (CanObjectsResolve(m.A, m.B))
                m.A.Velocity -= m.A.InvertedMass * impulse;
            if (CanObjectsResolve(m.B, m.A))
                m.B.Velocity += m.B.InvertedMass * impulse;
        }

        public const float SLOP = 0.01f;
        public const float PERCENT = 0.2f;
        public static void PositionalCorrection(Manifold m)
        {
            Vector2 correction = Math.Max(m.PenetrationDepth - SLOP, 0.0f) / (m.A.InvertedMass + m.B.InvertedMass) * PERCENT * m.Normal;
            if (CanObjectsResolve(m.A, m.B))
                m.A.Position -= m.A.InvertedMass * correction;
            if (CanObjectsResolve(m.B, m.A))
                m.B.Position += m.B.InvertedMass * correction;
        }

        /// <summary>
        /// Compares bounding boxes using Seperating Axis Thereom.
        /// </summary>
        public static bool AABBvsAABB(AABB a, AABB b, ref Manifold manifold)
        {
            manifold.Normal = a.Position - b.Position;

            //Calculate half widths
            float aExtent = a.Width / 2f;
            float bExtent = b.Width / 2f;

            //Calculate the overlap.
            float xExtent = aExtent + bExtent - Math.Abs(manifold.Normal.X);

            //If the overlap is greater than 0
            if (xExtent > 0)
            {
                //Calculate half widths
                aExtent = a.Height/2f;
                bExtent = b.Height/2f;

                //Calculate overlap
                float yExtent = aExtent + bExtent - Math.Abs(manifold.Normal.Y);

                if (yExtent > 0)
                {
                    //Variable to multiply the normal by to make the collision resolve
                    Vector2 faceNormal;

                    //Check to see which axis has the biggest "penetration" ;D

                    //Collision is happening on Y axis
                    if (xExtent > yExtent)
                    {
                        if (manifold.Normal.X < 0)
                            faceNormal = -Vector2.UnitX;
                        else
                            faceNormal = Vector2.UnitX;
                        manifold.PenetrationDepth = xExtent;

                        manifold.Normal = MathTools.Physics.GetNormal(a.Position, b.Position) * faceNormal.X;
                        manifold.AreColliding = true;

                        //TODO: Finish collision code
                        /// Need to find the axis of deepest penetration and only display flags from that side
                        /// UNLESS the penetration depth on the other sides are more than half the width. 
                        //A First
                        if (a.Top > b.Top && a.Top < b.Bottom && manifold.A.AllowCollisionDirection.HasMatchingBit(UP))
                            manifold.A.CollisionDirection.CombineMask(UP);
                        if (a.Bottom < b.Bottom && a.Bottom > b.Top && manifold.A.AllowCollisionDirection.HasMatchingBit(DOWN))
                            manifold.A.CollisionDirection.CombineMask(DOWN);
                        if (a.Left > b.Left && a.Left < b.Right && manifold.A.AllowCollisionDirection.HasMatchingBit(LEFT))
                            manifold.A.CollisionDirection.CombineMask(LEFT);
                        if (a.Right < b.Right && a.Right > b.Left && manifold.A.AllowCollisionDirection.HasMatchingBit(RIGHT))
                            manifold.A.CollisionDirection.CombineMask(RIGHT);

                        //B next
                        if (b.Top > a.Top && b.Top < a.Bottom && manifold.B.AllowCollisionDirection.HasMatchingBit(UP))
                            manifold.B.CollisionDirection.CombineMask(UP);
                        if (b.Bottom < a.Bottom && b.Bottom > a.Top && manifold.B.AllowCollisionDirection.HasMatchingBit(DOWN))
                            manifold.B.CollisionDirection.CombineMask(DOWN);
                        if (b.Left > a.Left && b.Left < a.Right && manifold.B.AllowCollisionDirection.HasMatchingBit(LEFT))
                            manifold.B.CollisionDirection.CombineMask(LEFT);
                        if (b.Right < a.Right && b.Right > a.Left && manifold.B.AllowCollisionDirection.HasMatchingBit(RIGHT))
                            manifold.B.CollisionDirection.CombineMask(RIGHT);
                        return true;
                    }
                        //Collision happening on X axis
                    else
                    {
                        if (manifold.Normal.Y < 0)
                            faceNormal = -Vector2.UnitY;
                        else
                            faceNormal = Vector2.UnitY;

                        manifold.Normal = MathTools.Physics.GetNormal(a.Position, b.Position) * faceNormal.Y;
                        manifold.PenetrationDepth = yExtent;
                        manifold.AreColliding = true;

                        //A First
                        if (a.Top > b.Top && a.Top < b.Bottom && manifold.A.AllowCollisionDirection.HasMatchingBit(UP))
                            manifold.A.CollisionDirection.CombineMask(UP);
                        if (a.Bottom < b.Bottom && a.Bottom > b.Top && manifold.A.AllowCollisionDirection.HasMatchingBit(DOWN))
                            manifold.A.CollisionDirection.CombineMask(DOWN);
                        if (a.Left > b.Left && a.Left < b.Right && manifold.A.AllowCollisionDirection.HasMatchingBit(LEFT))
                            manifold.A.CollisionDirection.CombineMask(LEFT);
                        if (a.Right < b.Right && a.Right > b.Left && manifold.A.AllowCollisionDirection.HasMatchingBit(RIGHT))
                            manifold.A.CollisionDirection.CombineMask(RIGHT);

                        //B First
                        if (b.Top > a.Top && b.Top < a.Bottom && manifold.B.AllowCollisionDirection.HasMatchingBit(UP))
                            manifold.B.CollisionDirection.CombineMask(UP);
                        if (b.Bottom < a.Bottom && b.Bottom > a.Top && manifold.B.AllowCollisionDirection.HasMatchingBit(DOWN))
                            manifold.B.CollisionDirection.CombineMask(DOWN);
                        if (b.Left > a.Left && b.Left < a.Right && manifold.B.AllowCollisionDirection.HasMatchingBit(LEFT))
                            manifold.B.CollisionDirection.CombineMask(LEFT);
                        if (b.Right < a.Right && b.Right > a.Left && manifold.B.AllowCollisionDirection.HasMatchingBit(RIGHT))
                            manifold.B.CollisionDirection.CombineMask(RIGHT);
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public static bool CircleVSCircle(Circle a, Circle b, ref Manifold manifold)
        {
            manifold.Normal = b.Position - a.Position;
            float r = a.Radius + b.Radius;
            r *= r;
            float l = manifold.Normal.LengthSquared();
            if (l > r)
            {
                //Set manifold for failure
                manifold.AreColliding = false;
                return false;
            }

            float d = manifold.Normal.Length();
            if (Math.Abs(d) > float.Epsilon)
            {
                manifold.PenetrationDepth = a.Radius + b.Radius - d;
                manifold.AreColliding = true;
                return true;
            }
            else
            {
                //find which one is bigger
                float maxRadius = Math.Max(a.Radius, b.Radius);
                manifold.PenetrationDepth = maxRadius;
                manifold.AreColliding = true;
                return true;
            }
        }

        //Collision resolver methods
        /// <summary>
        /// Uses a table to test collision between various shapes
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Manifold CheckCollision(Collision a, Collision b)
        {
            Shape aShape, bShape;
            aShape = a.GetLink<Shape>(Collision.DEPENDENCY_SHAPE);
            bShape = b.GetLink<Shape>(Collision.DEPENDENCY_SHAPE);

            Manifold manifold = new Manifold(a,b);

            if (aShape is AABB && bShape is AABB)
                AABBvsAABB((AABB)aShape, (AABB)bShape, ref manifold);
            else if (aShape is Circle && bShape is Circle)
                CircleVSCircle((Circle)aShape, (Circle)bShape, ref manifold);
            else
                throw new Exception("No existing methods for this kind of collision!");

            return manifold;
        }
    }
}