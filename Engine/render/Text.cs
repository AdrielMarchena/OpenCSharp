using System;
using GlmNet;
using SharpFont;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
namespace Engine.render
{
    public struct Character
    {
        private Int32 TextureID;
        private vec2 Size;
        private vec2 Bearing;
        private Int32 advance;
        
        public Character(int textureID, vec2 size, vec2 bearing, int Advance)
        {
            TextureID = textureID;
            Size = size;
            Bearing = bearing;
            advance = Advance;
        }

        public Int32 textureID { get => TextureID; }
        public vec2 size { get => Size; }
        public vec2 bearing { get => Bearing; }
        public Int32 Advance { get => advance; }
    }

    public class Text
    {
        private readonly Dictionary<uint, Character> Characters;
        /// <summary>
        /// 65.0f is the default, above this the Gap gets bigger
        /// </summary>
        public UInt32 Gap { get; set; }
        public Text(string fontPath = "fonts/arial.ttf")
        {
            Gap = 65;
            Characters = new Dictionary<uint, Character>();
            Library library = new Library();
            
            Face face = new Face(library, fontPath);
            face.SetPixelSizes(0, 48);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (uint i = 0; i < 255; i++)
            {
                face.LoadChar(i, LoadFlags.Render, LoadTarget.Normal);

                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.R8,
                    face.Glyph.Bitmap.Width,
                    face.Glyph.Bitmap.Rows,
                    0,
                    PixelFormat.Red,
                    PixelType.UnsignedByte,
                    face.Glyph.Bitmap.Buffer
                    );
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

                Characters[i] = new Character(
                    texture, 
                    new vec2(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows), 
                    new vec2(face.Glyph.BitmapLeft, face.Glyph.BitmapTop), 
                    (int)face.Glyph.Advance.X
                    );
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            face.Dispose();
            library.Dispose();
        }

        public void Dispose()
        {
            foreach(var d in Characters)
            {
                GL.DeleteTexture(d.Value.textureID);
            }
        }

        public void RenderText(string text,float x, float y, float scale,vec3 color)
        {
            TextRender.BeginBatch();
            float c_Char = x;
            foreach(char c in text)
            {
                if (!Characters.ContainsKey(c))
                    continue;

                Character ch = Characters[c];

                float xpos = c_Char + ch.bearing.x * scale;
                float ypos = y - (ch.size.y - ch.bearing.y) * scale;

                float w = ch.size.x * scale;
                float h = ch.size.y * scale;
                
                TextRender.DrawText(new vec2( xpos,ypos + h), new vec2( w,-h ) ,ch.textureID, color);
                
                // TODO: Do a decent bit shift and remove this Gap multiplication
                c_Char += ((ch.Advance * Gap) >> 6) * scale;
            }
            TextRender.EndBatch();
            TextRender.Flush();
        }
    }
}
