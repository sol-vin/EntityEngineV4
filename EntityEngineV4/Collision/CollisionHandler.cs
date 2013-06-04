using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Collision
{
    public class CollisionHandler : Service
    {
        private List<Collision> _collideables;
        private HashSet<Pair> _pairs;

        public CollisionHandler(EntityState stateref) : base(stateref)
        {
            _collideables = new List<Collision>();
            _pairs = new HashSet<Pair>();
        }

        public override void Update(GameTime gt)
        {
            //Simple collision detection, for now ;D
            foreach (var pair in _pairs)
            {
                if (AABBvsAABB(pair))
                {
                    pair.A.OnCollision(pair.B);
                    pair.B.OnCollision(pair.A);
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
        }

        /// <summary>
        /// Compares bounding boxes using Seperating Axis Thereom. 
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public static bool AABBvsAABB(Pair pair)
        {
            //Simple normal
            Vector2 normal = pair.A.Position - pair.B.Position;

            //Calculate half widths
            float aExtent = pair.A.Bounds.X / 2f;
            float bExtent = pair.B.Bounds.X / 2f;

            //Calculate the overlap. 
            float xExtent = aExtent + bExtent - Math.Abs(normal.X);

            //If the overlap is greater than 0
            if (xExtent > 0)
            {
                //Calculate half widths
                aExtent = pair.A.Bounds.Y / 2f;
                bExtent = pair.B.Bounds.Y / 2f;

                //Calculate overlap
                float yExtent = aExtent + bExtent - Math.Abs(normal.Y);

                if (yExtent > 0)
                {
                    return true;
                }
            }
            return false;
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

        /// <summary>
        /// Compares the masks and checks to see if they should be allowed to form a pair.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Whether or not the the two objects should be paired</returns>
        public static bool CanObjectsPair(Collision a, Collision b)
        {
            return (a.GroupMask & b.GroupMask) > 0 ||  //Compare the group masks.
                (a.GroupMask & b.PairMask) > 0 ||  //Compare the pair masks to the group masks.
                (a.PairMask & b.GroupMask) > 0;
        }
    }
}
