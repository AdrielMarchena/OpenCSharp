using OpenTK.Graphics.OpenGL;

namespace Engine.render
{
    public static class TextureUnitE
    {
        /// <summary>
        /// Util function to get the TextureUnit by a number
        /// </summary>
        /// <param name="i">The desired TextureUnit</param>
        /// <returns>TextureUnit finded, Texture0 if i >= 31</returns>
        public static TextureUnit Add(this TextureUnit t,int i)
        {
            if(i <= 31)
                return TextureUnit.Texture0 + i;
            return TextureUnit.Texture0;
        }

    }
}
