using System;
using Microsoft.Xna.Framework;
using CocosSharp;


namespace AngryNinjas
{
    public class IntroLayer : CCLayer
    {
        public override void OnEnter()
        {
            base.OnEnter();

            // ask director for the window size
            var size = Director.WindowSizeInPixels;

            var background = new CCSprite("IntroLayer");

            background.Position = new CCPoint(size.Width / 2, size.Height / 2);

            // add the background as a child to this Layer
            AddChild(background);

            // Wait a little and then transition to the new scene
            ScheduleOnce(MakeTransition, 2);
        }


        public void MakeTransition(float dt)
        {
            CCLog.Log("Make Transition to Level");
            // CCDirector.SharedDirector.ReplaceScene(new CCTransitionFade(1, TheLevel.Scene, CCColor3B.White));

            Director.ReplaceScene(TheLevel.Scene);


        }

        public static CCScene Scene
        {

            get
            {
                CCScene scene = new CCScene();

                IntroLayer layer = new IntroLayer();

                // add layer as a child to scene
                scene.AddChild(layer);

                // return the scene
                return scene;
            }
        }


    }
}

