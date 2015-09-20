using System;
using CocosSharp;
using Spine;


namespace CocosSharp.Spine
{


	public class CocosSharpTextureLoader : TextureLoader
	{


		CCTexture2D texture;


		#region Public


		public void Load (AtlasPage page, String path)
		{
			texture = CCTextureCache.SharedTextureCache.AddImage (path);

			page.rendererObject = texture;
			page.width = (int)texture.ContentSizeInPixels.Width;
			page.height = (int)texture.ContentSizeInPixels.Height;
		}


		public void Unload (Object texture)
		{
			((CCTexture2D)texture).Dispose ();
		}


		#endregion


	}


}
