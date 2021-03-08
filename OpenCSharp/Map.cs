using GlmNet;
using Engine.render;

namespace OpenCSharp
{
    public enum TileType : int
    {
        INVALID = '0',
        OUTBOUND = 'O',
        AIR = 'A',
        GROUND = 'G',
        WALL = 'W'
    }

    public struct Tile
    {
        public bool IsSolid;
        public Entity Block;
        public TileType Type;
    }

    unsafe public class Map
    {
        public vec2 PlayerInitPos { get; protected set; }
        public vec2 MapSize { get; protected set; }
        public Tile[,] TileMap { get; protected set; }
        public float TileSize { get; protected set; }
        public Map(uint w, uint h, string MapLayout, uint tileSize = 64)
        {
            TileMap = new Tile[w, h];
            MapSize = new vec2(w, h);
            TileSize = tileSize;
            var A = Window.m_subTexs[SubTex.Invalid];
            var G = Window.m_subTexs[SubTex.Ground1];
            var W = Window.m_subTexs[SubTex.Wall1];
            int lineh = ((int)h * (int)w) - (int)w;
            int hI = 0;
            Entity tmp;
            for (int i = (int)h; i > 0; i--)
            {
                string line = MapLayout.Substring(lineh, (int)w);

                for (int j = 0; j < w ; j++)
                {
                    switch (line[j])
                    {
                        case 'G':
                            TileMap[j, hI].IsSolid = true;
                            TileMap[j, hI].Type = TileType.GROUND;
                            tmp = new Entity(G)
                            {
                                DrawIt = true,
                                UpdateIt = true,
                                Position = new vec2(j * tileSize, hI * tileSize),
                                Size = new vec2(tileSize)
                            };
                            TileMap[j, hI].Block = tmp;
                        break;
                        case 'W':
                            TileMap[j, hI].IsSolid = true;
                            TileMap[j, hI].Type = TileType.WALL;
                            tmp = new Entity(W)
                            {
                                DrawIt = true,
                                UpdateIt = true,
                                Position = new vec2(j * tileSize, hI * tileSize),
                                Size = new vec2(tileSize)
                            };
                            TileMap[j, hI].Block = tmp;
                        break;
                        case 'A':

                            TileMap[j, hI].IsSolid = false;
                            TileMap[j, hI].Type = TileType.AIR;
                            tmp = new Entity(A)
                            {
                                DrawIt = false,
                                UpdateIt = true,
                                Position = new vec2(j * tileSize, hI * tileSize),
                                Size = new vec2(tileSize)
                            };
                            TileMap[j, hI].Block = tmp;

                        break;
                    }
                }
                hI++;
                lineh -= (int)w;
            }
        }
        public void Draw()
        {

            for (int i = 0; i < MapSize.y; i++)
            {
                for (int j = 0; j < (int)MapSize.x; j++)
                {
                    var tile = TileMap[j, i];
                    if (!tile.Block.DrawIt)
                        continue;
                    Render2D.DrawQuad(tile.Block.Position, tile.Block.Size, tile.Block.STexture);
                }
            }
        }

        public float Width
        {
            get => MapSize.x;
        }
        public float Height
        {
            get => MapSize.y;
        }
        public Tile WhatIsHere(uint w, uint h)
        {
            return TileMap[w, h];
        }

        public Tile this[uint w, uint h]
        {
            get { return TileMap[w,h]; }
            protected set { TileMap[w,h] = value; }
        }

        public bool IsDone()
        {
            return false;
        }

    }
}
