using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using Engine.utils;
namespace Engine.utils
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

    public static class ListExtra
    {
        public static void Resize<T>(this List<T> list, int sz, T c)
        {
            int cur = list.Count;
            if (sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if (sz > cur)
            {
                if (sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                    list.Capacity = sz;
                list.AddRange(Enumerable.Repeat(c, sz - cur));
            }
        }
        public static void Resize<T>(this List<T> list, int sz) where T : new()
        {
            Resize(list, sz, new T());
        }
    }

}
