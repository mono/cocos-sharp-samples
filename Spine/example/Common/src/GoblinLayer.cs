using System;
using CocosSharp;
using Microsoft.Xna.Framework;
using CocosSharp.Spine;
using Spine;

namespace spine_cocossharp
{
    class GoblinLayer : CCLayer
    {

        CCSkeletonAnimation skeletonNode;
        CCActionState skeletonActionState;
        CCSequence skeletonMoveAction;
        bool isMoving;

        CCMenuItemFont labelBones, labelSlots, labelTimeScaleUp, labelTimeScaleDown, labelSkin, labelScene, labelAction;
        CCMenu menu;

        public GoblinLayer()
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

            labelSkin = new CCMenuItemFont("S = Toggle Skin", (obj) =>
                {
                    if (skeletonNode.Skeleton.Skin.Name == "goblin")
                        skeletonNode.SetSkin("goblingirl");
                    else
                        skeletonNode.SetSkin("goblin");
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

            labelAction = new CCMenuItemFont("A = Toggle Move Action", (obj) =>
                {
                    if (isMoving)
                    {
                        StopAction(skeletonActionState);
                        isMoving = false;
                    }
                    else
                    {
                        skeletonActionState = skeletonNode.RepeatForever(skeletonMoveAction);
                        isMoving = true;
                    }
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            labelScene = new CCMenuItemFont("P = SpineBoy", (obj) =>
                {
                    Director.ReplaceScene(SpineBoyLayer.Scene(Window));
                }

            ) { AnchorPoint = CCPoint.AnchorMiddleLeft };

            menu = new CCMenu(labelBones, labelSlots, labelSkin, labelTimeScaleUp, labelTimeScaleDown, labelAction, labelScene);
            menu.AlignItemsVertically();
            AddChild(menu);

            String name = @"goblins-ffd";
            //String name = @"goblins";
            skeletonNode = new CCSkeletonAnimation(name + ".json", name + ".atlas", 0.5f);
            skeletonNode.PremultipliedAlpha = true;

            skeletonNode.SetSkin("goblin");

            //var wt = skeletonNode.NodeToWorldTransform;
            skeletonNode.SetSlotsToSetupPose();
            skeletonNode.UpdateWorldTransform();

            skeletonNode.AddAnimation(0, "walk", true, 4);
            skeletonNode.SetAnimation(0, "walk", true);

            skeletonNode.Start += Start;
            skeletonNode.End += End;
            skeletonNode.Complete += Complete;
            skeletonNode.Event += Event;

            //skeletonNode.RepeatForever(new CCFadeOut(1), new CCFadeIn(1));

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
                    else if (skeletonNode.Skeleton.Skin.Name == "goblin")
                        skeletonNode.SetSkin("goblingirl");
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
                        case CCKeys.S:
                            if (skeletonNode.Skeleton.Skin.Name == "goblin")
                                skeletonNode.SetSkin("goblingirl");
                            else
                                skeletonNode.SetSkin("goblin");
                            break;
                        case CCKeys.Up:
                            skeletonNode.TimeScale += 0.1f;
                            break;
                        case CCKeys.Down:
                            skeletonNode.TimeScale -= 0.1f;
                            break;
                        case CCKeys.A:
                            if (isMoving)
                            {
								StopAction(skeletonActionState);
                                isMoving = false;
                            }
                            else
                            {
                                skeletonActionState = skeletonNode.RepeatForever(skeletonMoveAction);
                                isMoving = true;
                            }
                            break;
                        case CCKeys.P:
							Director.ReplaceScene(SpineBoyLayer.Scene(Window));
                            break;
                    }

                };
			AddEventListener(keyListener, this);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();
        
            var windowSize = VisibleBoundsWorldspace.Size;

            menu.Position = new CCPoint(15, windowSize.Height - 85);
//			labelBones.Position = new CCPoint(15, windowSize.Height - 10);
//			labelSlots.Position = new CCPoint(15, windowSize.Height - 25);
//			labelSkin.Position = new CCPoint(15, windowSize.Height - 40);
//			labelTimeScale.Position = new CCPoint(15, windowSize.Height - 70);
//			labelAction.Position = new CCPoint(15, windowSize.Height - 55);
//			labelScene.Position = new CCPoint(15, windowSize.Height - 85);

			skeletonNode.Position = new CCPoint(windowSize.Center.X, skeletonNode.ContentSize.Height / 2);

			skeletonMoveAction = new CCSequence(new CCMoveTo(5, new CCPoint(windowSize.Width, 10)), new CCMoveTo(5, new CCPoint(10, 10)));

			skeletonActionState = skeletonNode.RepeatForever(skeletonMoveAction);
			isMoving = true;


		}

        public void Start(AnimationState state, int trackIndex)
        {
            CCLog.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": start");
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
            var layer = new GoblinLayer();

            // add layer as a child to scene
            scene.AddChild(layer);

            // return the scene
            return scene;

        }

    }
}

