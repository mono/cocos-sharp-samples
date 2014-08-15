using System;
using Microsoft.Xna.Framework;
using System.Linq;
using CocosDenshion;
using CocosSharp;

namespace AirHockey.Common
{
	public class IntroLayer : CCLayerColor
	{

		GameSprite _player1;
		GameSprite _player2;
		GameSprite _ball;

		GameSprite[] _players;
		CCLabelTtf _player1ScoreLabel;
		CCLabelTtf _player2ScoreLabel;

		CCSize _screenSize;

		int _player1Score;
		int _player2Score;

		int kPlayer1Tag = 1;
		int kPlayer2Tag = 2;


		public IntroLayer(CCSize size)
			: base(size)
		{

		}

	
		protected override void AddedToScene()
		{
			base.AddedToScene();

			//Init Game Elements
			_player1Score = 0;
			_player2Score = 0;


			//get screen size
			_screenSize = Window.WindowSizeInPixels; //CCDirector::sharedDirector()->getWinSize();

			//1. add court image
			GameSprite court = new GameSprite("court"); // CCSprite::create("court.png");
			court.SetPosition(new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.5f)); //->setPosition(ccp());

			AddChild(court);

			//2. add players
			_player1 = new GameSprite("mallet"); //GameSprite::gameSpriteWithFile("mallet.png");
			_player1.SetPosition(new CCPoint(_screenSize.Width * 0.5f, _player1.RotationX * 2f));

			AddChild(_player1, 0, kPlayer1Tag);

			_player2 = new GameSprite("mallet");
			_player2.SetPosition(new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height - _player1.RotationX * 2));
			AddChild(_player2, 0, kPlayer2Tag);

			_players = new GameSprite[] { _player1, _player2 };// CCArray::create(_player1, _player2, NULL);

			//3. add puck
			_ball = new GameSprite("puck");
			_ball.SetPosition(new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.5f - 2f * _ball.RotationX));
			AddChild(_ball);

			//4. add score display
			_player1ScoreLabel = new CCLabelTtf("0", "MarkerFelt", 22);
			_player1ScoreLabel.Position = new CCPoint(_screenSize.Width - 60f, _screenSize.Height * 0.5f - 80f);
			_player1ScoreLabel.Rotation = 90;
			AddChild(_player1ScoreLabel);

			_player2ScoreLabel = new CCLabelTtf("0", "MarkerFelt", 22);
			_player2ScoreLabel.Position = new CCPoint(_screenSize.Width - 60f, _screenSize.Height * 0.5f + 80f);
			_player2ScoreLabel.Rotation = 90;
			AddChild(_player2ScoreLabel);

			//listen for touches
			CCEventListenerTouchAllAtOnce tListener = new CCEventListenerTouchAllAtOnce();
			tListener.OnTouchesBegan = TouchesBegan;
			tListener.OnTouchesEnded = TouchesEnded;
			tListener.OnTouchesMoved = TouchesMoved;
			AddEventListener(tListener, this);

			Schedule(Update);
		}



		void TouchesBegan(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{

			CCPoint tap;
			GameSprite player;

			foreach (CCTouch touch in touches)
			{

				if (touch != null)
				{
					//get location in OpenGL view
					tap = touch.LocationOnScreen; ; ; //??¿¿?¿?
					tap.Y = Window.WindowSizeInPixels.Height - tap.Y;
					//CCPoint.Rotate()
					//tap = CCPoint.RotateByAngle(touch.Location, new CCPoint(0,0)  , 90);// ;  (touch.Location, touch.Location, 90);
					//loop through players and check to see if touch is landing on one of them
					for (int p = 0; p < 2; p++)
					{
						player = (GameSprite)_players[p];
						// var eee = RotatePoint(tap,player.Position, player);


						if (player.BoundingBox.ContainsPoint(tap))
						{
							//store player's touch 
							player.Touch = touch;
						}
					}
				}

			}

		}

		void TouchesMoved(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{

			CCPoint tap;
			GameSprite player;

			foreach (CCTouch touch in touches)
			{

				if (touch != null)
				{
					tap = touch.LocationOnScreen;
					tap.Y = Window.WindowSizeInPixels.Height - tap.Y;

					for (int p = 0; p < _players.Length; p++)
					{

						player = (GameSprite)_players[p];
						//if touch belongs to player
						if (player.Touch != null && player.Touch == touch)
						{
							CCPoint nextPosition = tap;

							//keep player inside screen
							if (nextPosition.X < player.radius)
								nextPosition.X = player.radius;
							if (nextPosition.X > _screenSize.Width - player.radius)
								nextPosition.X = _screenSize.Width - player.radius;
							if (nextPosition.Y < player.radius)
								nextPosition.Y = player.radius;
							if (nextPosition.Y > _screenSize.Height - player.radius)
								nextPosition.Y = _screenSize.Height - player.radius;

							//keep player inside its court
							if (player.PositionY < _screenSize.Height * 0.5f)
							{
								if (nextPosition.Y > _screenSize.Height * 0.5f - player.radius)
								{
									nextPosition.Y = _screenSize.Height * 0.5f - player.radius;
								}
							}
							else
							{
								if (nextPosition.Y < _screenSize.Height * 0.5f + player.radius)
								{
									nextPosition.Y = _screenSize.Height * 0.5f + player.radius;
								}
							}

							player.NextPosition = nextPosition;
							player.Vector = new CCPoint(tap.X - player.PositionX, tap.Y - player.PositionY);
						}
					}
				}

			}

		}

		void TouchesEnded(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{

			GameSprite player;

			foreach (CCTouch touch in touches)
			{

				if (touch != null)
				{
					for (int p = 0; p < _players.Length; p++)
					{
						player = (GameSprite)_players[p];
						if (player.Touch != null && player.Touch == touch)
						{
							//if touch ending belongs to this player, clear it
							player.Touch = null;
							player.Vector = CCPoint.Zero;
						}
					}
				}

			}



		}

		public override void Update(float dt)
		{
			base.Update(dt);

			//update puck
			CCPoint ballNextPosition = _ball.NextPosition;
			CCPoint ballVector = _ball.Vector;

			ballVector = ballVector *  0.98f;

			ballNextPosition.X += ballVector.X;
			ballNextPosition.Y += ballVector.Y;

			//test for puck and mallet collision
			float squared_radii = (float)Math.Pow(_player1.radius + _ball.radius, 2);

			GameSprite player;
			CCPoint playerNextPosition;
			CCPoint playerVector;

			for (int p = 0; p < _players.Length; p++)
			{

				//if (p == 0)
				//    continue;

				player = _players[p];
				playerNextPosition = player.NextPosition;
				playerVector = player.Vector;

				float diffx = ballNextPosition.X - player.PositionX;
				float diffy = ballNextPosition.Y - player.PositionY;

				float distance1 = (float) ( Math.Pow(diffx, 2) + Math.Pow(diffy, 2));

				float distance2 = (float) (Math.Pow(_ball.PositionX - playerNextPosition.X, 2) +
					Math.Pow(_ball.PositionY - playerNextPosition.Y, 2));

				if (distance1 <= squared_radii || distance2 <= squared_radii)
				{

					float mag_ball = (float)(Math.Pow(ballVector.X, 2) + Math.Pow(ballVector.Y, 2));
					float mag_player = (float) (Math.Pow(playerVector.X, 2) + Math.Pow(playerVector.Y, 2));

					float force =(float) Math.Sqrt(mag_ball + mag_player);
					float angle = (float) Math.Atan2(diffy, diffx);

					ballVector.X = (float) (force * Math.Cos(angle));
					ballVector.Y = (float)(force * Math.Sin(angle));

					ballNextPosition.X = (float)( playerNextPosition.X + (player.radius + _ball.radius + force) *  Math.Cos(angle));
					ballNextPosition.Y = (float) ( playerNextPosition.Y + (player.radius + _ball.radius + force) *  Math.Sin(angle));

					PlayHit(); //CCSimpleAudioEngine.SharedEngine.PlayEffect("hit");
				}
			}

			////check collision of ball and sides
			if (ballNextPosition.X < _ball.radius)
			{
				ballNextPosition.X = _ball.radius;
				ballVector.X *= -0.8f;
				PlayHit(); //CCSimpleAudioEngine.SharedEngine.PlayEffect("hit");
			}

			if (ballNextPosition.X > _screenSize.Width - _ball.radius)
			{
				ballNextPosition.X = _screenSize.Width - _ball.radius;
				ballVector.X *= -0.8f;

				PlayHit();
				//CCSimpleAudioEngine.SharedEngine.PlayEffect("hit");
			}

			//ball and top of the court
			if (ballNextPosition.Y > _screenSize.Height - _ball.radius)
			{
				if (_ball.PositionX < _screenSize.Width * 0.5f - GOAL_WIDTH * 0.5f ||
					_ball.PositionX > _screenSize.Width * 0.5f + GOAL_WIDTH * 0.5f)
				{
					ballNextPosition.Y = _screenSize.Height - _ball.radius;
					ballVector.Y *= -0.8f;
					PlayHit();
					//CCSimpleAudioEngine.SharedEngine.PlayEffect("hit");
				}
			}
			//ball and bottom of the court
			if (ballNextPosition.Y < _ball.radius)
			{
				if (_ball.PositionX < _screenSize.Width * 0.5f - GOAL_WIDTH * 0.5f ||
					_ball.PositionX > _screenSize.Width * 0.5f + GOAL_WIDTH * 0.5f)
				{
					ballNextPosition.Y = _ball.radius;
					ballVector.Y *= -0.8f;
					PlayHit();
				}
			}

			////finally, after all checks, update ball's vector and next position
			_ball.Vector = ballVector;
			_ball.NextPosition = ballNextPosition;


			////check for goals!
			if (ballNextPosition.Y < -_ball.radius * 2)
				playerScore(2);

			//}
			if (ballNextPosition.Y > _screenSize.Height + _ball.radius * 2)
			{
				playerScore(1);
			}

			////move pieces to next position
			_player1.Position = _player1.NextPosition; // ->setPosition(_player1->getNextPosition());
			_player2.Position = _player2.NextPosition; //->setPosition(_player2->getNextPosition());
			_ball.Position = _ball.NextPosition; //->setPosition(_ball->getNextPosition());    

		}


		void PlayHit()
		{
			CCSimpleAudioEngine.SharedEngine.PlayEffect("sounds/hit");
		}

		void playerScore(int player)
		{

			CCSimpleAudioEngine.SharedEngine.PlayEffect("sounds/score");

			_ball.Vector = CCPoint.Zero;

			//if player 1 scored...
			if (player == 1)
			{

				_player1Score++;
				_player1ScoreLabel.Text = String.Format("{0}", _player1Score);
				_ball.NextPosition = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.5f + 2f * _ball.radius);
			}
			else
			{

				_player2Score++;
				_player2ScoreLabel.Text = String.Format("{0}", _player2Score);
				_ball.NextPosition = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.5f - 2f * _ball.radius);

			}
			//move players to original position
			_player1.Position = new CCPoint(_screenSize.Width * 0.5f, _player1.radius * 2);
			_player2.Position = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height - _player1.radius * 2f);

			//clear current touches
			_player1.Touch = null;
			_player2.Touch = null;
		}



		public float GOAL_WIDTH = 400;

	}
}

