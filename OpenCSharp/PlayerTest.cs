using GlmNet;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Engine.render;
using System;
using System.Collections.Generic;
using System.Linq;
using Engine.utils;

namespace OpenCSharp
{
    public class PlayerTest : Mob
    {
		protected readonly ParticlesSystem m_particleEmiter;
		protected ParticleProps m_particleProps;
		protected SpriteAnim m_animation;

		protected vec4 CBoxColor = new(1.0f,0.0f,0.0f,0.0f);

		List<Action> renderThis = new List<Action>();

		//protected Entity Line;
		//protected double LineAngle;
		public float StopVelocity { get; protected set; } = 5 * 100;
		public float AcVelocity { get; protected set; } = 20 * 100;
		public vec2 TermVelocity { get; protected set; } = new(5 * 1000,5 * 5000);
		public vec2 NegTermVelocity { get; protected set; } = new(-5 * 1000);

		public float JumpForce { get; protected set; } = 10 * 1000;

		public bool onGround { get; protected set; }
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
			Position = Window.currentMap.PlayerInitPos;
			Size = new vec2(50.0f, 50.0f);
			Velocity = new vec2(0);
			DrawIt = UpdateIt = true;

			m_particleProps = ParticleProps.Effect2;
			m_particleProps.LifeTime = 2.0f;
			m_particleProps.SizeBegin = 10.0f;
			m_particleProps.SizeVariation = 0.1f;


			//Line = new Entity();
			//Line.Size = new vec2(1);
			base.OnAttach();
        }
		/// <summary>
		/// Check Fro surronding Tiles
		/// </summary>
		public void CheckTiles(float deltaTime)
        {
			ref Tile currentTile = ref Window.currentMap.WhatIsHere(0, 0);

			vec2 cp = new(), cn = new();
			float ct = 0.0f;
			List<Tuple<Tile,float>> tiles = new();
			//Check surrond Tiles
			for (uint i = TilePosition[0] - 1; i < TilePosition[0] + 2; i++)
            {
				//Prevent out of bounds
				if (i < 0 || i >= Window.currentMap.Width)
					continue;

				for(uint j = TilePosition[1] - 1; j < TilePosition[1] + 2; j++)
                {
					//Prevent out of bounds
					if (j < 0 || j >= Window.currentMap.Height)
						continue;

					currentTile = ref Window.currentMap.WhatIsHere(i, j);

					#if DEBUG
						//Give color to a Air Block
						if (currentTile.Type == TileType.AIR)
						{
							currentTile.Block.DrawIt = true;
						}
						else if (currentTile.Type == TileType.INVALID)
						{
						}
						else
							currentTile.Block.STexture = Window.m_subTexs[SubTex.Item];

					#endif

					// Loop through all Entitys inside that Tile
					foreach (Entity e in currentTile.EntitysInside)
                    {
						//Skip colision with itself
						if (this.Equals(e))
							continue;
						Colision.RectColision(this, e);
                    }

                    // Check colision with the Tile it self (if it's solid)
					if (currentTile.IsSolid)
					{
						if (Colision.DynamicRectVsRect(this, currentTile.Block.CBox,ref cp,ref cn,ref ct,deltaTime))
						{
							tiles.Add(new Tuple<Tile,float>(currentTile,ct));
						}
					}
				}
            }// End Loop


			//Order by the closest Tile
			var Ttiles = tiles.OrderBy(a => a.Item2).ToList();

			foreach(var pair in Ttiles)
            {
				if (Colision.DynamicRectVsRect(this, pair.Item1.Block.CBox, ref cp, ref cn, ref ct, deltaTime))
                {
					if (cn.y == 1)
						onGround = true;
					Velocity += cn * (new vec2(MathF.Abs(Velocity.x), MathF.Abs(Velocity.y)) * (1 - ct));
					
				}
			}
		}

		public override void TileChanged(Tile newTile)
		{
			#if DEBUG
			Console.BackgroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine("changed Tiles");
			Console.WriteLine($"XT{TilePosition[0]} YT{TilePosition[1]}");
			Console.WriteLine($"CX{cx} CY{cy} CW{cw} CH{ch}");
			Console.WriteLine($"X{x} Y{y} W{w} H{h}");
			#endif
		}

		public override void OnUpdate(KeyboardState keyboard, FrameEventArgs e)
        {
			PressH = HorizontalMove.NONE;
			PressV = VerticalMove.NONE;
			float deltaTime = (float)e.Time;
			if (keyboard.IsKeyDown(Keys.Right))
            {
				if (Velocity.x < TermVelocity.x)
					Velocity.x += AcVelocity * deltaTime;
				PressH = HorizontalMove.RIGHT;
			}

			if (keyboard.IsKeyDown(Keys.Left))
            {
				if (Velocity.x > NegTermVelocity.x)
					Velocity.x -= AcVelocity * deltaTime;
				PressH = HorizontalMove.LEFT;
			}

			if(keyboard.IsKeyDown(Keys.Space))
            {
				Velocity.y = JumpForce * deltaTime;
            }

			/*if (keyboard.IsKeyDown(Keys.Up))
            {
				if (Velocity.y < TermVelocity)
					Velocity.y += AcVelocity * deltaTime;
				PressV = VerticalMove.UP;
			}
				
			if (keyboard.IsKeyDown(Keys.Down))
            {
				if (Velocity.y > NegTermVelocity)
					Velocity.y -= AcVelocity * deltaTime;
				PressV = VerticalMove.DOWN;
			}*/
			
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

			/*if (Velocity.y > 0.0f)
			{
				Velocity.y -= StopVelocity * deltaTime;
				if (Velocity.y < 0)
					Velocity.y = 0.0f;
			}

			if (Velocity.y < 0.0f)
			{
				Velocity.y += StopVelocity * deltaTime;
				if (Velocity.y > 0)
					Velocity.y = 0.0f;
			}*/

			Velocity.y -= Window.currentMap.Gravity;

			base.UpdateTile(mx, my);
			//call OnUpdate firt, to update some important things first
			base.OnUpdate(keyboard, e);
			CBox.size *= 1.5f;
			//Check tiles, check colision after call base.Update, the 
			CheckTiles(deltaTime);

			if (Velocity.y > TermVelocity.y)
				Velocity.y = TermVelocity.y;
			if (Velocity.y < NegTermVelocity.y)
				Velocity.y = NegTermVelocity.y;

			if (Velocity.x > TermVelocity.x)
				Velocity.x = TermVelocity.x;
			if (Velocity.x < NegTermVelocity.x)
				Velocity.x = NegTermVelocity.x;

			Position += Velocity * deltaTime;

			if (Window.mouse.IsButtonDown(MouseButton.Button1))
            {
				for (int i = 0; i < 5; i++)
                {
					m_particleProps.Position = Window.MousePos;
					m_particleEmiter.Emit(m_particleProps);
				}
				renderThis.Add(delegate () { Render2D.DrawLine(new vec2(mx, my), Window.MousePos, 2, new vec4(1.0f)); });
			}

			m_particleEmiter.OnUpdate((float)e.Time);
		}
		public override void OnRender(FrameEventArgs args)
        {
			m_particleEmiter.OnRender();

			var st = m_animation.RunAnim((float)args.Time,0.5f);
			if (st != null)
				Render2D.DrawQuad(Position, Size * 1.5f, st);
			else
				Render2D.DrawQuad(Position, Size, Window.m_subTexs[SubTex.Invalid]);

			if (Colision.PointVsRect(Window.MousePos, this))
				CBoxColor = new(1.0f,0.0f,0.0f,1.0f);

			Render2D.DrawLineQuad(new vec2(cx, cy), new vec2(cw, ch), 2, CBoxColor);

			foreach(Action a in renderThis)
            {
				a.Invoke();
            }

			CBoxColor = new(0.0f, 1.0f, 1.0f, 1.0f);
			base.OnRender(args);
			renderThis.Clear();
        }
    }
}
