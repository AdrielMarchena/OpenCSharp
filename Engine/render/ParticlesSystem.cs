using System;
using OpenTK.Mathematics;
using Engine.utils;
using vtv = Engine.utils.Converter;
using GlmNet;
namespace Engine.render
{
    public struct ParticleProps
    {
        public vec2 Position;
        public vec2 Velocity, VelocityVariation;
        public Vector4 ColorBegin, ColorEnd;
        public float SizeBegin, SizeEnd, SizeVariation;
        public float LifeTime;
        public float Gravity;

        public static readonly ParticleProps Effect1;
        public static readonly ParticleProps Effect2;

        /// <summary>
        /// Init Some default effects
        /// </summary>
        static ParticleProps()
        {
            {
                Effect1.ColorBegin = (0.2f, 0.5f, 0.8f, 1.0f);
                Effect1.ColorEnd = (0.8f, 0.5f, 0.2f, 1.0f);
                Effect1.LifeTime = 3.0f;
                Effect1.SizeVariation = 5.0f;
                Effect1.SizeBegin = 20.0f;
                Effect1.SizeEnd = 0.0f;
                Effect1.Velocity = new vec2(10.0f, 10.0f);
                Effect1.VelocityVariation = new vec2(1.0f, 1.0f);
                Effect1.Gravity = 100.0f;
            }
            {
                Effect2 = Effect1;
                Effect2.LifeTime = 1.0f;
                Effect2.VelocityVariation = new vec2(50.0f, 50.0f);
                Effect2.ColorBegin = (0.9f, 0.3f, 0.2f, 1.0f);
                Effect2.ColorBegin = (0.8f, 0.2f, 0.1f, 1.0f);
            }
        }
    }
    public struct Particle
    {
        public vec2 Position;
        public vec2 Velocity, VelocityVariation;
        public vec3 axisRot;
        public Vector4 ColorBegin, ColorEnd;
        public float Rotation;
        public float SizeBegin, SizeEnd;

        public float LifeTime;
        public float LifeRemaining;
        public bool Active;
        public float Gravity;
    }
    public class ParticlesSystem
    {
        private Particle[] m_ParticlesPool;
        Int32 m_PoolIndex = 0;

        public ParticlesSystem()
        {
            m_ParticlesPool = new Particle[500];
        }

        public void OnUpdate(float deltaTime)
        {
            for (int i = 0; i < m_ParticlesPool.Length; i++)
            {
                if (!m_ParticlesPool[i].Active)
                    continue;

                if (m_ParticlesPool[i].LifeRemaining <= 0.0f)
                {
                    m_ParticlesPool[i].Active = false;
                    continue;
                }

                m_ParticlesPool[i].LifeRemaining -= deltaTime;
                m_ParticlesPool[i].Position += m_ParticlesPool[i].Velocity * (float)deltaTime;
                m_ParticlesPool[i].Position.y -= m_ParticlesPool[i].Gravity * deltaTime;
                m_ParticlesPool[i].Rotation += 50.0f * deltaTime;
            }
        }

        public void OnRender()
        {
            for (int i = 0; i < m_ParticlesPool.Length; i++)
            {
                if (!m_ParticlesPool[i].Active)
                    continue;

                // Fade away particles
                float life = m_ParticlesPool[i].LifeRemaining / m_ParticlesPool[i].LifeTime;
                Vector4 colorR = Vector4.Lerp(m_ParticlesPool[i].ColorBegin, m_ParticlesPool[i].ColorEnd, life);

                vec4[] colors = new vec4[4] {
                 vtv.Vect4toVec4(m_ParticlesPool[i].ColorBegin),
                 vtv.Vect4toVec4(colorR),
                 vtv.Vect4toVec4(m_ParticlesPool[i].ColorEnd),
                 vtv.Vect4toVec4(colorR)
                };

                colorR.W *= life;
                
                float size = MathHelper.Lerp(m_ParticlesPool[i].SizeEnd, m_ParticlesPool[i].SizeBegin, life);
                Render2D.DrawQuad(m_ParticlesPool[i].Position, new vec2(size,size), MathHelper.DegreesToRadians(m_ParticlesPool[i].Rotation), colors, m_ParticlesPool[i].axisRot);
        }
    }

        public void Emit(ParticleProps particleProps)
        {
            m_ParticlesPool[m_PoolIndex].Active = true;
            m_ParticlesPool[m_PoolIndex].Position = particleProps.Position;
            m_ParticlesPool[m_PoolIndex].Rotation = RandomTable.RandomFloat() * 5.0f * MathHelper.Pi;
            m_ParticlesPool[m_PoolIndex].Gravity = particleProps.Gravity;
            // Velocity
            m_ParticlesPool[m_PoolIndex].Velocity = particleProps.Velocity;
            m_ParticlesPool[m_PoolIndex].Velocity.x += particleProps.VelocityVariation.x * (RandomTable.RandomFloat() - 0.5f);
            m_ParticlesPool[m_PoolIndex].Velocity.y += particleProps.VelocityVariation.y * (RandomTable.RandomFloat() - 0.5f);

            // Color
            m_ParticlesPool[m_PoolIndex].ColorBegin = particleProps.ColorBegin;
            m_ParticlesPool[m_PoolIndex].ColorEnd = particleProps.ColorEnd;

            m_ParticlesPool[m_PoolIndex].LifeTime = particleProps.LifeTime;
            m_ParticlesPool[m_PoolIndex].LifeRemaining = particleProps.LifeTime;
            m_ParticlesPool[m_PoolIndex].SizeBegin = particleProps.SizeBegin + particleProps.SizeVariation * (RandomTable.RandomFloat() - 0.5f);
            m_ParticlesPool[m_PoolIndex].SizeEnd = particleProps.SizeEnd;

            m_ParticlesPool[m_PoolIndex].axisRot = new vec3((RandomTable.RandomInt() % 2), (RandomTable.RandomInt() * 0), (1 + RandomTable.RandomInt() % 2));
            
            m_PoolIndex = ++m_PoolIndex % m_ParticlesPool.Length;
        }
    }
}
