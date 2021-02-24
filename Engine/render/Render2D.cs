using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using GlmNet;
namespace Engine.render
{
    unsafe public class Render2D
    {
        private const Int32 MaxQuadCount = 2000;
        private const Int32 MaxVertexCount = MaxQuadCount * 4;
        private const Int32 MaxIndexCount = MaxQuadCount * 6;
        static private readonly Int32 MaxTextures = GL.GetInteger(GetPName.MaxTextureImageUnits);

        static private Int32 QuadVA = 0;
        static private Int32 QuadVB = 0;
        static private Int32 QuadIB = 0;
        static private Int32 WhiteTexture = 0;
        static private Int32 whiteTextureSlot = 0;
        static private Int32 IndexCount = 0;
        static private Vertex[] QuadBuffer;
        static private Int32 QuadBufferPtr = 0;
        static private Int32[] TextureSlots = new Int32[MaxVertexCount];
        //Zero is the default white texture
        static private UInt32 TextureSlotIndex = 1;
        static private vec2[] TexCoords =
            {
                new vec2(0.0f,0.0f),
                new vec2(1.0f,0.0f),
                new vec2(1.0f,1.0f),
                new vec2(0.0f,1.0f)
            };
        struct Vertex
        {
            public vec3 Position;
            public vec4 Color;
            public vec2 TexCoords;
            public float TexIndex;
        }

        private Render2D() { }
        static public void Init() 
        {
            System.Console.WriteLine("Max texture units: " + MaxTextures);

            if (QuadBuffer != null)
                return;

            QuadBuffer = new Vertex[MaxVertexCount];

            QuadBufferPtr = 0;

            QuadVA = GL.GenVertexArray();
            GL.BindVertexArray(QuadVA);

            QuadVB = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, QuadVB);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxVertexCount * sizeof(Vertex), (IntPtr)0, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("Position"));
            
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("Color"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("TexCoords"));

            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("TexIndex"));

            UInt32[] indices = new UInt32[MaxIndexCount];
            UInt32 offset = 0;
            for (int i = 0; i < MaxIndexCount; i += 6)
            {
                indices[i + 0] = 0 + offset;
                indices[i + 1] = 1 + offset;
                indices[i + 2] = 2 + offset;

                indices[i + 3] = 2 + offset;
                indices[i + 4] = 3 + offset;
                indices[i + 5] = 0 + offset;

                offset += 4;
            }
            QuadIB = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer,QuadIB);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            WhiteTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D,WhiteTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            byte[] color = { 255, 255, 255, 255 };
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 1, 1, 0, PixelFormat.Rgba, PixelType.UnsignedByte, color);
            
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            TextureSlots[0] = WhiteTexture;
            for (UInt32 i = 1; i < MaxTextures; i++)
                TextureSlots[i] = 0;
        
        }
        static public void ShutDown()
        {
            GL.DeleteVertexArray(QuadVA);
            GL.DeleteBuffer(QuadVB);
            GL.DeleteBuffer(QuadIB);

            GL.DeleteTexture(WhiteTexture);
        }
        static public void BeginBatch()
        {
            QuadBufferPtr = 0;
        }
        static public void EndBatch()
        {
            IntPtr size = (IntPtr)(QuadBufferPtr * sizeof(Vertex));
            GL.BindBuffer(BufferTarget.ArrayBuffer, QuadVB);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, size, QuadBuffer);
        }
        static private TextureUnit SwitchTexUnit(UInt32 index)
        {
            switch(index)
            {
                case 0: return TextureUnit.Texture0;
                case 1: return TextureUnit.Texture1;
                case 2: return TextureUnit.Texture2;
                case 3: return TextureUnit.Texture3;
                case 4: return TextureUnit.Texture4;
                case 5: return TextureUnit.Texture5;
                case 6: return TextureUnit.Texture6;
                case 7: return TextureUnit.Texture7;
                case 8: return TextureUnit.Texture8;
                case 9: return TextureUnit.Texture9;
                case 10: return TextureUnit.Texture10;
                case 11: return TextureUnit.Texture11;
                case 12: return TextureUnit.Texture12;
                case 13: return TextureUnit.Texture13;
                case 14: return TextureUnit.Texture14;
                case 15: return TextureUnit.Texture15;
                case 16: return TextureUnit.Texture16;
                case 17: return TextureUnit.Texture17;
                case 18: return TextureUnit.Texture18;
                case 19: return TextureUnit.Texture19;
                case 20: return TextureUnit.Texture20;
                case 21: return TextureUnit.Texture21;
                case 22: return TextureUnit.Texture22;
                case 23: return TextureUnit.Texture23;
                case 24: return TextureUnit.Texture24;
                case 25: return TextureUnit.Texture25;
                case 26: return TextureUnit.Texture26;
                case 27: return TextureUnit.Texture27;
                case 28: return TextureUnit.Texture28;
                case 29: return TextureUnit.Texture29;
                case 30: return TextureUnit.Texture30;
                case 31: return TextureUnit.Texture31;
                default: return TextureUnit.Texture0;
            }
        }
        static public void Flush()
        {
            for (UInt32 i = 0; i < TextureSlotIndex; i++)
            {
                GL.ActiveTexture(SwitchTexUnit(i));
                GL.BindTexture(TextureTarget.Texture2D,TextureSlots[i]);
            }

            GL.BindVertexArray(QuadVA);
            GL.DrawElements(BeginMode.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);

            IndexCount = 0;
            TextureSlotIndex = 1;
        }

        static private void FillVertices(vec2 position, vec2 size, vec4 color,vec2[] texCoords,float texIndex)
        {
            QuadBuffer[QuadBufferPtr].Position = new vec3(position.x, position.y, 0.0f);
            QuadBuffer[QuadBufferPtr].Color = color;
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[0];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = new vec3(position.x + size.x, position.y, 0.0f);
            QuadBuffer[QuadBufferPtr].Color = color;
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[1];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = new vec3(position.x + size.x, position.y + size.y, 0.0f);
            QuadBuffer[QuadBufferPtr].Color = color;
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[2];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = new vec3(position.x, position.y + size.y, 0.0f);
            QuadBuffer[QuadBufferPtr].Color = color;
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[3];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;
        }

        static private void FillVertices(vec3[] position, vec4 color, vec2[] texCoords, float texIndex)
        {
            QuadBuffer[QuadBufferPtr].Position = position[0];
            QuadBuffer[QuadBufferPtr].Color = color;
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[0];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = position[1];
            QuadBuffer[QuadBufferPtr].Color = color;
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[1];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = position[2];
            QuadBuffer[QuadBufferPtr].Color = color;
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[2];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = position[3];
            QuadBuffer[QuadBufferPtr].Color = color;
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[3];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;
        }

        static private void RotateVertices(ref vec3[] vertices,float angle,vec3 rotationCenter,vec3 axis)
        {
            mat4 translationMatrix = glm.translate(mat4.identity(), rotationCenter - (rotationCenter*2));
            mat4 rotationMatrix = glm.rotate(mat4.identity(), angle, axis);
            mat4 reserveTranslationMatrix = glm.translate(mat4.identity(), rotationCenter);

            for (UInt32 i = 0; i < 4; i++)
            {
                vertices[i] = new vec3(
                    reserveTranslationMatrix * rotationMatrix * translationMatrix * new vec4(vertices[i], 1.0f));
            }

        }
        static public void DrawQuad(vec2 position, vec2 size, vec4 color)
        {
            if (IndexCount >= MaxIndexCount)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;


            FillVertices(position, size, color , TexCoords, textureIndex);
            IndexCount += 6;
        }
        static public void DrawQuad(vec2 position, vec2 size, float rotation, vec4 color, vec3? axis = null)
        {
            if (IndexCount >= MaxIndexCount)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;

            vec3[] rectangleVertices = {
                new vec3(position.x,position.y,0.0f),
                new vec3(position.x + size.x,position.y,0.0f),
                new vec3(position.x + size.x,position.y + size.y,0.0f),
                new vec3(position.x,position.y + size.y,0.0f)
            };
            vec3 axiss;
            if (axis != null)
                axiss = (vec3)axis;
            else
                axiss = new vec3(0.0f, 0.0f, 1.0f);

            RotateVertices(ref rectangleVertices, rotation, new vec3(position.x + (size.x / 2), position.y + (size.y / 2), 0.0f),axiss);

            FillVertices(rectangleVertices, color, TexCoords, textureIndex);
            IndexCount += 6;
        }
        static public void DrawQuad(vec2 position, vec2 size, Int32 TextureID)
        {
            if (IndexCount >= MaxIndexCount || TextureSlotIndex > MaxTextures - 1)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;

            for (UInt32 i = 1; i < TextureSlotIndex; i++)
            {
                if (TextureSlots[i] == TextureID)
                {
                    textureIndex = (float)i;
                    break;
                }
            }

            if (textureIndex == 0.0f)
            {
                textureIndex = (float)TextureSlotIndex;
                TextureSlots[TextureSlotIndex] = TextureID;
                TextureSlotIndex++;
            }

            FillVertices(position, size, new vec4(1.0f, 1.0f, 1.0f, 1.0f), TexCoords, textureIndex);
            IndexCount += 6;
        }
        static public void DrawQuad(vec2 position, vec2 size, float rotation ,Int32 TextureID, vec3? axis = null)
        {
            if (IndexCount >= MaxIndexCount || TextureSlotIndex > MaxTextures - 1)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;

            for (UInt32 i = 1; i < TextureSlotIndex; i++)
            {
                if (TextureSlots[i] == TextureID)
                {
                    textureIndex = (float)i;
                    break;
                }
            }

            if (textureIndex == 0.0f)
            {
                textureIndex = (float)TextureSlotIndex;
                TextureSlots[TextureSlotIndex] = TextureID;
                TextureSlotIndex++;
            }

            vec3[] rectangleVertices = {
                new vec3(position.x,position.y,0.0f),
                new vec3(position.x + size.x,position.y,0.0f),
                new vec3(position.x + size.x,position.y + size.y,0.0f),
                new vec3(position.x,position.y + size.y,0.0f)
            };
            vec3 axiss;
            if (axis != null)
                axiss = (vec3)axis;
            else
                axiss = new vec3(0.0f, 0.0f, 1.0f);

            RotateVertices(ref rectangleVertices, rotation, new vec3(position.x + (size.x / 2), position.y + (size.y / 2), 0.0f), axiss);

            FillVertices(rectangleVertices, new vec4(1.0f, 1.0f, 1.0f, 1.0f), TexCoords, textureIndex);
            IndexCount += 6;
        }
        static public void DrawQuad(vec2 position, vec2 size, SubTexture SubTexture)
        {
            if (IndexCount >= MaxIndexCount || TextureSlotIndex > MaxTextures - 1)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;

            for (UInt32 i = 1; i < TextureSlotIndex; i++)
            {
                if (TextureSlots[i] == SubTexture.textureId)
                {
                    textureIndex = (float)i;
                    break;
                }
            }

            if (textureIndex == 0.0f)
            {
                textureIndex = (float)TextureSlotIndex;
                TextureSlots[TextureSlotIndex] = SubTexture.textureId;
                TextureSlotIndex++;
            }

            FillVertices(position, size, new vec4(1.0f, 1.0f, 1.0f, 1.0f), SubTexture.texCoord, textureIndex);
            IndexCount += 6;
        }
        static public void DrawQuad(vec2 position, vec2 size, float rotation, SubTexture SubTexture, vec3? axis = null)
        {
            if (IndexCount >= MaxIndexCount || TextureSlotIndex > MaxTextures - 1)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;

            for (UInt32 i = 1; i < TextureSlotIndex; i++)
            {
                if (TextureSlots[i] == SubTexture.textureId)
                {
                    textureIndex = (float)i;
                    break;
                }
            }

            if (textureIndex == 0.0f)
            {
                textureIndex = (float)TextureSlotIndex;
                TextureSlots[TextureSlotIndex] = SubTexture.textureId;
                TextureSlotIndex++;
            }

            vec3[] rectangleVertices = {
                new vec3(position.x,position.y,0.0f),
                new vec3(position.x + size.x,position.y,0.0f),
                new vec3(position.x + size.x,position.y + size.y,0.0f),
                new vec3(position.x,position.y + size.y,0.0f)
            };
            vec3 axiss;
            if(axis != null)
                axiss = (vec3)axis;
            else
                axiss = new vec3(0.0f, 0.0f, 1.0f);

            RotateVertices(ref rectangleVertices, rotation, new vec3(position.x + (size.x / 2), position.y + (size.y / 2), 0.0f), axiss);

            FillVertices(rectangleVertices, new vec4(1.0f, 1.0f, 1.0f, 1.0f), SubTexture.texCoord, textureIndex);
            IndexCount += 6;
        }

        //Get's
        static public Int32 MaxTextureUnits
        {
            get => MaxTextures;
        }

    }
}