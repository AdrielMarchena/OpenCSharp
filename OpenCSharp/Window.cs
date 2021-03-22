using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
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
    /// Index for Subtextures (strange i know)
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
        Exit,
        FreeCamera
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
        static public vec2 screenSize { get; protected set; }
        
        /// <summary>
        /// Mouse event, is updated each frame
        /// </summary>
        static public MouseState mouse { get; protected set; }
        
        /// <summary>
        /// Keyboard events, is updated each frame
        /// </summary>
        static public KeyboardState keyboard { get; protected set; }

        static public readonly ResourceManager<SubTex, SubTexture> m_subTexs = new ResourceManager<SubTex, SubTexture>();
        static public readonly ResourceManager<int, SubTexture> m_player_idle = new ResourceManager<int, SubTexture>();
        static public readonly ResourceManager<string, Texture> m_textures = new ResourceManager<string, Texture>();
        static public readonly ResourceManager<string, Text> m_fonts = new ResourceManager<string, Text>();
        static public readonly ResourceManager<string, SoundPlayer> m_snds = new ResourceManager<string, SoundPlayer>();
        static public readonly ResourceManager<string, Map> m_maps = new ResourceManager<string, Map>();
        static public GameState gameState;
        
        static public float m_updateTime { get; protected set; }
        static public float m_renderTime { get; protected set; }

        /// <summary>
        /// Entity list,
        /// </summary>
        private readonly List<Entity> m_entities;
        
        /// <summary>
        /// Player instance
        /// </summary>
        static private readonly SoundPlayer mPlayer = new SoundPlayer();
        
        /// <summary>
        /// The current Map
        /// </summary>
        static public Map currentMap { get; protected set; }

        /// <summary>
        /// Create all those SubTex from the TexMap
        /// </summary>
        static private void MontSubText()
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

            MapT = m_textures.GetResource("player_idle");
            ImageSize = new vec2(MapT.Width, MapT.Height);
            SpriteSize = new vec2(64);
            for(int j = 0; j < 5; j++)
            {
                tmp = SubTexture.CreateFromCoords(MapT.textureID, ImageSize, new vec2(j, 0), SpriteSize);
                m_player_idle[j] = tmp;
            }

        }
        
        /// <summary>
        /// Load Resources Here
        /// </summary>
        static private void Load_resources()
        {
            m_textures.AddResource("test", new Texture("tex/Test.png"));
            m_textures.AddResource("MapTextures", new Texture("tex/Map.png"));
            m_textures.AddResource("player_idle", new Texture("tex/player_idle.png"));

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
            MontSubText();
            m_entities = new List<Entity>();

            string layout = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAWAAAAAAAAAAAAAAAAAAAW" +
                            "WAAAAAAWWWWAAAAAAAAAWAAAAAAWWWWAAAAAAAAA" +
                            "WAAAAAAAAAAAAAAAAAAAWAAAAAAAAAAAAAAAAAAA" +
                            "WAAAAAAAAAAAAAAAAAAAWAAAAAAAAAAAAAAAAAAA" +
                            "GGGGGGGGGGGGGGAAAGGGGGGGGGGGGGGGGGAAAGGG" +
                            "GAAGGAAAAGGAAAAAAAAGGAAGGAAAAGGAAAAAAAAG" +
                            "WGGGGGGGGGGGGGGGGGGGWGGGGGGGGGGGGGGGGGGG" +
                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
                            "AAAAAAAAAAAAAAAAAAAWAAAAAAAAAAAAAAAAAAAW" +
                            "WAAAAAAWWWWAAAAAAAAAWAAAAAAWWWWAAAAAAAAA" +
                            "WAAAAAAAAAAAAAAAAAAAWAAAAAAAAAAAAAAAAAAA" +
                            "WAAAAAAAAAAAAAAAAAAAWAAAAAAAAAAAAAAAAAAA" +
                            "GGGGGGGGGGGGGGAAAGGGGGGGGGGGGGGGGGAAAGGG" +
                            "GAAGGAAAAGGAAAAAAAAGGAAGGAAAAGGAAAAAAAAG" +
                            "WGGGGGGGGGGGGGGGGGGGWGGGGGGGGGGGGGGGGGGG";

            m_maps["1-1"] = new Map(40, 20, layout, 64u);
            currentMap = m_maps["1-1"];

            string layout2 = "AAAAAAAAAAAAAAAAAAAA" +
                             "AAAAAAAAAAAAAAAAAAAA" +
                             "AAAAAAAAAAAAAAAAAAAA" +
                             "AAAAAAAAAAAAAAAAAAAW" +
                             "WAAAAAAWWWWAAAAAAAAA" +
                             "WAAAAAAAAAAAAAAAAAAA" +
                             "GGGGGGGGAAAAAAAAAAAA" +
                             "GGGGGGGGGGGGGGAAAGGG" +
                             "GGGGGGGGAAAGGGGGGGGG" +
                             "GGGGGGGGGGGGGGGGGGGG";

            m_maps["1-2"] = new Map(20, 10, layout2, 64u);

            gameState = GameState.Pause;

            screenSize = new vec2(1024.0f, 576.0f);
        }

        static Window() { camera = new OrthographicCameraController(1, true); }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            foreach (Entity en in m_entities)
                en.OnMouseDown(e);
            base.OnMouseDown(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            foreach (Entity en in m_entities)
                en.OnKeyDown(e);
            base.OnKeyDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            foreach (Entity en in m_entities)
                en.OnMouseUp(e);
            base.OnMouseDown(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            foreach (Entity en in m_entities)
                en.OnKeyUp(e);
            base.OnKeyDown(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            screenSize = new vec2(e.Width, e.Height);
            GL.Viewport(0, 0, e.Width, e.Height);
            camera.OnResize(e.Width, e.Height);
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
            //TODO: Understand why this is the GetViewProjectionMatrix turn Quad's in Rectangles (When the AspectRatio is not 1)
            shader.SetUniformMat4("u_ViewProj", camera.GetCamera().GetViewProjectionMatrixArray());
            shader.SetUniformMat4("u_Transform", glm.ortho(0,screenSize.x ,0,screenSize.y,-1.0f,100.0f).to_array());
            
            TextRender.Init();
            TextRender.SetProjView(mat4.identity().to_array());
            TextRender.SetTransform(glm.ortho(0, screenSize.x, 0, screenSize.y, -1.0f, 100.0f).to_array());

            PlayerTest player = new PlayerTest(m_textures.PopResource("test"));
            m_entities.Add(player);
            foreach(Entity e in m_entities)
            {
                e.OnAttach();
            }

            //m_snds.GetResource("music").Play();
            gameState = GameState.Running;
            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if(KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            m_updateTime = (float)e.Time;

            mouse = MouseState;
            keyboard = KeyboardState;

            camera.OnUpdate(KeyboardState, e.Time);
            
            shader.Bind();
            shader.SetUniformMat4("u_ViewProj", camera.GetCamera().GetViewProjectionMatrixArray());
            shader.SetUniformMat4("u_Transform", glm.ortho(0, screenSize.x, 0, screenSize.y, -1.0f, 100.0f).to_array());

            TextRender.SetTransform(glm.ortho(0, screenSize.x, 0, screenSize.y, -1.0f, 100.0f).to_array());

            if(gameState != GameState.Exit || gameState != GameState.Pause)
                foreach (Entity en in m_entities)
                {
                    if(en.UpdateIt)
                        en.OnUpdate(KeyboardState,e);
                }

            if (keyboard.IsKeyDown(Keys.C))
                currentMap = m_maps["1-2"];
            if (keyboard.IsKeyDown(Keys.U))
                Console.WriteLine("Camera pos X: " + camera.GetCamera().position.x + " Y: " + camera.GetCamera().position.y);

                base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            m_renderTime = (float)args.Time;
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shader.Bind();
            Render2D.BeginBatch();

            //Draws here
            
            vec4[] colors = new vec4[4] {
                 new vec4(0.0f,0.6f, 0.9f,1.0f),
                 new vec4(0.0f,0.6f, 0.9f,1.0f),
                 new vec4(0.7f,0.9f,0.9f,1.0f),
                 new vec4(0.7f,0.9f,0.9f,1.0f),
                };


            Render2D.DrawQuad(new vec2(0),new vec2(currentMap.Width * currentMap.TileSize, currentMap.Height * currentMap.TileSize), colors);
            m_fonts["Arial"].RenderText("Oi teste", 10.0f, screenSize.y - 25.0f, 0.5f, new vec3(0.1f));
            currentMap.Draw();
            foreach (Entity en in m_entities)
            {
                if (en.DrawIt)
                    en.OnRender(args);
            }
            //vec2 PlayerPos = new vec2(1);
            //double Distance = Math.Sqrt(Math.Pow(PlayerPos.x - 50.0f,2) + Math.Pow(50.0f - PlayerPos.y,2));

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
