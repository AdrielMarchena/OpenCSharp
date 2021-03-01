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

    public class Entity
    {
        public Texture Texture { get; protected set; }
        public SubTexture STexture { get; protected set; }
        public vec2 Position;
        public vec2 Size;
        public bool DrawIt;
        public bool UpdateIt;
        //protected bool ImGuiIt;

        public Entity(Texture texture) { Texture = texture; }
        public Entity(SubTexture stexture) { STexture = stexture; }

        public Entity() { }

        public virtual void OnAttach()
        {

        }

        public virtual void OnUpdate(KeyboardState keyboard, FrameEventArgs e)
        {
        }
        public virtual void OnRender(FrameEventArgs args)
        {
        }

        /*protected virtual void OnImGui()
        {
        }
        */
    }

    public class Mob : Entity 
    {
        protected vec2 Velocity;
        protected mat3 TilePosition;
        private float SpawnLife { get; }
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
