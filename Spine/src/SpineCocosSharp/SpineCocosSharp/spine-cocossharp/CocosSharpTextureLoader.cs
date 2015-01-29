using System;
using System.IO;
using CocosSharp;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace CocosSharp.Spine
{
	public class CocosSharpTextureLoader : TextureLoader 
	{
		public CocosSharpTextureLoader ()
		{
		}

		CCTexture2D texture;

		public void Load (AtlasPage page, String path) 
		{
			var ccTexture = CCTextureCache.SharedTextureCache.AddImage(path);
            texture = ccTexture;

            page.rendererObject = texture;
            page.width = (int)texture.ContentSizeInPixels.Width;
            page.height = (int)texture.ContentSizeInPixels.Height;
		}

		public void Unload (Object texture) {
			((CCTexture2D)texture).Dispose();
		}
	}
}

