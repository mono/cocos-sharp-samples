using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AirHockey.Common
{
    class GameSprite : CCSprite
    {
        #region Properties

        public float Radius { get { return ScaledContentSize.Width * 0.5f; } }
        public CCTouch Touch { get; set; }
        public CCPoint NextPosition { get; set; }
        public CCPoint Vector { get; set; }

        public override CCPoint Position 
        {
            get { return base.Position; }
            set 
            {
                base.Position = value;

                if (NextPosition != value)
                    NextPosition = value;
            }
        }

        #endregion Properties


        #region Constructors

        public GameSprite(string url):base(url)
        {
            NextPosition = Position;
        }

        #endregion Constructors
    }
}
