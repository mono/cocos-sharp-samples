using System;

using Microsoft.Xna.Framework;
using CocosSharp;

namespace RocketThrought.Common
{
	public enum lineTypes
	{
		LINE_TEMP = 1,
		LINE_DASHED = 2,
		LINE_NONE = 3
	}

	public class LineContainer : CCNode
	{

		public float _energy { get; set; }
		public CCPoint _pivot { get; set; }
		public CCPoint _tip { get; set; }
		public float _lineLength { get; set; }
		public lineTypes _lineType { get; set; }

		public float _lineAngle = 0;
		public float _energyLineX = 0;
		public float _energyHeight = 0;
		public float _energyDecrement = 0;

		public int _dash;
		public int _dashSpace;
		public CCSize _screenSize;

        CCDrawNode drawNode;

		public LineContainer()
		{
            drawNode = new CCDrawNode();
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();
			_screenSize = Window.WindowSizeInPixels;

			_dash = 10;
			_dashSpace = 10;
			_lineType = lineTypes.LINE_NONE;

			_energyLineX = _screenSize.Width * 0.96f;
			_energyHeight = _screenSize.Height * 0.8f;
			//glLineWidth(8.0 * CC_CONTENT_SCALE_FACTOR());
			reset();
		}

		public void reset()
		{
			_energy = 1.0f;
			_energyDecrement = 0.015f;
		}

		public override void Update(float dt)
		{
			base.Update(dt);
			_energy -= dt * _energyDecrement;
			if (_energy < 0) _energy = 0;
            Draw ();
		}

		public void setEnergyDecrement(float value)
		{
			_energyDecrement += value;
			//if (_energyDecrement > 0.07) _energyDecrement = 0.07;
		}

        protected void Draw()
        {
            CCColor4B color = new CCColor4B(1.0f, 1.0f, 1.0f, 1.0f);
            

            switch (_lineType)
            {
                case lineTypes.LINE_NONE:
                    break;
                case lineTypes.LINE_TEMP:
                    drawNode.DrawLine(_tip, _pivot, color);
                    drawNode.DrawCircle(_pivot, 10, 10, color);
                    break;

                case lineTypes.LINE_DASHED:
                    drawNode.DrawCircle(_pivot, 10, 10, color);
                    int segments = (int)(_lineLength / (_dash + _dashSpace));
                    float t = 0.0f;
                    float x_;
                    float y_;

                    for (int i = 0; i < segments + 1; i++)
                    {
                        x_ = _pivot.X + t * (_tip.X - _pivot.X);
                        y_ = _pivot.Y + t * (_tip.Y - _pivot.Y);
                        drawNode.DrawCircle(new CCPoint(x_, y_), 4, 6, color);
                        t += (float)1 / segments;
                    }
                    break;
            }

            //draw energy bar
            color = new CCColor4B(0.0f, 0.0f, 0.0f, 1.0f);

            drawNode.DrawLine(
                new CCPoint(_energyLineX, _screenSize.Height * 0.1f),
                new CCPoint(_energyLineX, _screenSize.Height * 0.9f),
                color);
            color = new CCColor4B(1.0f, 0.5f, 0.0f, 1.0f);
            drawNode.DrawLine(
                new CCPoint(_energyLineX, _screenSize.Height * 0.1f),
                new CCPoint(_energyLineX, _screenSize.Height * 0.1f + _energy * _energyHeight),
                color);
        }

		public static LineContainer Create()
		{
			return new LineContainer();
		}
	}
}