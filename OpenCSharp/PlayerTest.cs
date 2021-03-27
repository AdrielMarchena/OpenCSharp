using GlmNet;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Engine.render;
using Engine.utils;
using System.Collections.Generic;

namespace OpenCSharp
{
    public class PlayerTest : Mob
    {
		protected readonly ParticlesSystem m_particleEmiter;
		protected ParticleProps m_particleProps;
		protected SpriteAnim m_animation;

		//protected Entity Line;
		//protected double LineAngle;
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
				prop.m_sprites[i] = Window.m_player_idle[i];
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

			//Line = new Entity();
			//Line.Size = new vec2(1);
			base.OnAttach();
        }
		/// <summary>
		/// Check Fro surronding Tiles
		/// </summary>
		public void CheckTiles()
        {
			ref Tile currentTile = ref Window.currentMap.WhatIsHere(0, 0);
			//Check surrond Tiles
			for (uint i = TilePosition[0] - 1; i < TilePosition[0] + 1; i++)
            {
				//Prevent out of bounds
				if (i < 0 || i >= Window.currentMap.Width)
					continue;

				for(uint j = TilePosition[1] - 1; j < TilePosition[1] + 1; j++)
                {
					//Prevent out of bounds
					if (j < 0 || j >= Window.currentMap.Height)
						continue;

					currentTile = ref Window.currentMap.WhatIsHere(i, j);


					if (currentTile.Type == TileType.AIR)
                    {
						currentTile.Block.DrawIt = true;
                    }
					else if (currentTile.Type == TileType.INVALID)
                    {
                    }
					else
						currentTile.Block.STexture = Window.m_subTexs[SubTex.Item];



					// Loop through all Entitys inside that Tile
					foreach (Entity e in currentTile.EntitysInside)
                    {
						//Skip colision with itself
						if (this.Equals(e))
							continue;
						if(Colision.RectColision(this, e))
							break;
                    }
					// Check colision with the Tile it self (if it's solid)
					if (currentTile.IsSolid)
                    {
						if (Colision.RectColision(this, currentTile.Block))
							return;
					}
				}
            }

		}

		public override void TileChanged(Tile newTile)
		{
			System.Console.WriteLine("changed Tiles");
			System.Console.WriteLine(Window.currentMap.GetTilePos(x, y));
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
			
			//for(int i = 0; i < 5; i++)
				//m_particleEmiter.Emit(m_particleProps);
			//m_particleEmiter.OnUpdate((float)e.Time);

			if (Position.x >= Window.screenSize.x / 2)
				Window.camera.GetCamera().SetPosition(new vec3(Position/ 1000,0.0f));
			if (Position.y >= Window.screenSize.y / 2)
				Window.camera.GetCamera().SetPosition(new vec3(Position / 1000, 0.0f));

			base.OnUpdate(keyboard,e);
            CheckTiles();
        }
		public override void OnRender(FrameEventArgs args)
        {
			m_particleEmiter.OnRender();

			/*var st = m_animation.RunAnim((float)args.Time,0.5f);
			if(st != null)
				Render2D.DrawQuad(Position, Size ,st);*/
			Render2D.DrawQuad(Position, Size, new vec4(1.0f));

            base.OnRender(args);
        }

        public override void RectColisionNotification(Entity cause, RectSide side = RectSide.NONE)
        {
			switch(side)
            {
				case RectSide.BOTTOM:
					y = cause.y + cause.h + 0.01f;
					Velocity.y = 0;
					break;
				case RectSide.TOP:
					y = cause.y - this.h + 0.01f;
					Velocity.y = 0;
					break;
				case RectSide.LEFT:
					x = cause.x - this.w - 0.01f;
					Velocity.x = 0;
					break;
				case RectSide.RIGHT:
					x = cause.x + cause.w - 0.01f;
					Velocity.x = 0;
					break;
			}

            base.RectColisionNotification(cause, side);
        }

    }
}
