using System;
using CocosSharp;

namespace DynamicTextures
{
    public class SpriteWithColor : CCSprite
    {

        public SpriteWithColor(CCColor4B bgColor, float textureWidth, float textureHeight) 
            : this(bgColor, new CCSize(textureWidth, textureHeight))

        {

        }

        public SpriteWithColor(CCColor4B bgColor, CCSize textureSizeInPixels) : base ()
        {
            // 1: Create new CCRenderTexture
            CCRenderTexture rt = new CCRenderTexture(textureSizeInPixels, textureSizeInPixels);

            // 2: Call CCRenderTexture:begin
            rt.BeginWithClear(bgColor);

            // 3: Draw into the texture
            // You'll add this later
            GenerateGradient(textureSizeInPixels);


            var noise = new CCSprite("images/Noise.png");
            noise.AnchorPoint = CCPoint.AnchorLowerLeft;
            noise.Position = CCPoint.Zero;
            noise.BlendFunc = new CCBlendFunc(CCOGLES.GL_DST_COLOR, CCOGLES.GL_ZERO);
            noise.Texture.SamplerState = Microsoft.Xna.Framework.Graphics.SamplerState.LinearWrap;
            noise.TextureRectInPixels = new CCRect(0, 0, textureSizeInPixels.Width, textureSizeInPixels.Height);
            noise.Visit();

            // 4: Call CCRenderTexture:end
            rt.End();

            this.Texture = rt.Texture;

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