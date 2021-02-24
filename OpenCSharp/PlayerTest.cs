using GlmNet;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Engine.render;
namespace OpenCSharp
{
    public class PlayerTest : Mob
    {

		private readonly ParticlesSystem m_particleEmiter;
		private ParticleProps m_particleProps;

		public PlayerTest(Texture texture) :base(texture) 
		{
			m_particleEmiter = new ParticlesSystem();
			m_particleProps = new ParticleProps();
		}
		public override void OnAttach()
        {
            Position = new vec2(50.0f, 50.0f);
            Size = new vec2(50.0f, 50.0f);
			Velocity = new vec2(0);
			DrawIt = UpdateIt = true;

			m_particleProps = ParticleProps.Effect2;
			m_particleProps.LifeTime = 2.0f;
			m_particleProps.SizeBegin = 2.0f;
			base.OnAttach();
        }

		public override void OnUpdate(KeyboardState keyboard, FrameEventArgs e)
        {
			float deltaTime = (float)e.Time;
			if (keyboard.IsKeyDown(Keys.Right))
				if (Velocity.x < 100.0f)
					Velocity.x += 25.0f * deltaTime;
			if (keyboard.IsKeyDown(Keys.Left))
				if (Velocity.x > -100.0f)
					Velocity.x -= 25.0f * deltaTime;
			if (keyboard.IsKeyDown(Keys.Up))
				if (Velocity.y < 100.0f)
					Velocity.y += 25.0f * deltaTime;
			if (keyboard.IsKeyDown(Keys.Down))
				if (Velocity.y > -100.0f)
					Velocity.y -= 25.0f * deltaTime;

			if (Velocity.x > 0.0f)
			{
				Velocity.x -= 5.0f * deltaTime;
				if (Velocity.x < 0)
					Velocity.x = 0.0f;
			}

			if (Velocity.x < 0.0f)
			{
				Velocity.x += 5.0f * deltaTime;
				if (Velocity.x > 0)
					Velocity.x = 0.0f;
			}
			if (Velocity.y > 0.0f)
			{
				Velocity.y -= 5.0f * deltaTime;
				if (Velocity.y <= 0)
					Velocity.y = 0.0f;
			}
			if (Velocity.y < 0.0f)
			{
				Velocity.y += 5.0f * deltaTime;
				if (Velocity.y >= 0)
					Velocity.y = 0.0f;
			}
			Position += Velocity;
			m_particleProps.Position = (Position + (Size/2)) - (m_particleProps.SizeBegin / 2);
			
			for(int i = 0; i < 5; i++)
				m_particleEmiter.Emit(m_particleProps);
			m_particleEmiter.OnUpdate((float)e.Time);
			base.OnUpdate(keyboard,e);
        }
		public override void OnRender(FrameEventArgs args)
        {
			m_particleEmiter.OnRender();
			Render2D.DrawQuad(Position, Size ,m_texture.textureID);

            base.OnRender(args);
        }

    }
}
