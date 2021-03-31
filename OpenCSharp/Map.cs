using GlmNet;
using Engine.render;
using System.IO;
using System.Text;
using System.Collections.Generic;

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
        
        /// <summary>
        /// Tile entity, used to draw, have the position etc...
        /// <para>(This is a entity just to reuse some code)</para>
        /// </summary>
        public Entity Block;
        
        /// <summary>
        /// Type of the Tile
        /// </summary>
        public TileType Type;
        
        /// <summary>
        /// Entitys inside a Tile
        /// </summary>
        public List<Entity> EntitysInside;
    }

    public class Map
    {
        public vec2 PlayerInitPos { get; protected set; }
        public vec2 FinishXBoundery { get; protected set; }
        public vec2 MapSize { get; protected set; }
        public Tile[,] TileMap { get; protected set; }
        public float TileSize { get; protected set; }

        public float Gravity { get; protected set; }

        public Map(uint w, uint h, string MapLayout, uint tileSize = 64, float gravity = 9.8f)
        {
            TileMap = new Tile[w, h];
            MapSize = new vec2(w, h);
            Gravity = gravity;
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
                                Size = new vec2(tileSize),

                            };
                            TileMap[j, hI].Block = tmp;
                            TileMap[j, hI].Block.TilePosition[0] = (uint)j;
                            TileMap[j, hI].Block.TilePosition[1] = (uint)i;
                            TileMap[j, hI].EntitysInside = new List<Entity>(5);
                            TileMap[j, hI].Block.OnAttach();
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
                            TileMap[j, hI].EntitysInside = new List<Entity>(5);
                            TileMap[j, hI].Block.OnAttach();
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
                            TileMap[j, hI].EntitysInside = new List<Entity>(5);
                            TileMap[j, hI].Block.OnAttach();
                            break;
                        case 'P':
                            PlayerInitPos = new(j * TileSize, hI * TileSize);
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
                            TileMap[j, hI].EntitysInside = new List<Entity>(5);
                            TileMap[j, hI].Block.OnAttach();
                            break;
                    }
                }
                hI++;
                lineh -= (int)w;
            }
        }

        public void Draw(double deltaTime)
        {
            //The entity inside Tiles dont Update
            //Maybe add this later
            for (int i = 0; i < MapSize.y; i++)
            {
                for (int j = 0; j < (int)MapSize.x; j++)
                {
                    ref var tile = ref TileMap[j, i];
                    
                    //FIXME: this if's
                    if (tile.Block.DrawIt && tile.Type != TileType.AIR)
                        Render2D.DrawQuad(tile.Block.Position, tile.Block.Size, tile.Block.STexture);
                    else
                        if (tile.Type == TileType.AIR)
                            if (tile.Block.DrawIt)
                                {
                                    Render2D.DrawQuad(tile.Block.Position, tile.Block.Size, new vec4(0.1f, 0.5f, 0.9f, 0.5f));
                                    tile.Block.DrawIt = false;
                                }
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
        /// <summary>
        /// Get the Tile in a Tile Position
        /// </summary>
        /// <param name="x">x Tile position</param>
        /// <param name="y">y Tile position </param>
        /// <returns></returns>
        public ref Tile WhatIsHere(uint x, uint y)
        {
            if(x > Width || x < 0 || y > Height || y < 0)
                return ref TileMap[0, 0];
            return ref TileMap[x, y];
        }

        /// <summary>
        /// Get a real position and return your TilePosition
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <returns></returns>
        public vec2 GetTilePos(float x, float y)
        {
            int xx = 0;
            int yy = 0;
            for(int i = 0; i < Width * TileSize; i += (int)TileSize)
            {
                if (i < x && i + TileSize <= x)
                    xx++;
                else
                    break;
            }

            for (int i = 0; i < Height * TileSize; i += (int)TileSize)
            {
                if (i < y && i + TileSize <= y)
                    yy++;
                else
                    break;
            }
            return new vec2(xx, yy);
        }

        /// <summary>
        /// Sugar sintax, you can do like "Map[x,y] and get the same as calling WhatIsHere method"
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns>Tile</returns>
        public ref Tile this[uint x, uint y]
        {
            get 
            {
                if (x > Width-1 || x < 0 || y > Height-1 || y < 0)
                    return ref TileMap[0, 0];
                return ref TileMap[x, y];
            }
        }

        //TODO: do something with this
        public bool IsDone()
        {
            return false;
        }

        /// <summary>
        /// Utility function to read a map from a text file
        /// </summary>
        /// <param name="path">Map file text</param>
        /// <returns></returns>
        static public string GetFromFile(string path)
        {
            try 
            {
                string tmp = "";
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    tmp = reader.ReadToEnd();
                }
                return tmp;
            }
            catch (EndOfStreamException ex)
            {
                throw new System.Exception("Exception catch trying to read: " + path + "\n Catch: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Cold not read from: " + path + "\n Catch: " + ex.Message);
            }
        }

    }
}
