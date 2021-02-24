using GlmNet;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Engine.render;
namespace OpenCSharp
{
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
        protected float Life;
        protected readonly Texture m_texture;

        public Mob(Texture texture)
        {
            m_texture = texture;
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
    }
}
