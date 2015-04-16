using System;
using CocosSharp;

namespace RotateAroundCustomAction
{
    public class CCRotateAroundTo : CCFiniteTimeAction
    {
        public float DistanceAngle { get; private set; }

        public CCPoint Origin { get; private set; }

        public float RotationDirection { get; private set; }
        // -1 = ClockWise / 1 = CounterClockWise

        #region Constructors

        public CCRotateAroundTo(float duration, CCPoint origin, float angle, float rotationDirection = -1)
            : base(duration)
        {
            DistanceAngle = angle;
            Origin = origin;
            RotationDirection = rotationDirection;
        }

        #endregion Constructors

        protected override CCActionState StartAction(CCNode target)
        {
            return new CCRotateAroundToState(this, target);
        }

        public override CCFiniteTimeAction Reverse()
        {
            throw new NotImplementedException();
        }
    }

    public class CCRotateAroundToState : CCFiniteTimeActionState
    {
        protected float DiffAngle;

        protected float theta;

        protected float offsetX;
        protected float offsetY;

        protected float DistanceAngle { get; set; }

        protected float StartAngle;

        protected CCPoint Origin { get; set; }

        protected CCPoint StartPosition { get; set; }

        protected float RotationDirection { get; set; }

        public CCRotateAroundToState(CCRotateAroundTo action, CCNode target)
            : base(action, target)
        { 
            DistanceAngle = action.DistanceAngle;

            Origin = action.Origin;
            StartPosition = target.Position;
            RotationDirection = action.RotationDirection;

            offsetX = StartPosition.X - Origin.X;
            offsetY = StartPosition.Y - Origin.Y;

            // Calculate the Starting Angle of the target in relation to the Origin in which it is to rotate

            // Math.Atan2 returns the mathematical angle which is counter-clockwise [-Math.PI, +Math.PI]
            StartAngle = CCMathHelper.ToDegrees((float)Math.Atan2(offsetY, offsetX));
            // Map value [0,360]
            StartAngle = (StartAngle + 360) % 360;


            // Now we work out how far we actually have to rotate
            DiffAngle = DistanceAngle - StartAngle;
            // Map value [0,360] and take into consideration the direction of rotation - CCW or CW
            DiffAngle = (DiffAngle + 360 * RotationDirection) % 360;

            theta = CCMathHelper.ToRadians(DiffAngle);


        }

        public override void Update(float deltaTime)
        {
            if (Target != null)
            {
                var thetaOverTime = theta * deltaTime;

                // use the CCPoint method RotateByAngle
                //Target.Position = CCPoint.RotateByAngle(StartPosition, Origin, thetaOverTime);

                // or calculate yourself
                float cosTheta = (float)Math.Cos(thetaOverTime);
                float sinTheta = (float)Math.Sin(thetaOverTime);

                // 2d Rotation - http://en.wikipedia.org/wiki/Rotation_(mathematics)
                // x' = x cos(theta) - y sin(theta)
                // y' = x sin(theta) + y cos(theta)
                var x = offsetX * cosTheta - offsetY * sinTheta;
                var y = offsetX * sinTheta + offsetY * cosTheta;

                // now we translate the value back by adding the Origin
                Target.PositionX = x + Origin.X;
                Target.PositionY = y + Origin.Y;

            }
        }

    }

}

