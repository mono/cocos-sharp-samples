using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.IO.SneakyJoystick
{
    public class SneakyJoystickControlSkinnedBase : SneakyJoystickControl
    {

        #region Static properties

        public static string DEFAULT_IMAGE_BACKGROUND { get { return "control/joystick_background"; } }
        public static string DEFAULT_IMAGE_THUMB { get { return "control/joystick_thumb"; } }

        #endregion

        #region Private properties

        private CCSprite _backgroundSprite;
        private CCSprite _thumbSprite;
        private byte _opacity;

        #endregion

        #region Public properties

        public CCSprite ThumbSprite
        {
            get
            {
                return _thumbSprite;
            }
            set
            {
                if (_thumbSprite != null)
                {
                    if (_thumbSprite.Parent != null)
                        _thumbSprite.Parent.RemoveChild(_thumbSprite, true);
                }

                _thumbSprite = value;

                if (value != null)
                {

                    AddChild(_thumbSprite, 1);

                    RefreshThumbSpritePosition();

                }

            }
        }
        public CCSprite BackgroundSprite
        {
            get
            {
                return _backgroundSprite;
            }
            set
            {

                if (_backgroundSprite != null)
                {
                    if (_backgroundSprite.Parent != null)
                        _backgroundSprite.Parent.RemoveChild(_backgroundSprite, true);
                }

                _backgroundSprite = value;
                if (value != null)
                {
                    AddChild(_backgroundSprite, 0);

                    RefreshBackgroundSpritePosition();
                }

            }
        }
        public byte Opacity
        {
            get { return _opacity; }
            set
            {

                _opacity = value;

                if (_backgroundSprite != null)
                    _backgroundSprite.Opacity = value;
                if (_thumbSprite != null)
                    _thumbSprite.Opacity = value;

            }
        }

        //OVERRIDE PROPERTIES =================================================
        public override CCPoint Position
        {
            get
            {

                return base.Position;
            }
            set
            {
                base.Position = value;

                //Reposicionamos todo
                RefreshAllPosition();
            }
        }

        public override CCSize ContentSize
        {
            get
            {
                return base.ContentSize;
            }
            set
            {

                if (_backgroundSprite != null)
                {
                    _backgroundSprite.ContentSize = value;
                    _backgroundSprite.Position = new CCPoint(value.Width * 0.5f, value.Height * 0.5f); // new CCPoint(base.ContentSize.Width / 2, base.ContentSize.Height / 2);
                }

                if (_thumbSprite != null)
                {
                    _thumbSprite.ContentSize = value;
                    _thumbSprite.Position = new CCPoint(value.Width * 0.5f, value.Height * 0.5f);
                }

                JoystickRadius = value.Width / 2;

                base.ContentSize = value;
            }
        }

        #endregion

        public SneakyJoystickControlSkinnedBase(CCRect size)
            : base(size)
        {
        }

        public override void UpdateVelocity(CCPoint point)
        {
            base.UpdateVelocity(point);

            if (_thumbSprite != null)
                _thumbSprite.Position = StickPosition;
        }

        public void RefreshBackgroundSpritePosition()
        {
            ContentSize = _backgroundSprite.ContentSize;
            JoystickRadius = (_backgroundSprite.ContentSize.Width / 2);
        }

        public void RefreshThumbSpritePosition()
        {
            _thumbSprite.Position = new CCPoint(ContentSize.Width / 2,
                      ContentSize.Height / 2);

            ThumbRadius = _thumbSprite.ContentSize.Width / 2;
        }

        public void RefreshAllPosition()
        {
            RefreshBackgroundSpritePosition();
            RefreshThumbSpritePosition();
        }

        public static SneakyJoystickControlSkinnedBase Create()
        {
            var size = new CCRect(0, 0, 128, 128);
            return Create(size);
        }

        public static SneakyJoystickControlSkinnedBase Create(CCRect size)
        {
            SneakyJoystickControlSkinnedBase tmp = new SneakyJoystickControlSkinnedBase(size);
            tmp.BackgroundSprite = new CCSprite(DEFAULT_IMAGE_BACKGROUND);  //new ColoredCircleSprite( CCColor4B.Red, 100f);
            tmp.ThumbSprite = new CCSprite(DEFAULT_IMAGE_THUMB);  //new ColoredCircleSprite(CCColor4B.Blue,30f);
            return tmp;
        }


    }
}
