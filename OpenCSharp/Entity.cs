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

    public class Entity
    {
        public Texture Texture { get; set; }
        public SubTexture STexture { get; set; }
        public vec2 Position;
        public uint[] TilePosition { get; protected set; }
        public vec2 Size;
        public bool DrawIt;
        public bool UpdateIt;
        //protected bool ImGuiIt;

        public Entity(Texture texture) { Texture = texture; TilePosition = new uint[2]; }
        public Entity(SubTexture stexture) { STexture = stexture; TilePosition = new uint[2]; }

        public Entity() { }

        public virtual void OnAttach()
        {

        }

        /// <summary>
        /// Is called in the Window when rendering
        /// </summary>
        /// <param name="keyboard"></param>
        /// <param name="e"></param>
        public virtual void OnUpdate(KeyboardState keyboard, FrameEventArgs e)
        {
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
        /// <summary>
        /// Call back for rect colision
        /// </summary>
        /// <param name="cause">The object that cause the colision</param>
        /// <param name="side"> The side of the thing that the entity hit </param>
        public virtual void RectColisionNotification(Entity cause, RectSide side = RectSide.NONE)
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

        /*protected virtual void OnImGui()
        {
        }
        */
    }

    public class Mob : Entity 
    {
        public vec2 Velocity;
        public float SpawnLife { get; protected set; }
        public float Life { get; protected set; }
        public MobStates StateLife { get; protected set; }
        public MobStates PhysicState { get; protected set; }

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
        public override void OnUpdate(KeyboardState keyboard, FrameEventArgs e)
        {
            //TODO: Improve the way the current Tile is updated, such as the entitys inside the tile

            //Add this entity on the current Tile and save on Entity which tile he is in
            ref Tile cT = ref Window.currentMap[TilePosition[0], TilePosition[1]];
            if (cT.Type != TileType.INVALID)
            {
                if (this != null)
                    cT.EntitysInside.Add(this);
            }

            // calculate new Tile position if Tile change
            var newPos = Window.currentMap.GetTilePos(Position.x, Position.y);

            //If the Current tile change, set the new Tile position and delete this instance from the old tile
            if (!newPos.Equals(cT))
            {
                TilePosition[0] = (uint)newPos.x;
                TilePosition[1] = (uint)newPos.y;
                //Remove Entity from previous Tile
                cT.EntitysInside.Remove(this);
            }
            
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
