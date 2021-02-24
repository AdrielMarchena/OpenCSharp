using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GlmNet;
namespace Engine.camera
{
    public class OrthographicCameraController
    {

		private float m_AspectRatio;
		private float m_ZoomLevel = 1.0f;
		private OrthographicCamera m_Camera;
		private bool m_Rotation;
		private vec2 m_CameraPosition = new vec2( 0.0f, 0.0f );
		private double m_CameraRotation = 0.0f; //In degrees, in the anti-clockwise direction
		private double m_CameraTranslationSpeed = 5.0f, m_CameraRotationSpeed = 180.0f;

		public OrthographicCameraController(float aspectRatio, bool rotation = false)
        {
			m_AspectRatio = aspectRatio;
			m_Camera = new OrthographicCamera(-m_AspectRatio * m_ZoomLevel, -m_AspectRatio * m_ZoomLevel, -m_ZoomLevel, m_ZoomLevel);
			m_Rotation = rotation;
        }

		public void OnUpdate(KeyboardState keyboard,double deltaTime)
        {
			if (keyboard.IsKeyDown(Keys.A))
			{
				m_CameraPosition.x -= (float)(MathHelper.Cos(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
				m_CameraPosition.y -= (float)(MathHelper.Sin(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
			}
			else if (keyboard.IsKeyDown(Keys.D))
			{
				m_CameraPosition.x += (float)(MathHelper.Cos(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
				m_CameraPosition.y += (float)(MathHelper.Sin(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
			}

			if (keyboard.IsKeyDown(Keys.W))
			{
				m_CameraPosition.x += (float)(-MathHelper.Sin(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
				m_CameraPosition.y += (float)(MathHelper.Cos(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
			}
			else if (keyboard.IsKeyDown(Keys.S))
			{
				m_CameraPosition.x -= (float)(-MathHelper.Sin(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
				m_CameraPosition.y -= (float)(MathHelper.Cos(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
			}

			if (m_Rotation)
			{
				if (keyboard.IsKeyDown(Keys.Q))
					m_CameraRotation += m_CameraRotationSpeed * deltaTime;
				if (keyboard.IsKeyDown(Keys.E))
					m_CameraRotation -= m_CameraRotationSpeed * deltaTime;

				if (m_CameraRotation > 180.0f)
					m_CameraRotation -= 360.0f;
				else if (m_CameraRotation <= -180.0f)
					m_CameraRotation += 360.0f;

				m_Camera.SetRotation((float)m_CameraRotation);
			}
			m_Camera.SetPosition(new vec3(m_CameraPosition.x, m_CameraPosition.y,0.0f));

			m_CameraTranslationSpeed = m_ZoomLevel;
		}

		public bool OnMouseScrolled(float yoffset)
        {
			m_ZoomLevel -= yoffset * 0.15f;
			m_ZoomLevel = MathHelper.Max(m_ZoomLevel, 0.25f);
			m_Camera.SetProjection(-m_AspectRatio * m_ZoomLevel, m_AspectRatio * m_ZoomLevel, -m_ZoomLevel, m_ZoomLevel);
			return false;
		}

		public bool OnResize(float w, float h)
        {
			m_AspectRatio = w / h;
            m_Camera.SetProjection(-m_AspectRatio * m_ZoomLevel, m_AspectRatio * m_ZoomLevel, -m_ZoomLevel, m_ZoomLevel);
			return false;
		}

		public OrthographicCamera GetCamera() 
		{ 
			return m_Camera; 
		}

	   public float GetZoomLevel() 
		{ 
			return m_ZoomLevel;
		}
	   public void SetZoomLevel(float level) 
		{ 
			m_ZoomLevel = level;
		}
    }
}
