using OpenTK.Mathematics;
using GlmNet;

namespace Engine.utils
{
    class Converter
    {
        static public Vector4 vec4ToVect4(vec4 vec)
        {
            return new Vector4(vec.x, vec.y, vec.z, vec.w);
        }
        static public vec4 Vect4toVec4(Vector4 vec)
        {
            return new vec4(vec.X, vec.Y, vec.Z, vec.W);
        }
    }
}
