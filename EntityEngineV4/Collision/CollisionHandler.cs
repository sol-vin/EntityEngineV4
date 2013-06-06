using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Collision.Shapes;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Collision
{
    public class CollisionHandler : Service
    {
        private List<Collision> _collideables;
        private HashSet<Pair> _pairs;
        private HashSet<Manifold> _manifolds;

        public CollisionHandler(EntityState stateref) : base(stateref)
        {
            _collideables = new List<Collision>();
            _pairs = new HashSet<Pair>();
            _manifolds = new HashSet<Manifold>();
        }

        public override void Update(GameTime gt)
        {
            BroadPhase();
            foreach (var manifold in _manifolds)
            {
                manifold.A.OnCollision(manifold.B);
                manifold.B.OnCollision(manifold.A);

                //Attempt to resolve collisions
                if (CanObjectsResolve(manifold.A, manifold.B) || CanObjectsResolve(manifold.B, manifold.A))
                {
                    ResolveCollision(manifold);
                    PositionalCorrection(manifold);
                }
                
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            _manifolds.Clear();
        }
        
        public void AddCollision(Collision c)
        {
            //Check if the Collision is already in the list.
            if (Enumerable.Contains(_collideables, c)) return;
            _collideables.Add(c);

            //Generate our pairs
            GeneratePairs();
        }

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
                        var p = new Pair(a,b);
                        _pairs.Add(p);
                    }
                }
            }
        }

        public void BroadPhase()
        {
            //Do a basic SAT test
            foreach (var pair in _pairs)
            {
                Vector2 normal = pair.A.Position - pair.B.Position;

                //Calculate half widths
                float aExtent = pair.A.BoundingRect.Width / 2f;
                float bExtent = pair.B.BoundingRect.Width / 2f;

                //Calculate the overlap. 
                float xExtent = aExtent + bExtent - Math.Abs(normal.X);

                //If the overlap is greater than 0
                if (xExtent > 0)
                {
                    //Calculate half widths
                    aExtent = pair.A.BoundingRect.Height / 2f;
                    bExtent = pair.B.BoundingRect.Height / 2f;

                    //Calculate overlap
                    float yExtent = aExtent + bExtent - Math.Abs(normal.Y);

                    if (yExtent > 0)
                    {
                        //Do our real test now.
                        Manifold m = CheckCollision(pair.A.Shape, pair.B.Shape);
                        if (m.AreColliding)
                            _manifolds.Add(m);
                    }
                }
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
            return a.GroupMask.HasMatchingBit(b.GroupMask) || //Compare the group masks.
                   a.GroupMask.HasMatchingBit(b.PairMask) || //Compare the pair masks to the group masks.
                    a.PairMask.HasMatchingBit(b.GroupMask);
        }

        public static bool CanObjectsResolve(Collision resolver, Collision other)
        {
            return resolver.ResolutionGroupMask.HasMatchingBit(other.ResolutionGroupMask) || //Compare the group masks.
                    resolver.ResolutionPairMask.HasMatchingBit(other.ResolutionGroupMask);
        }

        public static void ResolveCollision(Manifold m)
        {
            Vector2 relVelocity = m.A.Velocity - m.A.Velocity;
            //Finds out if the objects are moving towards each other.
            //We only need to resolve collisions that are moving towards, not away.
            float velAlongNormal = PhysicsMath.DotProduct(relVelocity, m.Normal);
            if (velAlongNormal > 0)
                return;
            float e = Math.Min(m.A.Restitution, m.B.Restitution);

            float j = -(1 + e) * velAlongNormal;
            j /= m.A.InvertedMass + m.B.InvertedMass;

            Vector2 impulse = j * m.Normal;
            if (CanObjectsResolve(m.A, m.B))
                m.A.Velocity -= m.A.InvertedMass * impulse;
            if(CanObjectsResolve(m.B, m.A))
                m.B.Velocity += m.B.InvertedMass * impulse;
        }

        public static void PositionalCorrection(Manifold m)
        {
            const float percent = 0.2f;
            const float slop = 0.01f;
            Vector2 correction = Math.Max(m.PenetrationDepth - slop, 0.0f) / (m.A.InvertedMass + m.B.InvertedMass) * percent * m.Normal;
            m.A.Position -= m.A.InvertedMass * correction;
            m.B.Position += m.B.InvertedMass * correction;
        }

        /// <summary>
        /// Compares bounding boxes using Seperating Axis Thereom. 
        /// </summary>
        public static Manifold AABBvsAABB(AABB a, AABB b)
        {
            //Start packing the manifold
            Manifold m = new Manifold(a.Collision, b.Collision);
            m.Normal = a.Position - b.Position;

            //Calculate half widths
            float aExtent = a.Width / 2f;
            float bExtent = b.Width / 2f;

            //Calculate the overlap. 
            float xExtent = aExtent + bExtent - Math.Abs(m.Normal.X);

            //If the overlap is greater than 0
            if (xExtent > 0)
            {
                //Calculate half widths
                aExtent = a.Height / 2f;
                bExtent = b.Height / 2f;

                //Calculate overlap
                float yExtent = aExtent + bExtent - Math.Abs(m.Normal.Y);

                if (yExtent > 0)
                {
                                        //Find which axis has the biggest penetration ;D
                    Vector2 fixnormal;
                    if (xExtent > yExtent){
                        if(m.Normal.X < 0)
                            fixnormal = -Vector2.UnitX;
                        else
                            fixnormal = Vector2.UnitX;

                        m.Normal = PhysicsMath.GetNormal(a.Position, a.Position) * fixnormal.X;
                        m.PenetrationDepth = xExtent;	
                    }
                    else {
                        if(m.Normal.Y < 0)
                             fixnormal = -Vector2.UnitY;
                        else
                            fixnormal= Vector2.UnitY;
                        m.Normal = PhysicsMath.GetNormal(a.Position, a.Position) * fixnormal.Y;
                        m.PenetrationDepth = yExtent;
                    }
                    m.AreColliding = true;
                    return m;
                }
            }
            m.AreColliding = false;
            return m;
        }

        //Collision resolver methods

        public static Manifold CheckCollision(Shape a, Shape b)
        {
            return collide((dynamic) a, (dynamic) b);
        }

        private static Manifold collide(AABB a, AABB b)
        {
            return AABBvsAABB(a, b);
        }

    }
}
