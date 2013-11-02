using System;
using EntityEngineV4.Components;
using EntityEngineV4.Components.Rendering;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Tiles.Components
{
    public class TilemapRender : Render
    {
        private readonly Tile[,] _tiles;

        public Point Size { get { return new Point(_tiles.GetUpperBound(0) + 1, _tiles.GetUpperBound(1) + 1); } }

        public Point TileSize;
        public Vector2 Scale = Vector2.One;

        public TilemapRender(Entity parent, string name, Texture2D texture, Point size, Point tileSize)
            : base(parent, name)
        {
            Texture = texture;
            _tiles = new Tile[size.X, size.Y];
            SetAllTiles(Tile.EMPTY);
            TileSize = tileSize;

            //Dependencies
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }

        public TilemapRender(Entity parent, string name, Texture2D texture, Tile[,] tiles, Point tileSize)
            : base(parent, name)
        {
            Texture = texture;
            _tiles = tiles;
            TileSize = tileSize;
            //Dependencies
            AddLinkType(DEPENDENCY_BODY, typeof(Body));
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x > Size.X || y > Size.Y) throw new Exception("x or y is out of bounds!");
            return _tiles[x, y];
        }

        public Tile GetTile(Point p)
        {
            return GetTile(p.X, p.Y);
        }

        public Tile GetTileByPosition(Vector2 position)
        {
            position /= Scale;
            int x = (int)Math.Floor(position.X/TileSize.X);
            int y = (int)Math.Floor(position.Y / TileSize.Y);

            if (x > Size.X || y > Size.Y)
                return null;

            return GetTile(x, y);
        }

        public void SetTile(int x, int y, Tile t)
        {
            if (x < 0 || y < 0 || x > Size.X || y > Size.Y) throw new Exception("x or y is out of bounds!");
            _tiles[x, y] = t;
        }

        public void SetTile(int x, int y, short index)
        {
            if (x < 0 || y < 0 || x > Size.X || y > Size.Y) throw new Exception("x or y is out of bounds!");
            _tiles[x, y].Index = index;
        }

        public void SetTile(Point p, Tile t)
        {
            SetTile(p.X, p.Y, t);
        }

        public Tile[,] CloneTiles()
        {
            Tile[,] output = new Tile[Size.X, Size.Y];
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    output[x, y] = _tiles[x, y].Clone();
                }
            }

            return output;
        }

        public Rectangle GetTileBoundingRect(int x, int y)
        {
            return new Rectangle
                {
                    X = (int)(x * TileSize.X * Scale.X) + (int)GetDependency<Body>(DEPENDENCY_BODY).Position.X,
                    Y = (int)(y * TileSize.Y * Scale.Y) + (int)GetDependency<Body>(DEPENDENCY_BODY).Position.Y,
                    Width = (int)(TileSize.X * Scale.X),
                    Height = (int)(TileSize.Y * Scale.Y)
                };
        }

        public Rectangle GetTileBoundingRect(Point p)
        {
            return GetTileBoundingRect(p.X, p.Y);
        }

        public void SetAllTiles(short index)
        {
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    SetTile(x,y, new Tile(index));
                }
            }
        }

        public void SetAllTiles(Tile tile)
        {
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    SetTile(x, y, tile.Clone());
                }
            }
        }
        public Texture2D Texture { get; private set; }

        public int Columns { get { return (int)(Texture.Width / TileSize.X); } }

        public int Rows { get { return (int)(Texture.Height / TileSize.Y); } }

        public override Rectangle DrawRect
        {
            get
            {
                return new Rectangle(
                    (int)GetDependency<Body>(DEPENDENCY_BODY).X, 
                    (int)GetDependency<Body>(DEPENDENCY_BODY).Y, 
                    (int)(GetDependency<Body>(DEPENDENCY_BODY).Width * Scale.X), 
                    (int)(GetDependency<Body>(DEPENDENCY_BODY).Height * Scale.Y));
            }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
            GetDependency<Body>(DEPENDENCY_BODY).Width = TileSize.X * (_tiles.GetUpperBound(0) + 1);
            GetDependency<Body>(DEPENDENCY_BODY).Height = TileSize.Y * (_tiles.GetUpperBound(1) + 1);

        }

        public override void Draw(SpriteBatch sb)
        {
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    Tile t = GetTile(x, y);
                    if (t.Index <= Tile.EMPTY) continue; //Continue if the tile is empty

                    sb.Draw(Texture, GetTileBoundingRect(x, y), GetSourceRect(x, y, t.Index), t.Color, 0,
                            Vector2.Zero, t.Flip, Layer);
                }
            }
        }

        public Rectangle GetSourceRect(int x, int y, int index)
        {
            Rectangle source = new Rectangle();
            if (index > Columns)
            {
                source.X = ((index % Columns) * TileSize.X);
                source.Y = ((int)Math.Floor(index / (float)Columns) * TileSize.Y);
            }
            else
            {
                source.X = index * TileSize.X;
                source.Y = 0;
            }
            source.Width = TileSize.X;
            source.Height = TileSize.Y;
            return source;
        }

        public Tile[,] GetTiles()
        {
            return _tiles;
        }

        //Dependencies
        public const int DEPENDENCY_BODY = 0;
    }
}