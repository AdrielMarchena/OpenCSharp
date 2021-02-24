using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace Engine.render
{
    public class Texture
    {
        private readonly Int32 TextureID;
        private readonly float Wid = 0;
        private readonly float Hei = 0;
        private bool disposeValue = false;
        
        public Texture(string path)
        {
            TextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureID);


            Image<Rgba32> image = Image.Load<Rgba32>(path);

            image.Mutate(x => x.Flip(FlipMode.Vertical));

            var pixels = new List<byte>(4 * image.Width * image.Height);
            Wid = image.Width;
            Hei = image.Height;
            for (int y = 0; y < image.Height; y++)
            {
                var row = image.GetPixelRowSpan(y);

                for (int x = 0; x < image.Width; x++)
                {
                    pixels.Add(row[x].R);
                    pixels.Add(row[x].G);
                    pixels.Add(row[x].B);
                    pixels.Add(row[x].A);
                }
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                PixelInternalFormat.Rgba, 
                image.Width, 
                image.Height, 
                0, 
                PixelFormat.Rgba, 
                PixelType.UnsignedByte, 
                pixels.ToArray()
                );
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
        }
        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public Int32 textureID
        {
            get => TextureID;
        }
        public float Width
        {
            get => Wid;
        }
        public float Height
        {
            get => Hei;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposeValue)
            {
                GL.DeleteTexture(TextureID);
                disposeValue = true;
            }
        }
        ~Texture()
        {
            GL.DeleteTexture(TextureID);
        }
    }
}
