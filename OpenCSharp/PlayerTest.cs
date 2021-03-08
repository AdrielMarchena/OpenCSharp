using GlmNet;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Engine.render;

namespace OpenCSharp
{
    public class PlayerTest : Mob
    {
		protected readonly ParticlesSystem m_particleEmiter;
		protected ParticleProps m_particleProps;
		protected SpriteAnim m_animation;
		protected Entity Line;
		protected double LineAngle;
		public float StopVelocity { get; protected set; } = 10.0f;
		public float AcVelocity { get; protected set; } = 25.0f;
		public float TermVelocity { get; protected set; } = 100.0f;
		public float NegTermVelocity { get; protected set; } = -100.0f;

		public PlayerTest(Texture texture) :base(texture) 
		{
			m_particleEmiter = new ParticlesSystem();
			m_particleProps = new ParticleProps();

			string[] names =
			{
				"player_idle_animation"
			};

            SpriteAnimProp prop = new SpriteAnimProp
            {
                threshold = 5.0f,
                m_sprites = new SubTexture[5]
            };

            for (int i = 0; i < prop.m_sprites.Length; i++)
            {
				prop.m_sprites[i] = Window.m_subTexs[(SubTex)i+1];
            }

			SpriteAnimProp[] props =
			{
				prop
			};

			m_animation = new SpriteAnim(SpriteAnim.CreateSpriteDict(names, props));
			m_animation.SetAnim("player_idle_animation");
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

			Line = new Entity();
			Line.Size = new vec2(1);
			base.OnAttach();
        }

		public override void OnUpdate(KeyboardState keyboard, FrameEventArgs e)
        {
			float deltaTime = (float)e.Time;
			if (keyboard.IsKeyDown(Keys.Right))
				if (Velocity.x < TermVelocity)
					Velocity.x += AcVelocity * deltaTime;
			if (keyboard.IsKeyDown(Keys.Left))
				if (Velocity.x > NegTermVelocity)
					Velocity.x -= AcVelocity * deltaTime;
			if (keyboard.IsKeyDown(Keys.Up))
				if (Velocity.y < TermVelocity)
					Velocity.y += AcVelocity * deltaTime;
			if (keyboard.IsKeyDown(Keys.Down))
				if (Velocity.y > NegTermVelocity)
					Velocity.y -= AcVelocity * deltaTime;

			if (Velocity.x > 0.0f)
			{
				Velocity.x -= StopVelocity * deltaTime;
				if (Velocity.x < 0)
					Velocity.x = 0.0f;
			}

			if (Velocity.x < 0.0f)
			{
				Velocity.x += StopVelocity * deltaTime;
				if (Velocity.x > 0)
					Velocity.x = 0.0f;
			}
			if (Velocity.y > 0.0f)
			{
				Velocity.y -= StopVelocity * deltaTime;
				if (Velocity.y <= 0)
					Velocity.y = 0.0f;
			}
			if (Velocity.y < 0.0f)
			{
				Velocity.y += StopVelocity * deltaTime;
				if (Velocity.y >= 0)
					Velocity.y = 0.0f;
			}
			Position += Velocity;
			m_particleProps.Position = (Position + (Size/2)) - (m_particleProps.SizeBegin / 2);
			
			for(int i = 0; i < 5; i++)
				m_particleEmiter.Emit(m_particleProps);
			m_particleEmiter.OnUpdate((float)e.Time);

			if (Position.x >= Window.screenSize.x / 2)
				Window.camera.GetCamera().SetPosition(new vec3(Position/ 1000,0.0f));
			if (Position.y >= Window.screenSize.y / 2)
				Window.camera.GetCamera().SetPosition(new vec3(Position / 1000, 0.0f));

			/*if(Window.mouse.IsButtonDown(MouseButton.Button1))
            {
				Line.Position = Position;
				vec2 mpos = new vec2(Window.mouse.Position.X, Window.mouse.Position.X);
				double Distance = Math.Sqrt(Math.Pow(mpos.x - Line.Position.x,2) + Math.Pow(Line.Position.y - mpos.y, 2));

				double delta_x = mpos.x - Line.Position.x;
				double delta_y = mpos.y - Line.Position.y;
				LineAngle = (float)Math.Atan2(delta_y, delta_x);

				//LineAngle = Math.Atan2(mpos.y - Line.Position.y, mpos.x - Line.Position.x) * 180 / Math.PI;

				Line.Size.x = 1;
				Line.Size.y = (float)Distance;
			}*/

			base.OnUpdate(keyboard,e);
        }
		public override void OnRender(FrameEventArgs args)
        {
			
			m_particleEmiter.OnRender();

			var st = m_animation.RunAnim((float)args.Time,0.5f);
			if(st != null)
				Render2D.DrawQuad(Position, Size ,st);

            base.OnRender(args);
        }

    }
}
