using System;
using EntityEngineV4.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Tiles.Components
{
    public class TilemapRender : Component
    {
        private TilemapData _tilemapData;

        public Texture2D Texture { get; private set; }

        public int Columns { get { return (int)(Texture.Width / _tilemapData.TileSize.X); } }

        public int Rows { get { return (int)(Texture.Height / _tilemapData.TileSize.Y); } }

        public TilemapRender(Entity parent, string name, Texture2D texture, TilemapData tilemapData)
            : base(parent, name)
        {
            _tilemapData = tilemapData;
            Texture = texture;
        }

        public override void Draw(SpriteBatch sb)
        {
            for (int x = 0; x < _tilemapData.Size.X; x++)
            {
                for (int y = 0; y < _tilemapData.Size.Y; y++)
                {
                    Tile t = _tilemapData.GetTile(x, y);
                    if (t.Index <= Tile.EMPTY) continue; //Continue if the tile is empty

                    sb.Draw(Texture, _tilemapData.GetTileBoundingRect(x, y), GetSourceRect(x, y, t.Index), t.Color, 0, Vector2.Zero, t.Flip, t.Layer);
                }
            }
        }

        public Rectangle GetSourceRect(int x, int y, int index)
        {
            Rectangle source = new Rectangle();
            if (index > Columns)
            {
                source.X = ((index % Columns) * _tilemapData.TileSize.X);
                source.Y = ((int)Math.Floor(index / (float)Columns) * _tilemapData.TileSize.Y);
            }
            else
            {
                source.X = index * _tilemapData.TileSize.X;
                source.Y = 0;
            }
            source.Width = _tilemapData.TileSize.X;
            source.Height = _tilemapData.TileSize.Y;
            return source;
        }
    }
}