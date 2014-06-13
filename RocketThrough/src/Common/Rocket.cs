using System;

using Microsoft.Xna.Framework;

using CocosSharp;

namespace RocketThrought.Common
{

    public class Rocket : GameSprite
    {

        public static int ROTATE_NONE = 1;
        public static int ROTATE_CLOCKWISE = 2;
        public static int ROTATE_COUNTER = 3;

        public float _targetRotation;
        public float _dr;
        public float _ar;
        public float _vr;
        public float _rotationSpring = 0;
        public float _rotationDamping = 0;

        public CCPoint _vector;
        public CCPoint _pivot;
        public float _speed = 0;
        public float _angularSpeed = 0;
        public int _rotationOrientation;

        public Rocket(string spritename)
            : base(spritename)
        {
            _targetRotation = 0;
            _dr = 0;
            _ar = 0;
            _vr = 0;
            _rotationSpring = 0.2f;
            _rotationDamping = 0.5f;
            _pivot = new CCPoint(-1, -1);
            _rotationOrientation = ROTATE_NONE;
            reset();
        }

        public void setTargetRotation(float value)
        {
            _targetRotation = value;
        }

        public void setRotationFromVector()
        {
            _targetRotation = CCMathHelper.ToDegrees((float)Math.Atan2(-_vector.Y, _vector.X));
            //this->setTargetRotation(CC_RADIANS_TO_DEGREES( atan2(-_vector.y, _vector.x) ) );
        }

        public void reset()
        {
            _speed = 50;
            _pivot = new CCPoint(-1, 1);
            _rotationOrientation = ROTATE_NONE;
            Rotation = -90;
            _targetRotation = -90;
            float angle = CCMathHelper.ToRadians(RotationX);
            _vector = new CCPoint(_speed * (float)Math.Cos(angle),
                           -_speed * (float)Math.Sin(angle));

        }

        public override void Update(float dt)
        {
            base.Update(dt);

            CCPoint position = Position;

            if (_rotationOrientation == ROTATE_NONE)
            {
                position.X += _vector.X * dt;
                position.Y += _vector.Y * dt;
            }
            else
            {
                //rotate point around a pivot by a certain amount (rotation angle)
                CCPoint rotatedPoint = CCPoint.RotateByAngle(position, _pivot, _angularSpeed * dt);

                position.X = rotatedPoint.X;
                position.Y = rotatedPoint.Y;

                float rotatedAngle;

                CCPoint clockwise = CCPoint.Perp(position - _pivot);

                if (_rotationOrientation == ROTATE_COUNTER)
                {
                    rotatedAngle = (float)Math.Atan2(-1 * clockwise.Y, -1 * clockwise.X);
                }
                else
                {
                    rotatedAngle = (float)Math.Atan2(clockwise.Y, clockwise.X);
                }

                //update rocket vector
                _vector.X = _speed * (float)Math.Cos(rotatedAngle);
                _vector.Y = _speed * (float)Math.Sin(rotatedAngle);


                setRotationFromVector();
                //float m = time % slice;
                //wrap rotation values to 0-360 degrees
                if (RotationX > 0)
                {
                    Rotation = RotationX % 360.0f;
                }
                else
                {
                    Rotation = RotationX % -360.0f;
                }
            }

            if (_targetRotation > RotationX + 180)
            {
                _targetRotation -= 360;
            }
            if (_targetRotation < RotationX - 180)
            {
                _targetRotation += 360;
            }

            Position = position;
            //this->setPosition(position);

            _dr = _targetRotation - RotationX;
            _ar = _dr * _rotationSpring;
            _vr += _ar;
            _vr *= _rotationDamping;



            //m_fRotation += _vr;
            //TODO: REPASAR
            RotationX += _vr;
            RotationY += _vr;

        }

        public void select(bool flag)
        {
            if (flag)
            {


                DisplayFrame = CCApplication.SharedApplication.SpriteFrameCache["rocket_on.png"];
            }
            else
            {
                DisplayFrame = CCApplication.SharedApplication.SpriteFrameCache["rocket.png"];// CCSpriteFrameCache.SharedSpriteFrameCache.SpriteFrameByName("rocket.png");
            }
        }

        public bool collidedWithSides()
        {

            CCSize screenSize = Director.WindowSizeInPixels;

            //TODO: REPASAR
            //Rotation += _vr;

            if (Position.X > screenSize.Width - _radius)
            {
                PositionX = screenSize.Width - _radius;
                _rotationOrientation = ROTATE_NONE;
                _vector = new CCPoint(_vector.X * -1, _vector.Y);
                setRotationFromVector();
                return true;
            }

            if (Position.X < _radius)
            {
                PositionX = _radius;
                _rotationOrientation = ROTATE_NONE;

                _vector = new CCPoint(_vector.X * -1, _vector.Y);

                setRotationFromVector();
                return true;

            }

            if (Position.Y < _radius)
            {
                PositionY = _radius;
                _rotationOrientation = ROTATE_NONE;
                _vector = new CCPoint(_vector.X, _vector.Y * -1);
                setRotationFromVector();
                return true;
            }

            if (Position.Y > screenSize.Height - _radius)
            {
                PositionY = screenSize.Height - _radius;
                _rotationOrientation = ROTATE_NONE;
                _vector = new CCPoint(_vector.X, _vector.Y * -1);
                setRotationFromVector();
                return true;
            }

            return false;

        }

        public static Rocket Create()
        {
            var sprite = new Rocket("rocket.png");
            sprite._radius = sprite.BoundingBox.Size.Height * 0.5f;
            return sprite;
        }

    }

}