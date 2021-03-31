using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GlmNet;
namespace Engine.camera
{
    public class OrthographicCameraController
    {

		public enum Direction : byte
        {
			RIGHT,
			LEFT,
			DOWN,
			UP
        }

		public enum RotDirection : byte
		{
			RIGHT,
			LEFT
		}

		private float m_AspectRatio;
		private float m_ZoomLevel = 1.0f;
		private OrthographicCamera m_Camera;
		private bool m_Rotation;
		private vec2 m_CameraPosition = new vec2( 0.0f, 0.0f );
		private double m_CameraRotation = 0.0f; //In degrees, in the anti-clockwise direction
		private double m_CameraTranslationSpeed = 5.0f, m_CameraRotationSpeed = 180.0f;
		/// <summary>
		/// <para>X : horizontal min (default: 0.0f)</para>
		/// <para>Y : horizontal max (default: 100.0f)</para>
		/// <para>Z : vertical min (default: 0.0f)</para>
		/// <para>W : vertical max (default: 100.0f)</para>
		/// </summary>
		public vec4 m_cameraBounds = new vec4(0.0f, 2.5f, 0.0f, 2.5f);

		public OrthographicCameraController(float aspectRatio, bool rotation = false)
        {
			m_AspectRatio = aspectRatio;
			m_Camera = new OrthographicCamera(-m_AspectRatio * m_ZoomLevel, -m_AspectRatio * m_ZoomLevel, -m_ZoomLevel, m_ZoomLevel);
			m_Rotation = rotation;
        }

		public void Move(Direction direction,double deltaTime = 1)
        {
			switch(direction)
            {
				case Direction.LEFT:
					m_CameraPosition.x -= (float)(MathHelper.Cos(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
					m_CameraPosition.y -= (float)(MathHelper.Sin(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
				break;
				case Direction.RIGHT:
					m_CameraPosition.x += (float)(MathHelper.Cos(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
					m_CameraPosition.y += (float)(MathHelper.Sin(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
				break;
				case Direction.UP:
					m_CameraPosition.x += (float)(-MathHelper.Sin(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
					m_CameraPosition.y += (float)(MathHelper.Cos(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
				break;
				case Direction.DOWN:
					m_CameraPosition.x -= (float)(-MathHelper.Sin(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
					m_CameraPosition.y -= (float)(MathHelper.Cos(MathHelper.DegreesToRadians(m_CameraRotation)) * m_CameraTranslationSpeed * deltaTime);
				break;
            }
			m_Camera.SetPosition(new vec3(m_CameraPosition.x, m_CameraPosition.y, 0.0f));
		}

		public void RotateCamera(RotDirection direction,double deltaTime)
        {
			switch(direction)
            {
				case RotDirection.LEFT:
					m_CameraRotation += m_CameraRotationSpeed * deltaTime;
				break;
				case RotDirection.RIGHT:
					m_CameraRotation -= m_CameraRotationSpeed * deltaTime;
				break;
			}

			if (m_CameraRotation > 180.0f)
				m_CameraRotation -= 360.0f;
			else if (m_CameraRotation <= -180.0f)
				m_CameraRotation += 360.0f;
			m_Camera.SetRotation((float)m_CameraRotation);

		}

		public void OnUpdate(KeyboardState keyboard,double deltaTime)
        {
			if (keyboard.IsKeyDown(Keys.A))
			{
				if (m_CameraPosition.x > m_cameraBounds.x)
                {
					Move(Direction.LEFT,deltaTime);
                }
			}
			else if (keyboard.IsKeyDown(Keys.D))
			{
				if(m_CameraPosition.x < m_cameraBounds.y)
                {
					Move(Direction.RIGHT, deltaTime);
				}
			}

			if (keyboard.IsKeyDown(Keys.W))
			{
				if (m_CameraPosition.y < m_cameraBounds.w)
                {
					Move(Direction.UP, deltaTime);
				}
			}
			else if (keyboard.IsKeyDown(Keys.S))
			{
				if(m_CameraPosition.y > m_cameraBounds.z)
                {
					Move(Direction.DOWN, deltaTime);
				}
			}

			if (m_Rotation)
			{
				if (keyboard.IsKeyDown(Keys.Q))
					RotateCamera(RotDirection.LEFT, deltaTime);
				if (keyboard.IsKeyDown(Keys.E))
					RotateCamera(RotDirection.RIGHT, deltaTime);
			}

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
