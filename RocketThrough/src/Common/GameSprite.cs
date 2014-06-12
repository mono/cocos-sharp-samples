using System;
using Microsoft.Xna.Framework;
using CocosSharp;

namespace RocketThrought.Common
{
    public class GameSprite : CCSprite
    {
        public float _radius { get; set; }

        public GameSprite(string spritename)
            : base(spritename)
        {

        }

        public static GameSprite Create(string pszSpriteFrameName)
        {
            var sprite = new GameSprite(pszSpriteFrameName);
            sprite._radius = sprite.BoundingBox.Size.Width * 0.5f;
            return sprite;
        }


    }

}