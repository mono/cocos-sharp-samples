using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;

namespace TextField
{
    public class IntroLayer : CCLayerColor
    {

        // Define a textfield variable
        CCTextField trackNode;
        CCPoint beginPosition;

        // scrolling actions
        CCMoveTo scrollUp;
        CCMoveTo scrollDown;

        // Clink when editing
        static CCFiniteTimeAction textFieldAction = new CCBlink(1.0f, 1);

        // State tracking 
        CCActionState textFieldActionState;

        bool action;

        public IntroLayer()
            : base(CCColor4B.Blue)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            var s = VisibleBoundsWorldspace.Size;

            var textField = new CCTextField("[click here for input]",
                "fonts/MarkerFelt",
                22,
                CCLabelFormat.SpriteFont);

            textField.BeginEditing += OnBeginEditing;
            textField.EndEditing += OnEndEditing;
            textField.Position = s.Center;

            textField.AutoEdit = true;

            AddChild(textField);

            TrackNode = textField;
            scrollUp = new CCMoveTo(0.5f, VisibleBoundsWorldspace.Top() - new CCPoint(0, s.Height / 4));
            scrollDown = new CCMoveTo(0.5f, textField.Position);

        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Register Touch Event
            var touchListener = new CCEventListenerTouchOneByOne();
            touchListener.IsSwallowTouches = true;

            touchListener.OnTouchBegan = OnTouchBegan;
            touchListener.OnTouchEnded = OnTouchEnded;

            AddEventListener(touchListener);


        }

        protected CCTextField TrackNode
        {
            get { return trackNode; }
            set
            {
                if (value == null)
                {
                    if (trackNode != null)
                    {
                        DetachListeners();
                        trackNode = value;
                        return;
                    }
                }

                if (trackNode != value)
                {
                    DetachListeners();
                }

                trackNode = value;
                AttachListeners();
            }
        }

        bool OnTouchBegan(CCTouch pTouch, CCEvent touchEvent)
        {
            beginPosition = pTouch.Location;
            return true;
        }

        void OnTouchEnded(CCTouch pTouch, CCEvent touchEvent)
        {
            if (trackNode == null)
            {
                return;
            }

            var endPos = pTouch.Location;

            if (trackNode.BoundingBox.ContainsPoint(beginPosition) && trackNode.BoundingBox.ContainsPoint(endPos))
            {
                OnClickTrackNode(true);
            }
            else
            {
                OnClickTrackNode(false);
            }
        }

        public void OnClickTrackNode(bool bClicked)
        {
            if (bClicked && TrackNode != null)
            {
                TrackNode.Edit();
            }
            else
            {
                if (TrackNode != null)
                {
                    TrackNode.EndEdit();
                }
            }

        }

        private void OnEndEditing(object sender, ref string text, ref bool canceled)
        {
            ((CCNode)sender).RunAction(scrollDown);
        }

        private void OnBeginEditing(object sender, ref string text, ref bool canceled)
        {
            ((CCNode)sender).RunAction(scrollUp);
        }

        void AttachListeners()
        {
            // Attach our listeners.
            var imeImplementation = trackNode.TextFieldIMEImplementation;
            imeImplementation.KeyboardDidHide += OnKeyboardDidHide;
            imeImplementation.KeyboardDidShow += OnKeyboardDidShow;
            imeImplementation.KeyboardWillHide += OnKeyboardWillHide;
            imeImplementation.KeyboardWillShow += OnKeyboardWillShow;

        }

        void DetachListeners()
        {
            if (TrackNode != null)
            {
                // Remember to remove our event listeners.
                var imeImplementation = TrackNode.TextFieldIMEImplementation;
                imeImplementation.KeyboardDidHide -= OnKeyboardDidHide;
                imeImplementation.KeyboardDidShow -= OnKeyboardDidShow;
                imeImplementation.KeyboardWillHide -= OnKeyboardWillHide;
                imeImplementation.KeyboardWillShow -= OnKeyboardWillShow;
            }
        }

        void OnKeyboardWillShow(object sender, CCIMEKeyboardNotificationInfo e)
        {
            CCLog.Log("Keyboard will show");
        }

        void OnKeyboardWillHide(object sender, CCIMEKeyboardNotificationInfo e)
        {
            CCLog.Log("Keyboard will Hide");
        }

        void OnKeyboardDidShow(object sender, CCIMEKeyboardNotificationInfo e)
        {
            CCLog.Log("Keyboard did show");
            if (!action)
            {
                textFieldActionState = TrackNode.RepeatForever(textFieldAction);
                action = true;
            }
        }

        void OnKeyboardDidHide(object sender, CCIMEKeyboardNotificationInfo e)
        {
            CCLog.Log("Keyboard did hide");
            if (action)
            {
                TrackNode.StopAction(textFieldActionState);
                // Make sure we are in a visible state after the action ends.
                TrackNode.Visible = true;

                action = false;
            }

        }

    }
}

