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

        private CCSprite defaultSprite;
        private CCSprite activatedSprite;
        private CCSprite disabledSprite;
        private CCSprite pressSprite;
        private byte opacity;

        #endregion

        #region Public properties

        public CCSprite DefaultSprite
        {
            get
            {
                return defaultSprite;
            }
            set
            {

                if (defaultSprite != null)
                {
                    if (defaultSprite.Parent != null)
                        defaultSprite.Parent.RemoveChild(defaultSprite, true);
                }

                defaultSprite = value;

                if (value != null)
                {
                    //[self addChild:defaultSprite z:0];
                    AddChild(defaultSprite, 0);
                    ContentSize = defaultSprite.ContentSize;

                    SetRadius(defaultSprite.ContentSize.Width / 2);
                    //[self setContentSize:defaultSprite.contentSize];
                }

            }
        }

        public CCSprite ActivatedSprite
        {
            get
            {
                return activatedSprite;
            }
            set
            {

                if (activatedSprite != null)
                {
                    if (activatedSprite.Parent != null)
                        activatedSprite.Parent.RemoveChild(activatedSprite, true);
                    //[activatedSprite release];
                }

                activatedSprite = value;

                if (value != null)
                {
                    AddChild(activatedSprite, 1);
                    ContentSize = activatedSprite.ContentSize;
                }


            }
        }

        public CCSprite DisabledSprite
        {
            get { return disabledSprite; }
            set
            {

                if (disabledSprite != null)
                {
                    if (disabledSprite.Parent != null)
                        disabledSprite.Parent.RemoveChild(disabledSprite, true);
                    //[activatedSprite release];
                }
                disabledSprite = value;
                if (value != null)
                {
                    AddChild(disabledSprite, 2);
                    //[self addChild:activatedSprite z:1];
                    ContentSize = disabledSprite.ContentSize;
                    //[self setContentSize:activatedSprite.contentSize];
                }

            }
        }

        public CCSprite PressSprite
        {
            get { return pressSprite; }
            set
            {
                if (pressSprite != null)
                {
                    if (pressSprite.Parent != null)
                        pressSprite.Parent.RemoveChild(pressSprite, true);
                    //[activatedSprite release];
                }

                pressSprite = value;

                if (value != null)
                {
                    AddChild(pressSprite, 3);
                    //[self addChild:activatedSprite z:1];
                    ContentSize = pressSprite.ContentSize;
                    //[self setContentSize:activatedSprite.contentSize];
                }

            }
        }

        public byte Opacity
        {
            get
            {
                return opacity;
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

                opacity = value;
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

        public SneakyButtonControlSkinnedBase(int ID)
            : this(new CCRect(0, 0, 64, 64), ID)
        { }


        public SneakyButtonControlSkinnedBase(CCRect size, int ID)
            : this(size, "control/button_release.png", "control/button_pressed.png", "control/button_pressed.png", ID)
        { }



        public SneakyButtonControlSkinnedBase(CCRect rect, string defaultSprite, string activatedSprite, string pressSprite, string disabledSprite, int ID)
            : this(rect, new CCSprite(defaultSprite), new CCSprite(activatedSprite), new CCSprite(pressSprite), new CCSprite(disabledSprite), ID)
        { }

        public SneakyButtonControlSkinnedBase(CCRect rect,
            CCSprite defaultSprite,
            CCSprite activatedSprite,
            CCSprite pressSprite,
            CCSprite disabledSprite, int ID)
            : base(rect, ID)
        {

            DefaultSprite = defaultSprite;
            ActivatedSprite = activatedSprite;
            PressSprite = pressSprite;
            DisabledSprite = disabledSprite;

            CheckSelf();
        }

        public SneakyButtonControlSkinnedBase(CCRect rect, string defaultSprite, string activatedSprite, string pressSprite, int ID)
            : this(rect, new CCSprite(defaultSprite), new CCSprite(activatedSprite), new CCSprite(pressSprite), ID)
        {

        }

        public SneakyButtonControlSkinnedBase(CCRect rect,
            CCSprite defaultSprite,
            CCSprite activatedSprite,
            CCSprite pressSprite, int ID
          )
            : base(rect, ID)
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


    }
}
