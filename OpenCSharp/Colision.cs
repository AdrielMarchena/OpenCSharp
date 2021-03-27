using System;
using System.Collections.Generic;
using Engine.utils;
namespace OpenCSharp
{
	
    class Colision
    {

        private List<Entity> m_entityList;

        public Colision(Entity[] e)
        {
			m_entityList = new List<Entity>();
			m_entityList.AddRange(e);
        }

		public Colision()
        {
			m_entityList = new List<Entity>();
		}

		public void PushCol(Entity e)
        {
			m_entityList.Add(e);
        }

		public void PushCol(Entity[] e)
		{
			m_entityList.AddRange(e);
		}

		static public bool RectColision(Entity t, Entity f)
        {
			float dx = ((t.x + t.w) / 2) - ((f.x + f.w) / 2);
			float dy = ((t.y + t.h) / 2) - ((f.y + f.h) / 2);
			float wid = (t.w + f.w) / 2;
			float hei = (t.h + f.h) / 2;
			float crossW = wid * dy;
			float crossH = hei * dx;

			if (Math.Abs(dx) <= wid && Math.Abs(dy) <= hei)
				if (crossW > crossH)
				{
					if (crossW > (-crossH))
					{
						//Bottom
						t.RectColisionNotification(f, RectSide.BOTTOM);
						f.RectColisionNotification(t, RectSide.TOP);
						return true;
					}
					else
					{
						//Left
						t.RectColisionNotification(f, RectSide.LEFT);
						f.RectColisionNotification(t, RectSide.RIGHT);
						return true;
					}
				}
				else
				{
					if (crossW > -(crossH))
					{
						//Right
						t.RectColisionNotification(f, RectSide.RIGHT);
						f.RectColisionNotification(t, RectSide.LEFT);
						return true;
					}
					else
					{
						//Top
						t.RectColisionNotification(f, RectSide.TOP);
						f.RectColisionNotification(t, RectSide.BOTTOM);
						return true;
					}
				}
			else
            {
				return false;
				//t.RectColisionNotification(f);
				//f.RectColisionNotification(t);
			}
		}

		public int ResizeV
        {
            get => m_entityList.Capacity;
            set { m_entityList.Resize(value); }
        }

    }
}
