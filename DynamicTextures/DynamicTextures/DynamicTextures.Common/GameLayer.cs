using System;
using System.Collections.Generic;
using CocosSharp;

namespace DynamicTextures
{
    public class GameLayer : CCLayer
    {

        CCSprite backGround;
        static float scrollingOffset = 0;

        Action genBackground;

        public GameLayer()
        {
            // Select our Background generation
            genBackground = new Action(() =>
                {
                    Unschedule();
                    GradientBackground();
                    Schedule();
                });
//            genBackground = new Action(() => 
//                {
//                    Unschedule();
//                    StripedBackground();
//                    Schedule();
//                });

            // Make any renderable node objects (e.g. sprites) children of this layer

        }

        public override void OnEnter()
        {
            base.OnEnter();

            genBackground();
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (backGround != null)
            {
                float PIXELS_PER_SECOND = 100;
                scrollingOffset += PIXELS_PER_SECOND * dt;

                var textureSize = backGround.TextureRectInPixels.Size;

                backGround.TextureRectInPixels = new CCRect(scrollingOffset, 0, textureSize.Width, textureSize.Height);
            }

        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            CCRect bounds = VisibleBoundsWorldspace;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here
                genBackground();
            }
        }

        byte RandomColorValue()
        {
            var random = CCRandom.GetRandomInt(0, 255);
            return (byte)random;
        }

        CCColor4B RandomBrightColor ()
        {

            while (true) {
                float requiredBrightness = 192;
                CCColor4B randomColor = new CCColor4B (
                    RandomColorValue(),
                    RandomColorValue(), 
                    RandomColorValue(), 
                    255);
                if (randomColor.R > requiredBrightness || 
                    randomColor.G > requiredBrightness ||
                    randomColor.B > requiredBrightness) 
                {
                    return randomColor;
                }        
            }

        }

        void GradientBackground ()
        {
            if (backGround != null)
                backGround.RemoveFromParent();

            // Use the bounds to layout the positioning of our drawable assets
            CCRect bounds = VisibleBoundsWorldspace;

            // MUST be power of 2 in order to create a continue effect.
            // eg: 32x64, 512x128, 256x1024, 64x64, etc..
            var textureWidth = bounds.Size.Width.NextPOT();
            var textureHeight = bounds.Size.Height.NextPOT();

            backGround = new SpriteWithColor(RandomBrightColor(), textureWidth, textureHeight);

            backGround.AnchorPoint = CCPoint.AnchorLowerLeft;
            backGround.Position = CCPoint.Zero;
            AddChild(backGround, -1);  

        }

        void StripedBackground ()
        {
            if (backGround != null)
                backGround.RemoveFromParent();

            // Use the bounds to layout the positioning of our drawable assets
            CCRect bounds = VisibleBoundsWorldspace;

            var backGroundColor = RandomBrightColor();
            var stripeColor = RandomBrightColor();

            int minStripes = 4;
            int maxStripes = 10;

            // generate an even number of stripes or we will see offset patterns when using
            // linear wrap and scrolling.
            int numberOfStripes = 2 * CCRandom.GetRandomInt(minStripes / 2, maxStripes / 2);
            CCLog.Log("number of stripes " + numberOfStripes);

            backGround = new StripeWithColor(backGroundColor, stripeColor, 512, 512, numberOfStripes);

            backGround.AnchorPoint = CCPoint.AnchorLowerLeft;

            // MUST be power of 2 in order to create a continue effect.
            // eg: 32x64, 512x128, 256x1024, 64x64, etc..
            var textureWidth = bounds.Size.Width.NextPOT();
            var textureHeight = bounds.Size.Height.NextPOT();

            backGround.Texture.SamplerState = Microsoft.Xna.Framework.Graphics.SamplerState.LinearWrap;
            backGround.TextureRectInPixels = new CCRect(0,0,textureWidth,textureHeight);

            AddChild(backGround, -1);  

        }
    }

}
