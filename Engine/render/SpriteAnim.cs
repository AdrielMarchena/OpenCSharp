using System.Collections.Generic;
#nullable enable
namespace Engine.render
{
    public struct SpriteAnimProp
    {
        /// <summary>
        /// Duration of the animation
        /// </summary>
        public float threshold;
        public float thEach;
        /// <summary>
        /// Sprites to animate
        /// </summary>
        public SubTexture[] m_sprites;
    }
    public class SpriteAnim
    {
        private Dictionary<string,SpriteAnimProp> m_sprites;
        private float actualValueThresI = 0;
        private SpriteAnimProp actualAnimation;
        public string actualNameAnim { get; private set; } = "";
        public bool running { get; private set; } = false;
        public bool loop { get; set; } = false;

        private int actualSprite = 0;
        public SpriteAnim(Dictionary<string, SpriteAnimProp> sprites)
        {
            m_sprites = sprites;
        }

        /// <summary>
        /// Push a animation (sprite line)
        /// </summary>
        /// <param name="spriteName">sprite name</param>
        /// <param name="spriteProps">sprite props</param>
        public void PushSprite(string spriteName, SpriteAnimProp spriteProps)
        {
            spriteProps.thEach = spriteProps.threshold / spriteProps.m_sprites.Length;
            m_sprites[spriteName] = spriteProps;
        }

        /// <summary>
        /// Set the animation (sprite line) that will be used
        /// </summary>
        /// <param name="spriteName">Name of the sprite (sprite line)</param>
        public void SetAnim(string spriteName)
        {
            actualNameAnim = spriteName;
            actualAnimation = m_sprites[actualNameAnim];
            actualValueThresI = actualAnimation.threshold;
            actualSprite = 0;
            running = true;
            loop = true;
        }

        /// <summary>
        /// Run The animation, also control the timer to the next sprite
        /// <para>Be sure to call SetAnim first</para>
        /// </summary>
        /// <param name="deltaTime">Delta Time of the game loop</param>
        /// <param name="velocity">Value that the timer will decrement</param>
        /// <returns></returns>
        public SubTexture? RunAnim(float deltaTime,float velocity = 1)
        {
            if(running)
            {
                if(actualValueThresI <= 0 && loop)
                {
                    SetAnim(actualNameAnim);
                }
                if (actualValueThresI <= 0 && !loop)
                {
                    running = false;
                }
                if (actualValueThresI <= actualAnimation.thEach)
                {
                    actualSprite = (actualSprite + 1) % actualAnimation.m_sprites.Length;
                    //Add velocity to compensate the subtraction below
                    actualValueThresI = actualAnimation.threshold + velocity;
                }

                actualValueThresI -= velocity;
                return actualAnimation.m_sprites[actualSprite];
            }
            return null;
        }

        /// <summary>
        /// Utility Function to create a Dictionary, will just glue the name with the sprite props
        /// </summary>
        /// <param name="names">Array of names to the Lines of teh animation</param>
        /// <param name="spritesProps">Array of props for the animation</param>
        /// <returns></returns>
        static public Dictionary<string,SpriteAnimProp> CreateSpriteDict(string[] names, SpriteAnimProp[] spritesProps)
        {
            if (names.Length != spritesProps.Length)
                throw new System.Exception("Names and spriteProps need to be the same Length");

            Dictionary<string, SpriteAnimProp> tmp = new Dictionary<string, SpriteAnimProp>();
            for(int i = 0; i < names.Length; i++)
            {
                spritesProps[i].thEach = spritesProps[i].threshold / spritesProps[i].m_sprites.Length;
                tmp[names[i]] = spritesProps[i];
            }

            return tmp;
        }
    }
}
