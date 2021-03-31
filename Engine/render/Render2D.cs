using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using GlmNet;
using Engine.utils;

namespace Engine.render
{
    /// <summary>
    /// Render2D class, use to render quads (Call Render2D.Init() somewhere First)
    /// </summary>
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
        static private TextureUnit t;
        struct Vertex
        {
            public vec3 Position;
            public vec4 Color;
            public vec2 TexCoords;
            public float TexIndex;
        }

        static public bool PolygonMode;
        private Render2D() { }

        /// <summary>
        /// Prepare a bunch of resources needed
        /// </summary>
        static public void Init() 
        {
            Console.WriteLine("Max texture units: " + MaxTextures);

            PolygonMode = false;

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

        /// <summary>
        /// Dispose things and ShutDown the Render2D
        /// </summary>
        static public void ShutDown()
        {
            GL.DeleteVertexArray(QuadVA);
            GL.DeleteBuffer(QuadVB);
            GL.DeleteBuffer(QuadIB);

            GL.DeleteTexture(WhiteTexture);
        }

        /// <summary>
        /// Begin a new Batch
        /// </summary>
        static public void BeginBatch()
        {
            QuadBufferPtr = 0;
        }

        /// <summary>
        /// End the Batch (call it before Flush)
        /// </summary>
        static public void EndBatch()
        {
            IntPtr size = (IntPtr)(QuadBufferPtr * sizeof(Vertex));
            GL.BindBuffer(BufferTarget.ArrayBuffer, QuadVB);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, size, QuadBuffer);
        }
        
        /*
        /// <summary>
        /// Switch case to get the enum TextureUnit
        /// </summary>
        /// <param name="index">Texture Unit number</param>
        /// <returns></returns>
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
        */

        /// <summary>
        /// Draws all Quads prepared
        /// </summary>
        static public void Flush()
        {
            for (Int32 i = 0; i < TextureSlotIndex; i++)
            {
                GL.ActiveTexture(t.Add(i));
                //GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + i));
                GL.BindTexture(TextureTarget.Texture2D,TextureSlots[i]);
            }

            GL.BindVertexArray(QuadVA);
            if (PolygonMode)
                GL.PolygonMode(MaterialFace.FrontAndBack, OpenTK.Graphics.OpenGL.PolygonMode.Line);
            GL.DrawElements(BeginMode.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);

            IndexCount = 0;
            TextureSlotIndex = 1;
        }

        /// <summary>
        /// Fill vertices
        /// </summary>
        /// <param name="position">Position left bottom</param>
        /// <param name="size">Quad size</param>
        /// <param name="color">Quad color</param>
        /// <param name="texCoords">Quad Tex Coords</param>
        /// <param name="texIndex">Quad Tex index</param>
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

        /// <summary>
        /// Fill vertices
        /// </summary>
        /// <param name="position">Position left bottom</param>
        /// <param name="size">Quad size</param>
        /// <param name="color">Four colors to degradee</param>
        /// <param name="texCoords">Quad Tex Coords</param>
        /// <param name="texIndex">Quad Tex index</param>
        static private void FillVertices(vec2 position, vec2 size, vec4[] color, vec2[] texCoords, float texIndex)
        {
            QuadBuffer[QuadBufferPtr].Position = new vec3(position.x, position.y, 0.0f);
            QuadBuffer[QuadBufferPtr].Color = color[0];
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[0];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = new vec3(position.x + size.x, position.y, 0.0f);
            QuadBuffer[QuadBufferPtr].Color = color[1];
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[1];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = new vec3(position.x + size.x, position.y + size.y, 0.0f);
            QuadBuffer[QuadBufferPtr].Color = color[2];
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[2];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = new vec3(position.x, position.y + size.y, 0.0f);
            QuadBuffer[QuadBufferPtr].Color = color[3];
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[3];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;
        }

        /// <summary>
        /// Fill vertices
        /// </summary>
        /// <param name="position">The already calculated vertices</param>
        /// <param name="color">Quad color</param>
        /// <param name="texCoords">Quad Tex Coords</param>
        /// <param name="texIndex">Quad Tex index</param>
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

        /// <summary>
        /// Fill vertices
        /// </summary>
        /// <param name="position">The already calculated vertices</param>
        /// <param name="color">Four colors to degradee</param>
        /// <param name="texCoords">Quad Tex Coords</param>
        /// <param name="texIndex">Quad Tex index</param>
        static private void FillVertices(vec3[] position, vec4[] color, vec2[] texCoords, float texIndex)
        {
            QuadBuffer[QuadBufferPtr].Position = position[0];
            QuadBuffer[QuadBufferPtr].Color = color[0];
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[0];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = position[1];
            QuadBuffer[QuadBufferPtr].Color = color[1];
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[1];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = position[2];
            QuadBuffer[QuadBufferPtr].Color = color[2];
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[2];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;

            QuadBuffer[QuadBufferPtr].Position = position[3];
            QuadBuffer[QuadBufferPtr].Color = color[3];
            QuadBuffer[QuadBufferPtr].TexCoords = texCoords[3];
            QuadBuffer[QuadBufferPtr].TexIndex = texIndex;
            QuadBufferPtr++;
        }

        /// <summary>
        /// Used to rotate Quad using their vertices
        /// </summary>
        /// <param name="vertices">vertices of the quad</param>
        /// <param name="angle">angle in radians</param>
        /// <param name="rotationCenter">the rotation center</param>
        /// <param name="axis">The axis of the rotation</param>
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
        
        /// <summary>
        /// Draw a Quad with a color
        /// </summary>
        /// <param name="position">Is in the left bottom</param>
        /// <param name="size">The size of the Quad</param>
        /// <param name="color">The color of the quad</param>
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

        /// <summary>
        /// Draw a Quad with a degradee color
        /// </summary>
        /// <param name="position">Is in the left bottom</param>
        /// <param name="size">The size of the Quad</param>
        /// <param name="color">The four color to generate a degradee</param>
        static public void DrawQuad(vec2 position, vec2 size, vec4[] color)
        {
            if (IndexCount >= MaxIndexCount)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            float textureIndex = 0.0f;

            FillVertices(position, size, color, TexCoords, textureIndex);
            IndexCount += 6;
        }

        /// <summary>
        /// Draw a rotated Quad whith a color
        /// </summary>
        /// <param name="position">Is in the left bottom</param>
        /// <param name="size">The size of the Quad</param>
        /// <param name="rotation">Rotation in radians</param>
        /// <param name="color">The color of the Quad</param>
        /// <param name="axis">The axis of the rotation (default is z axis)</param>
        /// <param name="rotatinCenter">The center of the rotation (center si default)</param>
        static public void DrawQuad(vec2 position, vec2 size, float rotation, vec4 color, vec3? axis = null, vec3 ? rotatinCenter = null)
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

            vec3 rotcent;
            if (rotatinCenter != null)
                rotcent = (vec3)rotatinCenter;
            else
                rotcent = new vec3(position.x + (size.x / 2), position.y + (size.y / 2), 0.0f);

            RotateVertices(ref rectangleVertices, rotation, rotcent,axiss);

            FillVertices(rectangleVertices, color, TexCoords, textureIndex);
            IndexCount += 6;
        }

        /// <summary>
        /// Draw a rotated Quad whith a degradee color
        /// </summary>
        /// <param name="position">Is in the left bottom</param>
        /// <param name="size">The size of the Quad</param>
        /// <param name="rotation">Rotation in radians</param>
        /// <param name="color">The four color to generate a degradee</param>
        /// <param name="axis">The axis of the rotation (default is z axis)</param>
        /// <param name="rotatinCenter">The center of the rotation (center is default)</param>
        static public void DrawQuad(vec2 position, vec2 size, float rotation, vec4[] color, vec3? axis = null, vec3? rotatinCenter = null)
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

            vec3 rotcent;
            if (rotatinCenter != null)
                rotcent = (vec3)rotatinCenter;
            else
                rotcent = new vec3(position.x + (size.x / 2), position.y + (size.y / 2), 0.0f);

            RotateVertices(ref rectangleVertices, rotation, rotcent, axiss);

            FillVertices(rectangleVertices, color, TexCoords, textureIndex);
            IndexCount += 6;
        }

        /// <summary>
        /// Draw a Quad with a texture
        /// </summary>
        /// <param name="position">Is in the left bottom</param>
        /// <param name="size">The size of the Quad</param>
        /// <param name="TextureID">Id of an valid texture, 0 for white default texture</param>
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

        /// <summary>
        /// Draw a rotate Quad with a texture
        /// </summary>
        /// <param name="position">Is in the left bottom</param>
        /// <param name="size">The size of the Quad</param>
        /// <param name="rotation">The rotation in radians</param>
        /// <param name="TextureID">Id of an valid texture, 0 for white default texture</param>
        /// <param name="axis">The axis of the rotation (default is z axis)</param>
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
        
        /// <summary>
        /// Draws a Quad with a Croped Texture
        /// </summary>
        /// <param name="position">Is in the left bottom</param>
        /// <param name="size">The size of the Quad</param>
        /// <param name="SubTexture">The subtexture to be used</param>
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

        /// <summary>
        /// Draws a rotated Quad with a Croped Texture
        /// </summary>
        /// <param name="position">Is in the left bottom</param>
        /// <param name="size">The size of the Quad</param>
        /// <param name="rotation">rotation in radians</param>
        /// <param name="SubTexture">The subtexture to be used</param>
        /// <param name="axis">The axis of the rotation (default is z axis)</param>
        /// <param name="rotatinCenter">The center of the rotation (center is default)</param>
        static public void DrawQuad(vec2 position, vec2 size, float rotation, SubTexture SubTexture, vec3? axis = null, vec3? rotatinCenter = null)
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

            vec3 rotcent;
            if (rotatinCenter != null)
                rotcent = (vec3)rotatinCenter;
            else
                rotcent = new vec3(position.x + (size.x / 2), position.y + (size.y / 2), 0.0f);

            RotateVertices(ref rectangleVertices, rotation, rotcent, axiss);

            FillVertices(rectangleVertices, new vec4(1.0f, 1.0f, 1.0f, 1.0f), SubTexture.texCoord, textureIndex);
            IndexCount += 6;
        }
        /// <summary>
        /// Draw outlined quad
        /// <para>Origin is the Left Corner</para>
        /// <para>Up Right Down Left </para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="thick"></param>
        /// <param name="color"></param>
        static public void DrawLineQuad(vec2 position, vec2 size, float thick,vec4 color)
        {
            Render2D.DrawQuad(new vec2(position.x, position.y), new vec2(thick, size.y), color);
            Render2D.DrawQuad(new vec2(position.x, position.y + size.y), new vec2(size.x, thick), color);
            Render2D.DrawQuad(new vec2(position.x + size.x, position.y + size.y), new vec2(-thick, -size.y), color);
            Render2D.DrawQuad(new vec2(position.x + size.x, position.y), new vec2(-size.x, -thick), color);
        }
        /// <summary>
        /// Draw outlined quad (each line can be any color)
        /// <para>Origin is the Left Corner</para>
        /// <para>Up Right Down Left </para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="thick"></param>
        /// <param name="color"></param>
        static public void DrawLineQuad(vec2 position, vec2 size, float thick, vec4[] color)
        {
            Render2D.DrawQuad(new vec2(position.x, position.y), new vec2(thick, size.y), color[0]);
            Render2D.DrawQuad(new vec2(position.x, position.y + size.y), new vec2(size.x, thick), color[1]);
            Render2D.DrawQuad(new vec2(position.x + size.x, position.y + size.y), new vec2(-thick, -size.y), color[2]);
            Render2D.DrawQuad(new vec2(position.x + size.x, position.y), new vec2(-size.x, -thick), color[3]);
        }

        static public void DrawLineQuad(vec2 position, vec2 size, float thick, vec4[][] color)
        {
            Render2D.DrawQuad(new vec2(position.x, position.y), new vec2(thick, size.y), color[0]);
            Render2D.DrawQuad(new vec2(position.x, position.y + size.y), new vec2(size.x, -thick), color[1]);
            Render2D.DrawQuad(new vec2(position.x + size.x, position.y + size.y), new vec2(-thick, -size.y), color[2]);
            Render2D.DrawQuad(new vec2(position.x + size.x, position.y), new vec2(-size.x, thick), color[3]);
        }

        static public void DrawLine(vec2 origin, vec2 dest, float thick, vec4 color)
        {
            //TODO: improve this method
            if (IndexCount >= MaxIndexCount)
            {
                EndBatch();
                Flush();
                BeginBatch();
            }

            //RIGHT UP LEFT DOWN

            //This check make the line make more sense
            if((origin.x > dest.x && origin.y < dest.y) || (origin.x < dest.x && origin.y > dest.y))
            {
                QuadBuffer[QuadBufferPtr].Position = new(origin.x ,origin.y, 0);
                QuadBuffer[QuadBufferPtr].Color = color;
                QuadBuffer[QuadBufferPtr].TexCoords = TexCoords[0];
                QuadBuffer[QuadBufferPtr].TexIndex = 0;
                QuadBufferPtr++;

                QuadBuffer[QuadBufferPtr].Position = new(origin.x, origin.y + thick/2, 0);
                QuadBuffer[QuadBufferPtr].Color = color;
                QuadBuffer[QuadBufferPtr].TexCoords = TexCoords[1];
                QuadBuffer[QuadBufferPtr].TexIndex = 0;
                QuadBufferPtr++;

                QuadBuffer[QuadBufferPtr].Position = new(dest.x,dest.y, 0.0f);
                QuadBuffer[QuadBufferPtr].Color = color;
                QuadBuffer[QuadBufferPtr].TexCoords = TexCoords[2];
                QuadBuffer[QuadBufferPtr].TexIndex = 0;
                QuadBufferPtr++;

                QuadBuffer[QuadBufferPtr].Position = new(dest.x,dest.y - thick/2, 0.0f);
                QuadBuffer[QuadBufferPtr].Color = color;
                QuadBuffer[QuadBufferPtr].TexCoords = TexCoords[3];
                QuadBuffer[QuadBufferPtr].TexIndex = 0;
                QuadBufferPtr++;
                
                IndexCount += 6;
            }
            else
            if ((origin.x < dest.x && origin.y < dest.y) || (origin.x > dest.x && origin.y > dest.y))
            {
                QuadBuffer[QuadBufferPtr].Position = new(dest.x, dest.y, 0);
                QuadBuffer[QuadBufferPtr].Color = color;
                QuadBuffer[QuadBufferPtr].TexCoords = TexCoords[0];
                QuadBuffer[QuadBufferPtr].TexIndex = 0;
                QuadBufferPtr++;

                QuadBuffer[QuadBufferPtr].Position = new(dest.x, dest.y + thick / 2, 0);
                QuadBuffer[QuadBufferPtr].Color = color;
                QuadBuffer[QuadBufferPtr].TexCoords = TexCoords[1];
                QuadBuffer[QuadBufferPtr].TexIndex = 0;
                QuadBufferPtr++;

                QuadBuffer[QuadBufferPtr].Position = new(origin.x, origin.y, 0.0f);
                QuadBuffer[QuadBufferPtr].Color = color;
                QuadBuffer[QuadBufferPtr].TexCoords = TexCoords[2];
                QuadBuffer[QuadBufferPtr].TexIndex = 0;
                QuadBufferPtr++;

                QuadBuffer[QuadBufferPtr].Position = new(origin.x, origin.y - thick / 2, 0.0f);
                QuadBuffer[QuadBufferPtr].Color = color;
                QuadBuffer[QuadBufferPtr].TexCoords = TexCoords[3];
                QuadBuffer[QuadBufferPtr].TexIndex = 0;
                QuadBufferPtr++;

                IndexCount += 6;
            }
        }

        //Get's
        static public Int32 MaxTextureUnits
        {
            get => MaxTextures;
        }

    }
}