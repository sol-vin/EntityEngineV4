using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using EntityEngineV4.Input;
using EntityEngineV4.Tiles.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Tiles
{
    public class Tilemap : Node
    {
        //TODO: Finish Tilemap

        public Body Body;
        public TilemapRender Render;

        public event Tile.EventHandler TileSelected;

        public Point Size { get { return Render.Size; } }

        public float Width { get { return Render.TileSize.X * Render.Size.X; } }
        public float Height { get { return Render.TileSize.Y * Render.Size.Y; } }

        public Tilemap(Node parent, string name, Texture2D tileTexture, Tile[,] tiles, Point tileSize)
            : base(parent, name)
        {
            Body = new Body(this, "Body");
            Render = new TilemapRender(this, "TilemapRender", tileTexture, tiles, tileSize);
            Render.LinkDependency(TilemapRender.DEPENDENCY_BODY, Body);
        }

        public Tilemap(Node parent, string name, Texture2D tileTexture, Point size, Point tileSize)
            : base(parent, name)
        {
            Body = new Body(this, "Body");
            Render = new TilemapRender(this, "TilemapRender", tileTexture, size, tileSize);
            Render.LinkDependency(TilemapRender.DEPENDENCY_BODY, Body);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (MouseService.Cursor.Released())
            {
                //Get a tile, if it's null dont do anything
                Tile t = GetTileByPosition(MouseService.Cursor.Position);
                if (t.Index != Tile.EMPTY && TileSelected != null)
                {
                    TileSelected(t);
                }
            }
        }

        public Tile GetTileByPosition(Vector2 position)
        {
            if (!Render.DrawRect.Contains(new Point((int)position.X, (int)position.Y)))
                return new Tile(Tile.EMPTY);
            else
            {
                return Render.GetTileByPosition(position - Render.GetDependency<Body>(TilemapRender.DEPENDENCY_BODY).Position);
            }
        }

        public Tile GetTile(int x, int y)
        {
            return Render.GetTile(x, y);
        }

        public Tile[,] GetTiles()
        {
            return Render.GetTiles();
        }

        public Tile[,] CloneTiles()
        {
            return Render.CloneTiles();
        }

        public void SetTile(int x, int y, Tile t)
        {
            Render.SetTile(x, y, t);
        }

        public void SetTile(int x, int y, short index)
        {
            Render.SetTile(x, y, index);
        }

        public void SetTile(Point p, Tile t)
        {
            SetTile(p.X, p.Y, t);
        }

        public void SetAllTiles(short index)
        {
            Render.SetAllTiles(index);
        }

        public void SetAllTiles(Tile tile)
        {
            Render.SetAllTiles(tile);
        }
    }
}