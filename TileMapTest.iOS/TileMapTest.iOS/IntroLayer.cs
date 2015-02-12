using System;
using System.IO;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace TileMapTest.iOS
{
    public class IntroLayer : CCLayerColor
    {
        CCLabelTtf label;

        public IntroLayer()
        {

            // create and initialize a Label
            label = new CCLabelTtf("Hello CocosSharp", "MarkerFelt", 22);
            label.AnchorPoint = CCPoint.AnchorMiddle;

            // add the label as a child to this Layer
            AddChild(label);

            // setup our color for the background
            Color = new CCColor3B(CCColor4B.Blue);
            Opacity = 255;

        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            var tilemap = new CCTileMap("tilemaps/iso-test-zorder.tmx");
            AddChild(tilemap);

            // Uncomment this to test loading from a stream reader with Release > 1.3.1.0
            //
            // Note: the application.ContentSearchPaths.Add("tilemaps"); in AppDelegate.cs module
            //
            // Without a TileMapFileName there is no way to determine the relative offset of the backing 
            // graphic asset so this will have to set in the tile map definition or use a search path added 
            // to the application ContentSearchPaths ex.. application.ContentSearchPaths.Add("images");

//            using (var streamReader = new StreamReader(CCFileUtils.GetFileStream("tilemaps/iso-test-zorder.tmx")))
//            {
//                var tileMap = new CCTileMap(streamReader);
//                AddChild(tileMap);
//            }

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // position the label on the center of the screen
            label.Position = bounds.Center;

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
            }
        }
    }
}

