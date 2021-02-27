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
        public vec2 Position;
        public vec2 Size;
        public bool DrawIt;
        public bool UpdateIt;
        //protected bool ImGuiIt;

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
        private float SpawnLife { get; }
        public float Life { get; protected set; }
        protected readonly Texture m_texture;
        public MobStates state { get; protected set; }

        public Mob(Texture texture, float spawnLife = 30)
        {
            m_texture = texture;
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
            state = MobStates.DEAD;
            DrawIt = false;
            UpdateIt = false;
        }
        public virtual void Spawn()
        {
            Life = SpawnLife;
            state = MobStates.ALIVE;
            DrawIt = true;
            UpdateIt = true;
        }

        public virtual void Damage(float damage)
        {
            if (Life > 0)
                Life -= damage;
        }

    }
}
