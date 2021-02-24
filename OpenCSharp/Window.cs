using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using Engine.render;
using Engine.camera;
using System;
using System.Collections.Generic;
using GlmNet;
using Engine;
using Engine.resources;
namespace OpenCSharp
{

    public class Window : GameWindow
    {
        private Shader shader;
        public static readonly OrthographicCameraController camera;
        static private vec2 ScreenSize;

        static private readonly ResourceManager<uint, SubTexture> m_subTexs = new ResourceManager<uint, SubTexture>();
        static private readonly ResourceManager<string, Texture> m_textures = new ResourceManager<string, Texture>();
        static private readonly ResourceManager<string, Text> m_fonts = new ResourceManager<string, Text>();
        unsafe private List<Entity> m_entities;

        private readonly ParticlesSystem emiter;
        private ParticleProps props;

        static private void Load_resources()
        {
            m_textures.AddResouce("test", new Texture("tex/Test.png"));
            m_fonts.AddResouce("FreeSans", new Text("fonts/FreeSans.ttf"));
        }

        public Window(GameWindowSettings gameWindowSettings,NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings,nativeWindowSettings)
        {

            Load_resources();
            m_entities = new List<Entity>();
            
            ScreenSize = new vec2(1024.0f, 576.0f);

            var texture = m_textures.GetResource("test");
            m_subTexs.AddResouce(0,SubTexture.CreateFromCoords(texture.textureID, new vec2(texture.Width, texture.Height), new vec2(1, 0), new vec2(64, 64)));
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
            }

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
            Render2D.DrawQuad(new vec2(10.0f, 10.0f), new vec2(50.0f, 50.0f), new vec4(0.9f, 0.9f, 0.9f, 1.0f));

            foreach (Entity en in m_entities)
            {
                if (en.DrawIt)
                    en.OnRender(args);
                PlayerPos = en.Position;
            }

            double Distance = Math.Sqrt(Math.Pow(PlayerPos.x - 50.0f,2) + Math.Pow(50.0f - PlayerPos.y,2));
            m_fonts.GetResource("FreeSans").RenderText("distance is: " + Distance.ToString(), 300.0f, 375.0f, 0.5f, new vec3(0.2f, 0.5f, 0.8f));
            m_fonts.GetResource("FreeSans").RenderText("Time Elapsed: " + args.Time, 50.0f, ScreenSize.y - 50.0f, 0.5f, new vec3(0.2f, 0.5f, 0.8f));
            m_fonts.GetResource("FreeSans").RenderText("FPS: " + (1000.0f / args.Time), 50.0f, ScreenSize.y - 100.0f, 0.5f, new vec3(0.2f, 0.5f, 0.8f));


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
            Render2D.ShutDown();
            TextRender.ShutDown();
            base.OnUnload();
        }
    }
}
