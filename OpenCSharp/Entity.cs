using GlmNet;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Engine.render;
namespace OpenCSharp
{
    public enum MobStates : byte
    {
        ALIVE = 0,
        DEAD,
        FALLING,
        ON_GROUND
    }

    public enum RectSide : byte
    {
        NONE,
        BOTTOM,
        LEFT,
        RIGHT,
        TOP
    }

    public enum VerticalMove : byte
    {
        NONE,
        UP,
        DOWN
    }

    public enum HorizontalMove : byte
    {
        NONE,
        RIGHT,
        LEFT
    }

    public class Rect
    {
        public vec2 pos = new();
        public vec2 size = new();
    }

    public class Entity
    {
        public Texture Texture { get; set; }
        public SubTexture STexture { get; set; }
        public vec2 Position;
        public vec2 Size;
        public vec2 Velocity;

        public uint[] TilePosition { get; protected set; }
        public bool DrawIt;
        public bool UpdateIt;
        //protected bool ImGuiIt;

        protected bool SyncCBox_Position = true;

        public Rect CBox { get; protected set; } = new();
        public Entity(Texture texture) { Texture = texture; TilePosition = new uint[2]; }
        public Entity(SubTexture stexture) { STexture = stexture; TilePosition = new uint[2]; }

        public Entity() { }

        public virtual void OnAttach()
        {
            if(SyncCBox_Position)
            {
                var r = CBox;
                r.pos.x = x;
                r.pos.y = y;
                r.size.x = w;
                r.size.y = h;
                CBox = r;
            }
        }

        /// <summary>
        /// Is called in the Window when rendering
        /// </summary>
        /// <param name="keyboard"></param>
        /// <param name="e"></param>
        public virtual void OnUpdate(KeyboardState keyboard, FrameEventArgs e)
        {
            if (SyncCBox_Position)
            {
                var r = CBox;
                r.pos.x = x;
                r.pos.y = y;
                r.size.x = w;
                r.size.y = h;
                CBox = r;
            }
        }

        /// <summary>
        /// Is called in the Window when rendering
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnRender(FrameEventArgs args)
        {
        }

        /// <summary>
        /// Is called in the Window when mouse button is down
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseDown(MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Is called in the Window when A key on keyboard is Down
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
        }

        /// <summary>
        /// Is called in the Window when mouse button is up (release)
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnMouseUp(MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Is called in the Window when A key on keyboard is Up (release)
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnKeyUp(KeyboardKeyEventArgs e)
        {
        }
        protected virtual void UpdateTile(float x, float y)
        {
        }

        /// <summary>
        /// Call back for rect colision
        /// </summary>
        /// <param name="cause">The object that cause the colision</param>
        /// <param name="side"> The side of the thing that the entity hit </param>
        public virtual void RectColisionNotification(Entity cause, RectSide side = RectSide.NONE)
        {
        }

        public virtual void TileChanged(Tile newTile)
        {
        }

        /// <summary>
        /// Same as Position.x
        /// </summary>
        public float x
        {
            get => Position.x;
            set => Position.x = value;
        }
        /// <summary>
        /// Same as Position.y
        /// </summary>
        public float y
        {
            get => Position.y;
            set => Position.y = value;

        }

        /// <summary>
        /// Same as Size.x
        /// </summary>
        public float w
        {
            get => Size.x;
            set => Size.x = value;
        }
        /// <summary>
        /// Same as size.y
        /// </summary>
        public float h
        {
            get => Size.y;
            set => Size.y = value;
        }

        /// <summary>
        /// Return the mid x position (x + (w / 2))
        /// </summary>
        public float mx
        {
            get => x + (w / 2);
        }

        /// <summary>
        /// Return the mid y position (y + (h / 2))
        /// </summary>
        public float my
        {
            get => y + (h / 2);
        }

        /// <summary>
        /// Same as CBox.pos.x
        /// </summary>
        public float cx
        {
            get => CBox.pos.x;
            set => CBox.pos.x = value;
        }
        /// <summary>
        /// Same as CBox.pos.y
        /// </summary>
        public float cy
        {
            get => CBox.pos.y;
            set => CBox.pos.y = value;

        }

        /// <summary>
        /// Same as CBox.size.x
        /// </summary>
        public float cw
        {
            get => CBox.size.x;
            set => CBox.size.x = value;
        }
        /// <summary>
        /// Same as CBox.size.y
        /// </summary>
        public float ch
        {
            get => CBox.size.y;
            set => CBox.size.y = value;
        }

        /*protected virtual void OnImGui()
        {
        }
        */
    }

    public class Mob : Entity 
    {
        public float SpawnLife { get; protected set; }
        public float Life { get; protected set; }
        public MobStates StateLife { get; protected set; }
        public MobStates PhysicState { get; protected set; }

        public VerticalMove VMove { get; protected set; }

        public HorizontalMove HMove { get; protected set; }

        public HorizontalMove PressH { get; protected set; }
        public VerticalMove PressV { get; protected set; }


        public Mob(Texture texture, float spawnLife = 30)
            :base(texture)
        {
            SpawnLife = spawnLife;
        }
        public Mob(SubTexture stexture, float spawnLife = 30)
            :base(stexture)
        {
            SpawnLife = spawnLife;
        }

        public override void OnAttach()
        {
            base.OnAttach();
        }

        protected override void UpdateTile(float x, float y)
        {
            //Add this entity on the current Tile and save on Entity which tile he is in
            ref Tile cT = ref Window.currentMap[TilePosition[0], TilePosition[1]];

            // Get current tile position (from middle real position)
            var newPos = Window.currentMap.GetTilePos(x, y);

            //If the tile change, set the new Tile position and delete this instance from the old tile
            if (TilePosition[0] != newPos.x || TilePosition[1] != newPos.y)
            {
                TilePosition[0] = (uint)newPos.x;
                TilePosition[1] = (uint)newPos.y;

                //Remove Entity from previous Tile
                cT.EntitysInside.Remove(this);

                //Get current Tile and let's dive inside
                cT = ref Window.currentMap[TilePosition[0], TilePosition[1]];
                if (cT.Type != TileType.INVALID)
                {
                    if (this != null)
                        cT.EntitysInside.Add(this);
                }
                TileChanged(cT);
            }
        }

        public override void OnUpdate(KeyboardState keyboard, FrameEventArgs e)
        {
            
            //Update the Vertical and horizontal movement of th Mob 
            if (Velocity.x > 0.0) HMove = HorizontalMove.RIGHT;
            else if (Velocity.x < 0.0) HMove = HorizontalMove.LEFT;
            else HMove = HorizontalMove.NONE;

            if (Velocity.y > 0.0) VMove = VerticalMove.UP;
            else if (Velocity.y < 0.0) VMove = VerticalMove.DOWN;
            else VMove = VerticalMove.NONE;

            base.OnUpdate(keyboard,e);
        }
        public override void OnRender(FrameEventArgs args)
        {
            base.OnRender(args);
        }

        public virtual void Die()
        {
            Life = 0;
            StateLife = MobStates.DEAD;
        }
        public virtual void Spawn()
        {
            Life = SpawnLife;
            StateLife = MobStates.ALIVE;
            DrawIt = true;
            UpdateIt = true;
        }

        public virtual void Damage(float damage)
        {
        }

    }
}
