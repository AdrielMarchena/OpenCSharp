using System;
using System.Collections.Generic;
using Engine.utils;
using GlmNet;
namespace OpenCSharp
{
    public class Colision
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

		static public bool RectVsRect(Entity t, Entity f)
        {
			if (t.cx < f.cx + f.cw &&
				t.cx + t.cw > f.cx &&
				t.cy < f.cy + f.ch &&
				t.cy + t.ch > f.cy)
			{
				t.RectColisionNotification(f);
				f.RectColisionNotification(t);
				return true;
			}

			return false;
		}

		static public bool PointVsRect(vec2 p, Entity r)
        {
			return (p.x >= r.cx && p.y >= r.cy && p.x < r.cx + r.cw && p.y < r.cy + r.ch);
        }

		static public bool RayVsRect(vec2 ray_origin, vec2 ray_dir, Rect target,ref vec2 contact_point, ref vec2 contact_normal, ref float t_hit_near)
        {
			vec2 t_near = (target.pos - ray_origin).Divide(ray_dir);
			vec2 t_far = (target.pos + target.size - ray_origin).Divide(ray_dir);

			if (float.IsNaN(t_far.y) || float.IsNaN(t_far.x)) return false;
			if (float.IsNaN(t_near.y) || float.IsNaN(t_near.x)) return false;

			if (t_near.x > t_far.x) Others.Swap(ref t_near.x, ref t_far.x);
			if (t_near.y > t_far.y) Others.Swap(ref t_near.y, ref t_far.y);

			if (t_near.x > t_far.y || t_near.y > t_far.x) return false;

			t_hit_near = Math.Max(t_near.x, t_near.y);
			float t_hit_far = Math.Min(t_far.x, t_far.y);

			if (t_hit_far < 0) return false;

			contact_point = ray_origin + t_hit_near * ray_dir;

			if (t_near.x > t_near.y)
				if (ray_dir.x < 0)
					contact_normal = new vec2(1, 0);
				else
					contact_normal = new vec2(-1, 0);
			else if (t_near.x < t_near.y)
				if (ray_dir.y < 0)
					contact_normal = new vec2(0, 1);
				else
					contact_normal = new vec2(0, -1);

			return true;
		}

		static public bool DynamicRectVsRect(Entity moving, Rect target, ref vec2 contact_point, ref vec2 contact_normal, ref float contact_time, float deltaTime)
        {
			if (moving.Velocity.x == 0 && moving.Velocity.y == 0)
				return false;

			Rect expanded_target = new();
			expanded_target.pos = target.pos - moving.CBox.size.Divide(2);
			expanded_target.size = target.size + moving.CBox.size;

			if(RayVsRect(moving.CBox.pos + moving.CBox.size.Divide(2), moving.Velocity * deltaTime, expanded_target, ref contact_point,ref contact_normal,ref contact_time))
            {
				if (contact_time >= 0.0f && contact_time <= 1.0f)
					return true;
            }
			return false;
        }

		public int ResizeV
        {
            get => m_entityList.Capacity;
            set { m_entityList.Resize(value); }
        }

    }
}
