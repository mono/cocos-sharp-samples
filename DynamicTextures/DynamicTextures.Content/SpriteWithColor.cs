using System;
using CocosSharp;
using Microsoft.Xna.Framework.Graphics;

namespace DynamicTextures
{
    public class SpriteWithColor : CCSprite
    {

        public SpriteWithColor(CCColor4B bgColor, float contentWidth, float contentHeight) 
            : this(bgColor, new CCSize(contentWidth, contentHeight))

        {
        }

        public SpriteWithColor(CCColor4B bgColor, CCSize contentSize) : base ()
        {

            CCRenderTexture rt = new CCRenderTexture(contentSize, contentSize);

            rt.BeginWithClear(bgColor);

            GenerateGradient(contentSize);

            // Load our sprite. By default the content size of this sprite may not match our desired contentsize
            // So let's change it
            var noise = new CCSprite("images/Noise.png");
            noise.ContentSize = contentSize;

            // Now the texture may be scaled to fit the content size so let's make sure that we're wrapping the texture instead
            noise.Texture.SamplerState = SamplerState.LinearWrap;

            noise.AnchorPoint = CCPoint.AnchorLowerLeft;
            noise.Position = CCPoint.Zero;
            noise.BlendFunc = new CCBlendFunc(CCOGLES.GL_DST_COLOR, CCOGLES.GL_ZERO);
            noise.Visit();

            rt.End();

            Texture = rt.Texture;
            ContentSize = contentSize;
            Texture.SamplerState = SamplerState.LinearWrap;

        }

        /*
         *  TL    TR
         *   0----1 0,1,2,3 = index offsets for vertex indices
         *   |   /| 
         *   |  / |
         *   | /  |
         *   |/   |
         *   2----3
         *  BL    BR
         */
        void GenerateGradient (CCSize textureSizeInPixels)
        {
            var gradientNode = new CCDrawNode();

            var gradientAlpha = new CCColor4B(0, 0, 0,(byte)(0.7f * 255f));
            CCV3F_C4B[] vertices = new CCV3F_C4B[6];

            // Left triangle TL - 0
            vertices[0].Vertices = new CCVertex3F(0, textureSizeInPixels.Height, 0);
            vertices[0].Colors = CCColor4B.Transparent;

            // Left triangle BL - 2
            vertices[1].Vertices = new CCVertex3F(0, 0, 0);
            vertices[1].Colors = gradientAlpha;

            // Left triangle TR - 1
            vertices[2].Vertices = new CCVertex3F(textureSizeInPixels.Width, textureSizeInPixels.Height, 0);
            vertices[2].Colors = CCColor4B.Transparent;

            // Right triangle BL - 2
            vertices[3].Vertices = new CCVertex3F(0, 0, 0);
            vertices[3].Colors = gradientAlpha;

            // Right triangle BR - 3
            vertices[4].Vertices = new CCVertex3F(textureSizeInPixels.Width, 0, 0);
            vertices[4].Colors = gradientAlpha;

            // Right triangle TR - 1
            vertices[5].Vertices = new CCVertex3F(textureSizeInPixels.Width, textureSizeInPixels.Height, 0);
            vertices[5].Colors = CCColor4B.Transparent;

            gradientNode.DrawTriangleList(vertices);

            gradientNode.Visit();

        }

    }
}