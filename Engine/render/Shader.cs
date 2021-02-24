using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace Engine
{
    public class Shader
    {
        private Int32 ShaderID;
        private bool disposedValue = false;
        //Save the uniforms location, get uniform locations are kinda costly
        private Dictionary<string, int> m_UniformLocations;
        public Shader(string vertexPath,string fragmentPath)
        {
            m_UniformLocations = new Dictionary<string, int>();
            string VertexShaderSource;

            using(StreamReader reader = new StreamReader(vertexPath,Encoding.UTF8))
            {
                VertexShaderSource = reader.ReadToEnd();
            }

            string FragmentShaderSource;

            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                FragmentShaderSource = reader.ReadToEnd();
            }

            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);


            GL.CompileShader(VertexShader);

            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != System.String.Empty)
                System.Console.WriteLine(infoLogVert);

            GL.CompileShader(FragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);

            if (infoLogFrag != System.String.Empty)
                System.Console.WriteLine(infoLogFrag);

            ShaderID = GL.CreateProgram();

            GL.AttachShader(ShaderID, VertexShader);
            GL.AttachShader(ShaderID, FragmentShader);

            GL.LinkProgram(ShaderID);

            GL.DetachShader(ShaderID, VertexShader);
            GL.DetachShader(ShaderID, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

        }

        public int GetLocation(string name)
        {
            if (m_UniformLocations.ContainsKey(name))
            { 
                return m_UniformLocations[name];
            }

            int location = GL.GetUniformLocation(ShaderID, name);
            if (location == -1)
                Console.WriteLine("Warning: uniform '" + name + "' doesn't exist!");
            m_UniformLocations.Add(name,location);
            return location;
        }
        
        //TODO: Make others versions of this function
        public void SetUniformMat4(string name,Matrix4 matrix,bool transpose = false)
        {
            GL.UniformMatrix4(GetLocation(name), transpose, ref matrix);
        }
        public void SetUniformMat4(string name, float[] value, bool transpose = false)
        {
            GL.UniformMatrix4(GetLocation(name), 1 ,transpose,value);
        }

        static public void BindShader(int ID)
        {
            GL.UseProgram(ID);
        }

        static public void UnbindShader()
        {
            GL.UseProgram(0);
        }

        public Int32 shaderID
        {
            get => ShaderID;
        }
        public void Bind()
        {
            GL.UseProgram(ShaderID);
        }
        public void Unbind()
        {
            GL.UseProgram(0);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(ShaderID);

                disposedValue = true;
            }
        }
        ~Shader()
        {
            GL.DeleteProgram(ShaderID);
        }

    }
}
