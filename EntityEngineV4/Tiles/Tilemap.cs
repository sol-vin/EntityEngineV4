using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityEngineV4.Components;
using EntityEngineV4.Engine;
using EntityEngineV4.Tiles.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntityEngineV4.Tiles
{
    public class Tilemap : Entity
    {
        //TODO: Finish this
        public Body Body;
        public TilemapRender Render;
        public TilemapData Data;

        public Tilemap(IComponent parent, string name, Texture2D tileTexture, Tile[,] tiles, Point tileSize) : base(parent, name)
        {
            Body = new Body(this, "Body");
            Data = new TilemapData(tiles, tileSize, Body);
            Render = new TilemapRender(this, "TilemapRender", tileTexture ,Data);
        }


        public Tilemap(IComponent parent, string name, Texture2D tileTexture, Point size, Point tileSize)
            : base(parent, name)
        {
            Body = new Body(this, "Body");
            Data = new TilemapData(size, tileSize, Body);
            Render = new TilemapRender(this, "TilemapRender", tileTexture, Data);
        }

        public void SetData(Point size, Point tileSize)
        {
            Data = new TilemapData(size, tileSize, Body);
        }

    }
}
