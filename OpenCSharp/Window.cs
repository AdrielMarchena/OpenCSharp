using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Engine.render;
using Engine.camera;
using System;
using System.Collections.Generic;
using GlmNet;
using Engine;
using Engine.resources;
using Engine.audio;

namespace OpenCSharp
{
    public enum SubTex : byte
    {
        Invalid = 0,
        Player,
        Enemy,
        Item,
        Ground1,
        Ground2,
        Ground3,
        Wall1,
        Wall2,
        Wall3
    }
    public class Window : GameWindow

    {
        private Shader shader;
        public static readonly OrthographicCameraController camera;
        static private vec2 ScreenSize;

        static private readonly ResourceManager<SubTex, SubTexture> m_subTexs = new ResourceManager<SubTex, SubTexture>();
        static private readonly ResourceManager<string, Texture> m_textures = new ResourceManager<string, Texture>();
        static private readonly ResourceManager<string, Text> m_fonts = new ResourceManager<string, Text>();
        static private readonly ResourceManager<string, SoundPlayer> m_snds = new ResourceManager<string, SoundPlayer>();

        private readonly unsafe List<Entity> m_entities;

        static private readonly SoundPlayer mPlayer = new SoundPlayer();

        private readonly ParticlesSystem emiter;
        private ParticleProps props;

        static private void montSubText()
        {
            var Map = m_textures.GetResource("MapTextures");
            vec2 ImageSize = new vec2(Map.Width, Map.Height);
            vec2 SpriteSize = new vec2(64);
            
            //Default subtex for invalid itens
            SubTexture tmp = SubTexture.CreateFromCoords(Map.textureID, ImageSize, new vec2(0, 0), new vec2(1,1));
            m_subTexs[SubTex.Invalid] = tmp;
            //Get sub texts from the Major tex here
            int i = 1;
            for(int r = 3; r >= 0; r--)
            {
                
                for(int c = 0; c <= 3;c++)
                {
                    tmp = SubTexture.CreateFromCoords(Map.textureID, ImageSize, new vec2(c , r), SpriteSize);
                    m_subTexs[(SubTex)i] = tmp;
                    i++;
                }
            }

           /*SubTexture tmp = SubTexture.CreateFromCoords(Map.textureID, ImageSize, new vec2(0,3), SpriteSize);
            m_subTexs.AddResouce("player_tex", tmp);

            tmp = SubTexture.CreateFromCoords(Map.textureID, ImageSize, new vec2(1,3) ,SpriteSize);
            m_subTexs.AddResouce("enemy_tex", tmp);

            tmp = SubTexture.CreateFromCoords(Map.textureID, ImageSize, new vec2(2, 3), SpriteSize);
            m_subTexs.AddResouce("item_tex", tmp);

            tmp = SubTexture.CreateFromCoords(Map.textureID, ImageSize, new vec2(3, 3), SpriteSize);
            m_subTexs.AddResouce("ground_tex", tmp);

            tmp = SubTexture.CreateFromCoords(Map.textureID, ImageSize, new vec2(0, 2), SpriteSize);
            m_subTexs.AddResouce("ground2_tex", tmp);

            tmp = SubTexture.CreateFromCoords(Map.textureID, ImageSize, new vec2(1, 2), SpriteSize);
            m_subTexs.AddResouce("ground3_tex", tmp);*/

        }

        static private void Load_resources()
        {
            m_textures.AddResource("test", new Texture("tex/Test.png"));
            m_textures.AddResource("MapTextures", new Texture("tex/Map.png"));

            m_fonts.AddResource("Arial", new Text("fonts/arial.ttf"));
            //Play on My headphone, the default is 0
            mPlayer.Open("snd/Requiem.mp3", SoundPlayer.Devices[1]);
            m_snds.AddResource("music", mPlayer);
        }

        public Window(GameWindowSettings gameWindowSettings,NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings,nativeWindowSettings)
        {
            Load_resources();
            montSubText();
            m_entities = new List<Entity>();
            
            ScreenSize = new vec2(1024.0f, 576.0f);

            props = ParticleProps.Effect2;
            props.Gravity = 0.0f;
            emiter = new ParticlesSystem();
        }

        static Window() { camera = new OrthographicCameraController(1, true); }

        protected override void OnResize(ResizeEventArgs e)
        {
            ScreenSize = new vec2(e.Width, e.Height);
            GL.Viewport(0, 0, e.Width, e.Height);
            //camera.OnResize(e.Width, e.Height);
            camera.OnResize(1, 1);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            float norm = 1.0f / 256.0f;
            GL.ClearColor(norm * 24.0f, norm * 24.0f, norm * 24.0f, 1.0f);

            shader = new Shader("shader/shader.vert", "shader/shader.frag");
            shader.Bind();

            //Some stuff for textures Slots
            var loc = GL.GetUniformLocation(shader.shaderID, "u_Textures");
            Int32[] samplers = new Int32[Render2D.MaxTextureUnits];
            for (int i = 0; i < Render2D.MaxTextureUnits; i++)
                samplers[i] = i;
            GL.Uniform1(loc, Render2D.MaxTextureUnits, samplers);

            Render2D.Init();
            //TODO: Understand why this is the GetViewProjectionMatrix turn Quad's in Rectangles
            shader.SetUniformMat4("u_ViewProj", camera.GetCamera().GetViewProjectionMatrixArray());
            shader.SetUniformMat4("u_Transform", glm.ortho(0,ScreenSize.x ,0,ScreenSize.y,-1.0f,100.0f).to_array());
            
            TextRender.Init();
            TextRender.SetProjView(camera.GetCamera().GetViewProjectionMatrixArray());
            TextRender.SetTransform(glm.ortho(0, ScreenSize.x, 0, ScreenSize.y, -1.0f, 100.0f).to_array());

            PlayerTest player = new PlayerTest(m_textures.PopResource("test"));
            m_entities.Add(player);
            foreach(Entity e in m_entities)
            {
                e.OnAttach();
            }

            m_snds.GetResource("music").Play();


            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if(KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if(MouseState.IsButtonDown(MouseButton.Button1))
            {
                props.Position.y = camera.GetCamera().GetPosition().y - (MousePosition.Y - ScreenSize.y);
                props.Position.x = MousePosition.X;
                emiter.Emit(props);
                m_snds.GetResource("music").Play();
            }else m_snds.GetResource("music").Pause();

            camera.OnUpdate(KeyboardState, e.Time);
            
            shader.Bind();
            shader.SetUniformMat4("u_ViewProj", camera.GetCamera().GetViewProjectionMatrixArray());
            shader.SetUniformMat4("u_Transform", glm.ortho(0, ScreenSize.x, 0, ScreenSize.y, -1.0f, 100.0f).to_array());

            TextRender.SetProjView(mat4.identity().to_array());
            TextRender.SetTransform(glm.ortho(0, ScreenSize.x, 0, ScreenSize.y, -1.0f, 100.0f).to_array());


            foreach (Entity en in m_entities)
            {
                if(en.UpdateIt)
                    en.OnUpdate(KeyboardState,e);
            }

            emiter.OnUpdate((float)e.Time);

            base.OnUpdateFrame(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            camera.OnMouseScrolled(e.OffsetY);
            base.OnMouseWheel(e);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shader.Bind();
            Render2D.BeginBatch();
            //Draws here
            vec2 PlayerPos = new vec2(1);
            float pos = 150.0f;

            m_fonts["Arial"].RenderText("Player: ", pos, 80.0f, 0.5f, new vec3(0.8f, 0.8f, 0.9f));
            Render2D.DrawQuad(new vec2(pos, 10.0f), new vec2(50.0f, 50.0f), m_subTexs[SubTex.Player]);
            pos += 100.0f;

            m_fonts["Arial"].RenderText("Enemy: ", pos, 80, 0.5f, new vec3(0.8f, 0.8f, 0.9f));
            Render2D.DrawQuad(new vec2(pos, 10.0f), new vec2(50.0f, 50.0f), m_subTexs[SubTex.Enemy]);
            pos += 100.0f;

            m_fonts["Arial"].RenderText("Item: ", pos, 80, 0.5f, new vec3(0.8f, 0.8f, 0.9f));
            Render2D.DrawQuad(new vec2(pos, 10.0f), new vec2(50.0f, 50.0f), m_subTexs[SubTex.Item]);
            pos += 100.0f;

            m_fonts["Arial"].RenderText("Ground: ", pos , 80, 0.5f, new vec3(0.8f, 0.8f, 0.9f));
            Render2D.DrawQuad(new vec2(pos, 10.0f), new vec2(50.0f, 50.0f), m_subTexs[SubTex.Ground1]);
            pos += 100.0f;


            foreach (Entity en in m_entities)
            {
                if (en.DrawIt)
                    en.OnRender(args);
                PlayerPos = en.Position;
            }
            
            double Distance = Math.Sqrt(Math.Pow(PlayerPos.x - 50.0f,2) + Math.Pow(50.0f - PlayerPos.y,2));
            double fps = RenderTime + UpdateTime / 2 * 1000;

            m_fonts.GetResource("Arial").RenderText("distance is: " + Distance.ToString(), 300.0f, 375.0f, 0.5f, new vec3( 0.8f, 0.8f,0.9f));
            m_fonts.GetResource("Arial").RenderText("Time Elapsed: " + args.Time, 50.0f, ScreenSize.y - 50.0f, 0.5f, new vec3(0.2f, 0.5f, 0.8f));
            m_fonts.GetResource("Arial").RenderText("FPS: " + fps, 50.0f, ScreenSize.y - 100.0f, 0.5f, new vec3(0.2f, 0.5f, 0.8f));
            
            emiter.OnRender();

            Render2D.EndBatch();
            Render2D.Flush();

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected void PushEntity(ref Entity ent)
        {
            ent.OnAttach();
            m_entities.Add(ent);
        }
        protected void PushEntity(ref Entity[] ent)
        {
            foreach (Entity e in ent)
            {
                e.OnAttach();
            }
            m_entities.AddRange(ent);
        }
        protected override void OnUnload()
        {
            shader.Dispose();
            mPlayer.Dispose();
            Render2D.ShutDown();
            TextRender.ShutDown();
            base.OnUnload();
        }
    }
}
