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
using Engine.audio;

namespace OpenCSharp
{
    /// <summary>
    /// Index for Subtextures(Yes, this is pretty static i know)
    /// </summary>
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

    public enum GameState
    {
        Running,
        Pause,
        Exit
    }

    public class Window : GameWindow

    {
        private Shader shader;
        /// <summary>
        /// Global camera
        /// </summary>
        public static readonly OrthographicCameraController camera;
        /// <summary>
        /// It's updated when the screen is resized
        /// </summary>
        static public vec2 ScreenSize { get; protected set; }
        static public MouseState mouse { get; protected set; }
        static public KeyboardState keyboard { get; protected set; }

        /// <summary>
        /// Static SubTextures from a Atlas Texture
        /// </summary>
        static public readonly ResourceManager<SubTex, SubTexture> m_subTexs = new ResourceManager<SubTex, SubTexture>();
        static public readonly ResourceManager<string, Texture> m_textures = new ResourceManager<string, Texture>();
        static public readonly ResourceManager<string, Text> m_fonts = new ResourceManager<string, Text>();
        static public readonly ResourceManager<string, SoundPlayer> m_snds = new ResourceManager<string, SoundPlayer>();
        /// <summary>
        /// Entity list,
        /// </summary>
        private readonly List<Entity> m_entities;

        static private readonly SoundPlayer mPlayer = new SoundPlayer();

        private readonly ParticlesSystem emiter;
        private ParticleProps props;

        private Map map;

        /// <summary>
        /// Create all those SubTex from the TexMap
        /// </summary>
        static private void montSubText()
        {
            var MapT = m_textures.GetResource("MapTextures");
            vec2 ImageSize = new vec2(MapT.Width, MapT.Height);
            vec2 SpriteSize = new vec2(64);

            //Default subtex for invalid itens
            SubTexture tmp = SubTexture.CreateFromCoords(MapT.textureID, ImageSize, new vec2(0, 0), new vec2(1,1));
            m_subTexs[SubTex.Invalid] = tmp;
            //Get sub texts from the Major tex here
            int i = 1;
            for(int r = 3; r >= 0; r--)
            {
                for(int c = 0; c <= 3;c++)
                {
                    tmp = SubTexture.CreateFromCoords(MapT.textureID, ImageSize, new vec2(c , r), SpriteSize);
                    m_subTexs[(SubTex)i] = tmp;
                    i++;
                }
            }
        }
        /// <summary>
        /// Load Resources Here
        /// </summary>
        static private void Load_resources()
        {
            m_textures.AddResource("test", new Texture("tex/Test.png"));
            m_textures.AddResource("MapTextures", new Texture("tex/Map.png"));

            m_fonts.AddResource("Arial", new Text("fonts/arial.ttf"));
            //Play on My headphone, the default is 0
            mPlayer.Open("snd/Requiem.mp3", SoundPlayer.Devices[1]);
            mPlayer.Volume = 2;
            m_snds.AddResource("music", mPlayer);
        }

        public Window(GameWindowSettings gameWindowSettings,NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings,nativeWindowSettings)
        {
            Load_resources();
            montSubText();
            m_entities = new List<Entity>();

            string layout = "AAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAW" +
                            "WAAAAAAWWWWAAAAAAAAA" +
                            "WAAAAAAAAAAAAAAAAAAA" +
                            "WAAAAAAAAAAAAAAAAAAA" +
                            "GGGGGGGGGGGGGGAAAGGG" +
                            "GAAGGAAAAGGAAAAAAAAG" +
                            "WGGGGGGGGGGGGGGGGGGG";

            map = new Map(20, 10, layout);

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
            TextRender.SetProjView(mat4.identity().to_array());
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
                Close();

            mouse = MouseState;
            keyboard = KeyboardState;

            camera.OnUpdate(KeyboardState, e.Time);
            
            shader.Bind();
            shader.SetUniformMat4("u_ViewProj", camera.GetCamera().GetViewProjectionMatrixArray());
            shader.SetUniformMat4("u_Transform", glm.ortho(0, ScreenSize.x, 0, ScreenSize.y, -1.0f, 100.0f).to_array());

            TextRender.SetTransform(glm.ortho(0, ScreenSize.x, 0, ScreenSize.y, -1.0f, 100.0f).to_array());

            foreach (Entity en in m_entities)
            {
                if(en.UpdateIt)
                    en.OnUpdate(KeyboardState,e);
            }

            emiter.OnUpdate((float)e.Time);

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shader.Bind();
            Render2D.BeginBatch();

            //Draws here

            map.Draw();
            foreach (Entity en in m_entities)
            {
                if (en.DrawIt)
                    en.OnRender(args);
            }
            //vec2 PlayerPos = new vec2(1);
            //double Distance = Math.Sqrt(Math.Pow(PlayerPos.x - 50.0f,2) + Math.Pow(50.0f - PlayerPos.y,2));

            emiter.OnRender();

            Render2D.EndBatch();
            Render2D.Flush();

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            camera.OnMouseScrolled(e.OffsetY);
            base.OnMouseWheel(e);
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
