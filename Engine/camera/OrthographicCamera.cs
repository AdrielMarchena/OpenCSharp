using OpenTK.Mathematics;
using GlmNet;
namespace Engine.camera
{
    public class OrthographicCamera
    {
        private mat4 m_ProjectionMatrix;
        private mat4 m_ViewMatrix;
        private mat4 m_ViewProjectionMatrix;
        private vec3 m_Position = new vec3(0.0f, 0.0f, 0.0f);
        private float m_Rotation = 0.0f;

        public OrthographicCamera(float left, float right, float bottom, float top)
        {
            m_ProjectionMatrix = glm.ortho(left, right, bottom, top, -1.0f, 1.0f);
            m_ViewMatrix = new mat4(1.0f);
            m_ViewProjectionMatrix = m_ViewMatrix * m_ProjectionMatrix;
        }

        public void SetProjection(float left, float right, float bottom, float top)
        {
            m_ProjectionMatrix = glm.ortho(left, right, bottom, top, -1.0f, 1.0f);
            m_ViewProjectionMatrix = m_ViewMatrix * m_ProjectionMatrix;
        }

        private void RecalculateViewMatrix()
        {
            mat4 transform = glm.translate(mat4.identity(), m_Position) *
            glm.rotate(mat4.identity(), glm.radians(m_Rotation), new vec3(0, 0, 1));

            m_ViewMatrix = glm.inverse(transform);
            m_ViewProjectionMatrix = m_ViewMatrix * m_ProjectionMatrix;
        }

        public vec3 position
        {
            get => m_Position;
            private set => m_Position = value;
        }
        public vec3 GetPosition() 
        { 
            return m_Position;
        }
        public void SetPosition(vec3 position) 
        { 
            m_Position = position;
            RecalculateViewMatrix(); 
        }

        public float rotation
        {
            get => m_Rotation;
            private set => m_Rotation = value;
        }

        public float GetRotation() 
        { 
            return m_Rotation;
        }
        public void SetRotation(float rotation) 
        { 
            m_Rotation = rotation; 
            RecalculateViewMatrix(); 
        }

        public mat4 GetProjectionMatrix() 
        { 
            return m_ProjectionMatrix;
        }

        public float[] GetProjectionMatrixArray()
        {
            return m_ProjectionMatrix.to_array();
        }

        public Matrix4 GetProjectionMatrixTK()
        {
            float[] tmp = m_ProjectionMatrix.to_array();
            
            return new Matrix4(tmp[0], tmp[1], tmp[2], tmp[3], 
                                tmp[4], tmp[5], tmp[6], tmp[7], 
                                tmp[8], tmp[9], tmp[10], tmp[11], 
                                tmp[12], tmp[13], tmp[14], tmp[15]);
        }


        public mat4 GetViewMatrix() 
        { 
            return m_ViewMatrix; 
        }

        public float[] GetViewMatrixArray()
        {
            return m_ViewMatrix.to_array();
        }

        public Matrix4 GetViewMatrixTK()
        {
            float[] tmp = m_ViewMatrix.to_array();

            return new Matrix4(tmp[0], tmp[1], tmp[2], tmp[3],
                                tmp[4], tmp[5], tmp[6], tmp[7],
                                tmp[8], tmp[9], tmp[10], tmp[11],
                                tmp[12], tmp[13], tmp[14], tmp[15]);
        }


        public mat4 GetViewProjectionMatrix() 
        { 
            return m_ViewProjectionMatrix; 
        }

        public float[] GetViewProjectionMatrixArray()
        {
            return m_ViewProjectionMatrix.to_array();
        }

        public Matrix4 GetViewProjectionMatrixTK()
        {
            float[] tmp = m_ViewProjectionMatrix.to_array();

            return new Matrix4(tmp[0], tmp[1], tmp[2], tmp[3],
                                tmp[4], tmp[5], tmp[6], tmp[7],
                                tmp[8], tmp[9], tmp[10], tmp[11],
                                tmp[12], tmp[13], tmp[14], tmp[15]);
        }
    }
}
