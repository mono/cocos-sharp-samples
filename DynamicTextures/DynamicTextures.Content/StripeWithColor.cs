using System;
using CocosSharp;
using Microsoft.Xna.Framework.Graphics;

namespace DynamicTextures
{
    public class StripeWithColor : CCSprite
    {
        public StripeWithColor(CCColor4B c1, CCColor4B c2, float textureWidth, float textureHeight, int nStripes) 
            : this(c1, c2, new CCSize(textureWidth, textureHeight), nStripes)

        {

        }

        public StripeWithColor(CCColor4B c1, CCColor4B c2, CCSize contentSize, int nStripes) : base ()
        {
            // 1: Create new CCRenderTexture
            CCRenderTexture rt = new CCRenderTexture(contentSize, contentSize);

            // 2: Call CCRenderTexture:begin
            rt.BeginWithClear(c1);

            // 3: Draw into the texture
            // You'll add this later
            GenerateStripes(contentSize, c2, nStripes);

            var noise = new CCSprite("images/Noise.png");
            noise.AnchorPoint = CCPoint.AnchorLowerLeft;
            noise.BlendFunc = new CCBlendFunc(CCOGLES.GL_DST_COLOR, CCOGLES.GL_ZERO);
            noise.Visit();

            // 4: Call CCRenderTexture:end
            rt.End();

            Texture = rt.Texture;
            Texture.SamplerState = SamplerState.LinearWrap;
            ContentSize = contentSize;
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
        void GenerateStripes (CCSize textureSizeInPixels, CCColor4B c2, int numberOfStripes)
        {
            var gradientNode = new CCDrawNode();

            // Layer 1: Stripes
            CCV3F_C4B[] vertices = new CCV3F_C4B[numberOfStripes*6];

            var textureWidth = textureSizeInPixels.Width;
            var textureHeight = textureSizeInPixels.Height;

            int nVertices = 0;
            float x1 = -textureHeight;
            float x2;
            float y1 = textureHeight;
            float y2 = 0;
            float dx = textureWidth / numberOfStripes * 2;
            float stripeWidth = dx/2;

            for (int i=0; i<numberOfStripes; i++) 
            {

                x2 = x1 + textureHeight;

                // Left triangle TL - 0
                vertices[nVertices].Vertices = new CCVertex3F(x1, y1, 0);
                vertices[nVertices++].Colors = c2;

                // Left triangle BL - 2
                vertices[nVertices].Vertices = new CCVertex3F(x1+stripeWidth, y1, 0);
                vertices[nVertices++].Colors = c2;

                // Left triangle TR - 1
                vertices[nVertices].Vertices = new CCVertex3F(x2, y2, 0);
                vertices[nVertices++].Colors = c2;

                // Right triangle BL - 2
                vertices[nVertices].Vertices = vertices[nVertices-2].Vertices;
                vertices[nVertices++].Colors = c2;

                // Right triangle BR - 3
                vertices[nVertices].Vertices = vertices[nVertices-2].Vertices;
                vertices[nVertices++].Colors = c2;

                // Right triangle TR - 1
                vertices[nVertices].Vertices = new CCVertex3F(x2+stripeWidth, y2, 0);
                vertices[nVertices++].Colors = c2;

                x1 += dx;
            }

            gradientNode.DrawTriangleList(vertices);

            gradientNode.Visit();

        }

    }
}