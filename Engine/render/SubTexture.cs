using System;
using GlmNet;
namespace Engine.render
{
    public class SubTexture
    {
        private Int32 TextureID = 0;
        private vec2[] TexCoords;

        public SubTexture(Int32 textureID,vec2 min,vec2 max)
        {
            TextureID = textureID;
            if(TextureID != 0)
            {
                TexCoords = new vec2[4];
                TexCoords[0] = new vec2( min.x,min.y );
                TexCoords[1] = new vec2( max.x,min.y );
                TexCoords[2] = new vec2( max.x,max.y );
                TexCoords[3] = new vec2( min.x,max.y );
            }
        }

        static public SubTexture CreateFromCoords(Int32 textureID,vec2 size, vec2 coords, vec2 spriteSize)
        {
            float sheetWidth = size.x, sheetHeight = size.y;

            vec2 min = new vec2( (coords.x * spriteSize.x) / sheetWidth, (coords.y * spriteSize.y) / sheetHeight );
            vec2 max = new vec2( ((coords.x + 1) * spriteSize.x) / sheetWidth, ((coords.y + 1) * spriteSize.y) / sheetHeight );

            return new SubTexture(textureID, min, max);
        }

        public Int32 textureId
        {
            get => TextureID;
        }
        public vec2[] texCoord
        {
            get => TexCoords;
        }

}
}
