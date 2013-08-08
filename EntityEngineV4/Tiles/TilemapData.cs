using System;
using EntityEngineV4.Components;
using Microsoft.Xna.Framework;

namespace EntityEngineV4.Tiles
{
    public class TilemapData
    {
        private readonly Tile[,] _tiles;

        public Point Size { get; private set; }

        public Point TileSize;
        public Vector2 Scale = Vector2.One;

        //Dependencies
        private Body _body;

        public TilemapData(Point size, Point tileSize, Body body)
        {
            _tiles = new Tile[size.X, size.Y];
            TileSize = tileSize;
            _body = body;
            Size = size;
        }

        public TilemapData(Tile[,] tiles, Point tileSize, Body body)
        {
            _tiles = tiles;
            TileSize = tileSize;
            _body = body;
            Size = new Point(tiles.GetUpperBound(0) + 1, tiles.GetUpperBound(1) + 1);
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

        public void SetTile(int x, int y, Tile t)
        {
            if (x < 0 || y < 0 || x > Size.X || y > Size.Y) throw new Exception("x or y is out of bounds!");
            _tiles[x, y] = t;
        }

        public void SetTile(Point p, Tile t)
        {
            SetTile(p.X, p.Y, t);
        }

        public Tile[,] GetClone()
        {
            return (Tile[,])_tiles.Clone();
        }

        public Rectangle GetTileBoundingRect(int x, int y)
        {
            return new Rectangle
                {
                    X = (int)(x * TileSize.X * Scale.X) + (int)_body.Position.X,
                    Y = (int)(y * TileSize.Y * Scale.Y) + (int)_body.Position.Y,
                    Width = (int)(TileSize.X * Scale.X),
                    Height = (int)(TileSize.Y * Scale.Y)
                };
        }

        public Rectangle GetTileBoundingRect(Point p)
        {
            return GetTileBoundingRect(p.X, p.Y);
        }
    }
}