using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CocosSharp.Extensions.SneakyJoystick
{
    public class SneakyButtonControlSkinnedBase : SneakyButtonControl
    {

        #region Static properties

        public static string DEFAULT_IMAGE_BUTTON_RELEASE { get { return "control/button_release"; } }
        public static string DEFAULT_IMAGE_BUTTON_PRESSED { get { return "control/button_pressed"; } }
        public static string DEFAULT_IMAGE_BUTTON_DISABLED { get { return DEFAULT_IMAGE_BUTTON_PRESSED; } }

        #endregion

        #region Private properties

        private CCSprite _defaultSprite;
        private CCSprite _activatedSprite;
        private CCSprite _disabledSprite;
        private CCSprite _pressSprite;
        private byte _opacity;

        #endregion

        #region Public properties

        public CCSprite DefaultSprite
        {
            get
            {
                return _defaultSprite;
            }
            set
            {

                if (_defaultSprite != null)
                {
                    if (_defaultSprite.Parent != null)
                        _defaultSprite.Parent.RemoveChild(_defaultSprite, true);
                }

                _defaultSprite = value;

                if (value != null)
                {
                    //[self addChild:defaultSprite z:0];
                    AddChild(_defaultSprite, 0);
                    ContentSize = _defaultSprite.ContentSize;

                    SetRadius(_defaultSprite.ContentSize.Width / 2);
                    //[self setContentSize:defaultSprite.contentSize];
                }

            }
        }

        public CCSprite ActivatedSprite
        {
            get
            {
                return _activatedSprite;
            }
            set
            {

                if (_activatedSprite != null)
                {
                    if (_activatedSprite.Parent != null)
                        _activatedSprite.Parent.RemoveChild(_activatedSprite, true);
                    //[activatedSprite release];
                }

                _activatedSprite = value;

                if (value != null)
                {
                    AddChild(_activatedSprite, 1);
                    ContentSize = _activatedSprite.ContentSize;
                }


            }
        }

        public CCSprite DisabledSprite
        {
            get { return _disabledSprite; }
            set
            {

                if (_disabledSprite != null)
                {
                    if (_disabledSprite.Parent != null)
                        _disabledSprite.Parent.RemoveChild(_disabledSprite, true);
                    //[activatedSprite release];
                }
                _disabledSprite = value;
                if (value != null)
                {
                    AddChild(_disabledSprite, 2);
                    //[self addChild:activatedSprite z:1];
                    ContentSize = _disabledSprite.ContentSize;
                    //[self setContentSize:activatedSprite.contentSize];
                }

            }
        }

        public CCSprite PressSprite
        {
            get { return _pressSprite; }
            set
            {
                if (_pressSprite != null)
                {
                    if (_pressSprite.Parent != null)
                        _pressSprite.Parent.RemoveChild(_pressSprite, true);
                    //[activatedSprite release];
                }

                _pressSprite = value;

                if (value != null)
                {
                    AddChild(_pressSprite, 3);
                    //[self addChild:activatedSprite z:1];
                    ContentSize = _pressSprite.ContentSize;
                    //[self setContentSize:activatedSprite.contentSize];
                }

            }
        }

        public byte Opacity
        {
            get
            {
                return _opacity;
            }

            set
            {
                if (DefaultSprite != null)
                    DefaultSprite.Opacity = value;

                if (ActivatedSprite != null)
                    ActivatedSprite.Opacity = value;

                if (DisabledSprite != null)
                    DisabledSprite.Opacity = value;

                if (PressSprite != null)
                    PressSprite.Opacity = value;

                _opacity = value;
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

                if (DefaultSprite != null)
                    DefaultSprite.ContentSize = value;

                SetRadius(value.Width / 2);

                base.ContentSize = value;
            }
        }

        #endregion

        #region Contructor

        public SneakyButtonControlSkinnedBase(CCRect rect)
            : base(rect)
        {
            CheckSelf();
        }

        public SneakyButtonControlSkinnedBase(CCRect rect, string defaultSprite, string activatedSprite, string pressSprite, string disabledSprite)
            : this(rect, new CCSprite(defaultSprite), new CCSprite(activatedSprite), new CCSprite(pressSprite), new CCSprite(disabledSprite))
        {

        }

        public SneakyButtonControlSkinnedBase(CCRect rect,
            CCSprite defaultSprite,
            CCSprite activatedSprite,
            CCSprite pressSprite,
            CCSprite disabledSprite)
            : base(rect)
        {

            DefaultSprite = defaultSprite;
            ActivatedSprite = activatedSprite;
            PressSprite = pressSprite;
            DisabledSprite = disabledSprite;

            CheckSelf();
        }

        public SneakyButtonControlSkinnedBase(CCRect rect, string defaultSprite, string activatedSprite, string pressSprite)
            : this(rect, new CCSprite(defaultSprite), new CCSprite(activatedSprite), new CCSprite(pressSprite))
        {

        }

        public SneakyButtonControlSkinnedBase(CCRect rect,
            CCSprite defaultSprite,
            CCSprite activatedSprite,
            CCSprite pressSprite
          )
            : base(rect)
        {

            DefaultSprite = defaultSprite;
            ActivatedSprite = activatedSprite;
            PressSprite = pressSprite;

            CheckSelf();
        }


        #endregion

        /// <summary>
        /// Renaming:  - (void) watchSelf
        /// </summary>
        public override void CheckSelf()
        {
            if (!status)
            {
                if (DisabledSprite != null)
                {
                    DisabledSprite.Visible = true;
                }
                else
                {
                    DisabledSprite.Visible = false;
                }
            }
            else
            {
                if (!active)
                {

                    if (PressSprite != null)
                        PressSprite.Visible = false;

                    if (!value)
                    {
                        if (ActivatedSprite != null)
                            ActivatedSprite.Visible = false;

                        if (DefaultSprite != null)
                            DefaultSprite.Visible = true;
                    }
                    else
                    {
                        if (ActivatedSprite != null)
                            ActivatedSprite.Visible = true;
                    }
                }
                else
                {

                    if (DefaultSprite != null)
                        DefaultSprite.Visible = false;

                    if (PressSprite != null)
                        PressSprite.Visible = true;

                }
            }


        }

        internal static SneakyButtonControlSkinnedBase Create(CCRect size)
        {
            var tmp = new SneakyButtonControlSkinnedBase(size,
              "control/button_release.png", "control/button_pressed.png", "control/button_pressed.png"
              );
            return tmp;
        }

        internal static SneakyButtonControlSkinnedBase Create()
        {
            CCRect size = new CCRect(0, 0, 64, 64);
            return Create(size);

        }






    }
}
