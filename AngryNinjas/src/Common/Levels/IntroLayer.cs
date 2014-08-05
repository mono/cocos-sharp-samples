using System;
using Microsoft.Xna.Framework;
using CocosSharp;


namespace AngryNinjas
{
	public class IntroLayer : CCLayer
	{

		CCSize _screenSize;

		public IntroLayer(CCSize size)
			: base(size)
		{

		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

			//get screen size
			_screenSize = Window.WindowSizeInPixels; //CCDirector::sharedDirector()->getWinSize();


			var background = new CCSprite("IntroLayer");

			background.Position = new CCPoint(_screenSize.Width / 2, _screenSize.Height / 2);

			// add the background as a child to this Layer
			AddChild(background);

			// Wait a little and then transition to the new scene
			ScheduleOnce(MakeTransition, 2);
		}


		public void MakeTransition(float dt)
		{
			CCLog.Log("Make Transition to Level");
			// CCDirector.SharedDirector.ReplaceScene(new CCTransitionFade(1, TheLevel.Scene, CCColor3B.White));

			Director.ReplaceScene(TheLevel.GetScene(Window));


		}



	}
}

