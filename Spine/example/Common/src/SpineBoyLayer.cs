using System;
using CocosSharp;
using Microsoft.Xna.Framework;
using CocosSharp.Spine;
using Spine;

namespace spine_cocossharp
{
    class SpineBoyLayer : CCLayer
    {

        CCSkeletonAnimation skeletonNode;

		CCMenuItemFont labelBones, labelSlots, labelTimeScaleUp, labelTimeScaleDown, labelScene, labelJump;
        CCMenu menu;

        public SpineBoyLayer()
        {
            CCMenuItemFont.FontName = "arial";
            CCMenuItemFont.FontSize = 12;

            labelBones = new CCMenuItemFont("B = Toggle Debug Bones", (obj) =>
                {
                    skeletonNode.DebugBones = !skeletonNode.DebugBones;
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelSlots = new CCMenuItemFont("M = Toggle Debug Slots", (obj) =>
                {
                    skeletonNode.DebugSlots = !skeletonNode.DebugSlots;
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelTimeScaleUp = new CCMenuItemFont("Up - TimeScale +", (obj) =>
                {
                    skeletonNode.TimeScale += 0.1f;
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelTimeScaleDown = new CCMenuItemFont("Down - TimeScale -", (obj) =>
                {
                    skeletonNode.TimeScale -= 0.1f;
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelScene = new CCMenuItemFont("G = Goblins", (obj) =>
                {
                    Director.ReplaceScene(GoblinLayer.Scene(Window));
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelJump = new CCMenuItemFont("J = Jump", (obj) =>
                {
                    // I truthfully do not know if this is how it is done or not
                    skeletonNode.SetAnimation(0, "jump", false);
                    skeletonNode.AddAnimation(0, "run", true);
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            menu = new CCMenu(labelBones, labelSlots, labelTimeScaleUp, labelTimeScaleDown, labelJump, labelScene);
            menu.AlignItemsVertically();
            AddChild(menu);

			String name = @"spineboy";
            skeletonNode = new CCSkeletonAnimation(name + ".json", name + ".atlas", 0.25f);

            skeletonNode.SetMix("walk", "jump", 0.2f);
            skeletonNode.SetMix("jump", "run", 0.2f);
            skeletonNode.SetAnimation(0, "walk", true);
			TrackEntry jumpEntry = skeletonNode.AddAnimation(0, "jump", false, 3);
            skeletonNode.AddAnimation(0, "run", true);

            skeletonNode.Start += Start;
            skeletonNode.End += End;
            skeletonNode.Complete += Complete;
            skeletonNode.Event += Event;

            AddChild(skeletonNode);

            var listener = new CCEventListenerTouchOneByOne();
            listener.OnTouchBegan = (touch, touchEvent) =>
                {
                    if (!skeletonNode.DebugBones)
                    {
                        skeletonNode.DebugBones = true; 
                    }
                    else if (skeletonNode.TimeScale == 1)
                        skeletonNode.TimeScale = 0.3f;
                    return true;
                };
			AddEventListener(listener, this);

            var keyListener = new CCEventListenerKeyboard();
            keyListener.OnKeyPressed = (keyEvent) =>
                {
                    switch (keyEvent.Keys)
                    {
                        case CCKeys.B:
                            skeletonNode.DebugBones = !skeletonNode.DebugBones;
                            break;
                        case CCKeys.M:
                            skeletonNode.DebugSlots = !skeletonNode.DebugSlots;
                            break;
                        case CCKeys.Up:
                            skeletonNode.TimeScale += 0.1f;
                            break;
                        case CCKeys.Down:
                            skeletonNode.TimeScale -= 0.1f;
                            break;
                        case CCKeys.G:
                            Director.ReplaceScene(GoblinLayer.Scene(Window));
                            break;
						case CCKeys.J:
							// I truthfully do not know if this is how it is done or not
							skeletonNode.SetAnimation(0, "jump", false);
							skeletonNode.AddAnimation(0, "run", true);
							break;
                    }

                };
			AddEventListener(keyListener, this);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            var windowSize = VisibleBoundsWorldspace.Size;

            menu.Position = new CCPoint(15, windowSize.Height - 70);

			skeletonNode.Position = new CCPoint(windowSize.Center.X, 10);

		}


        public void Start(AnimationState state, int trackIndex)
        {
            var entry = state.GetCurrent(trackIndex);
            var animationName = (entry != null && entry.Animation != null) ? entry.Animation.Name : string.Empty;

            CCLog.Log(trackIndex + ":start " + animationName);
        }

        public void End(AnimationState state, int trackIndex)
        {
            CCLog.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": end");
        }

        public void Complete(AnimationState state, int trackIndex, int loopCount)
        {
            CCLog.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": complete " + loopCount);
        }

        public void Event(AnimationState state, int trackIndex, Event e)
        {
            CCLog.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": event " + e);
        }

        public static CCScene Scene(CCWindow window)
        {
                // 'scene' is an autorelease object.
                var scene = new CCScene(window);

                // 'layer' is an autorelease object.
                var layer = new SpineBoyLayer();

                // add layer as a child to scene
                scene.AddChild(layer);

                // return the scene
                return scene;

        }

    }
}

