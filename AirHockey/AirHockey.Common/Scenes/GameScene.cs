using System;
using System.Collections.Generic;
using CocosSharp;

namespace AirHockey.Common
{
    public class GameScene : CCScene
    {
        CCLayer mainLayer;

        GameSprite ball;
        GameSprite[] players;

        CCLabel player1ScoreLabel;
        CCLabel player2ScoreLabel;

        CCRect courtBounds;

        int player1Score;
        int player2Score;

        int kPlayer1Tag = 1;
        int kPlayer2Tag = 2;

        const float goalWidth = 400f;


        #region Initialisation

        public static void LoadGame (object sender, EventArgs e)
        {
            CCGameView gameView = sender as CCGameView;

            if (gameView != null) {
                var contentSearchPaths = new List<string> () { "Fonts", "Images", "Sounds" };
                CCSizeI viewSize = gameView.ViewSize;

                int width = 1024;
                int height = 768;

                CCSprite.DefaultTexelToContentSizeRatio = 2.0f;

                // Set world dimensions
                gameView.DesignResolution = new CCSizeI (width, height);
                gameView.ContentManager.SearchPaths = contentSearchPaths;

                CCScene gameScene = new GameScene (gameView);
                gameView.RunWithScene (gameScene);
            }
        }

        public GameScene (CCGameView gameView) : base(gameView)
        {
            CreateLayers ();

            SetupInput ();

            LoadScene ();

            Schedule (Update);
        }

        void CreateLayers ()
        {
            mainLayer = new CCLayerColor (CCColor4B.Black);
            this.AddLayer (mainLayer);
        }

        void SetupInput ()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesBegan = OnTouchesBegan;
            touchListener.OnTouchesMoved = OnTouchesMoved;
            touchListener.OnTouchesEnded = OnTouchesEnded;

            mainLayer.AddEventListener(touchListener, this);

            Schedule(Update);
        }

        void LoadScene ()
        {
            var boundsSize = mainLayer.VisibleBoundsWorldspace.Size;

            var court = new CCSprite("court");
            court.Position = new CCPoint(boundsSize.Width * 0.5f, boundsSize.Height * 0.5f);
            mainLayer.AddChild(court);

            courtBounds = court.BoundingBoxTransformedToWorld;

            var player1 = new GameSprite("mallet");
            player1.Position = new CCPoint(boundsSize.Width * 0.5f, player1.Radius * 2f);
            mainLayer.AddChild(player1, 0, kPlayer1Tag);

            var player2 = new GameSprite("mallet");
            player2.Position = new CCPoint(boundsSize.Width * 0.5f, boundsSize.Height - player1.Radius * 2f);
            mainLayer.AddChild(player2, 0, kPlayer2Tag);

            players = new GameSprite[] { player1, player2 };

            ball = new GameSprite("puck");
            ball.Position = new CCPoint(boundsSize.Width * 0.5f, boundsSize.Height * 0.5f - 2f * ball.Radius);
            mainLayer.AddChild(ball);

            player1ScoreLabel = new CCLabel("0", "MarkerFelt", 22);
            player1ScoreLabel.Position = new CCPoint(boundsSize.Width - 60f, boundsSize.Height * 0.5f - 80f);
            player1ScoreLabel.Rotation = 90;
            mainLayer.AddChild(player1ScoreLabel);

            player2ScoreLabel = new CCLabel("0", "MarkerFelt", 22);
            player2ScoreLabel.Position = new CCPoint(boundsSize.Width - 60f, boundsSize.Height * 0.5f + 80f);
            player2ScoreLabel.Rotation = 90;
            mainLayer.AddChild(player2ScoreLabel);
        }

        #endregion Initialisation


        #region Touch handling

        void OnTouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {
            CCPoint tap;

            foreach (CCTouch touch in touches)
            {
                if (touch != null)
                {
                    tap = touch.LocationOnScreen; 
                    tap = mainLayer.ScreenToWorldspace (tap);

                    foreach (GameSprite player in players)
                    {
                        if (player.BoundingBox.ContainsPoint(tap))
                            player.Touch = touch;
                    }
                }
            }

        }

        void OnTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {
            CCPoint tap;

            foreach (CCTouch touch in touches)
            {
                if (touch != null)
                {
                    tap = touch.LocationOnScreen;
                    tap = mainLayer.ScreenToWorldspace (tap);

                    foreach (GameSprite player in players)
                    {
                        if (player.Touch != null && player.Touch == touch)
                        {
                            CCPoint nextPosition = tap;

                            nextPosition.X = Math.Max (nextPosition.X, courtBounds.Origin.X + player.Radius);
                            nextPosition.X = Math.Min (nextPosition.X, courtBounds.Origin.X + courtBounds.Size.Width - player.Radius);

                            nextPosition.Y = Math.Max (nextPosition.Y, courtBounds.Origin.Y + player.Radius);
                            nextPosition.Y = Math.Min (nextPosition.Y, courtBounds.Origin.Y + courtBounds.Size.Height - player.Radius);

                            float halfHeight = courtBounds.Size.Height / 2.0f;

                            if (player.PositionY < courtBounds.Origin.Y + halfHeight)
                                nextPosition.Y = Math.Min (nextPosition.Y, courtBounds.Origin.Y + halfHeight - player.Radius);
                            else
                                nextPosition.Y = Math.Max (nextPosition.Y, courtBounds.Origin.Y + halfHeight + player.Radius);

                            player.NextPosition = nextPosition;
                            player.Vector = new CCPoint(tap.X - player.PositionX, tap.Y - player.PositionY);
                        }
                    }
                }
            }
        }

        void OnTouchesEnded(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            foreach (CCTouch touch in touches)
            {
                if (touch != null)
                {
                    foreach (GameSprite player in players)
                    {
                        if (player.Touch != null && player.Touch == touch)
                        {
                            player.Touch = null;
                            player.Vector = CCPoint.Zero;
                        }
                    }
                }
            }
        }

        #endregion Touch handling


        #region Run-loop

        public override void Update(float dt)
        {
            base.Update(dt);

            CCPoint ballNextPosition = ball.NextPosition;
            CCPoint ballVector = ball.Vector;

            ballVector = ballVector *  0.98f;

            ballNextPosition.X += ballVector.X;
            ballNextPosition.Y += ballVector.Y;

            float squared_radii = (float)Math.Pow(players[0].Radius + ball.Radius, 2);
            float halfWidth = courtBounds.Size.Width / 2.0f;


            foreach (GameSprite player in players)
            {
                CCPoint playerNextPosition = player.NextPosition;
                CCPoint playerVector = player.Vector;

                float diffx = ballNextPosition.X - player.PositionX;
                float diffy = ballNextPosition.Y - player.PositionY;

                float distance1 = (float) ( Math.Pow(diffx, 2) + Math.Pow(diffy, 2));

                float distance2 = (float) (Math.Pow(ball.PositionX - playerNextPosition.X, 2) +
                    Math.Pow(ball.PositionY - playerNextPosition.Y, 2));

                if (distance1 <= squared_radii || distance2 <= squared_radii)
                {
                    float mag_ball = (float)(Math.Pow(ballVector.X, 2) + Math.Pow(ballVector.Y, 2));
                    float mag_player = (float) (Math.Pow(playerVector.X, 2) + Math.Pow(playerVector.Y, 2));

                    float force =(float) Math.Sqrt(mag_ball + mag_player);
                    float angle = (float) Math.Atan2(diffy, diffx);

                    ballVector.X = (float) (force * Math.Cos(angle));
                    ballVector.Y = (float)(force * Math.Sin(angle));

                    ballNextPosition.X = (float)( playerNextPosition.X + (player.Radius + ball.Radius + force) *  Math.Cos(angle));
                    ballNextPosition.Y = (float) ( playerNextPosition.Y + (player.Radius + ball.Radius + force) *  Math.Sin(angle));

                    PlayHit();
                }
            }

            if (ballNextPosition.X < courtBounds.Origin.X + ball.Radius)
            {
                ballNextPosition.X = courtBounds.Origin.X + ball.Radius;
                ballVector.X *= -0.8f;
                PlayHit ();
            }

            else if (ballNextPosition.X > courtBounds.Origin.X + courtBounds.Size.Width - ball.Radius)
            {
                ballNextPosition.X = courtBounds.Origin.X + courtBounds.Size.Width - ball.Radius;
                ballVector.X *= -0.8f;

                PlayHit ();
            }

            if (ballNextPosition.Y < courtBounds.Origin.Y + ball.Radius)
            {
                if (ball.PositionX < courtBounds.Origin.X + halfWidth - goalWidth * 0.5f ||
                    ball.PositionX > courtBounds.Origin.X + halfWidth + goalWidth * 0.5f)
                {
                    ballNextPosition.Y = courtBounds.Origin.Y + ball.Radius;
                    ballVector.Y *= -0.8f;
                    PlayHit();
                }
            }

            else if (ballNextPosition.Y > courtBounds.Origin.Y + courtBounds.Size.Height - ball.Radius)
            {
                if (ball.PositionX < courtBounds.Origin.X + halfWidth - goalWidth * 0.5f ||
                    ball.PositionX > courtBounds.Origin.X + halfWidth + goalWidth * 0.5f)
                {
                    ballNextPosition.Y = courtBounds.Origin.Y + courtBounds.Size.Height - ball.Radius;
                    ballVector.Y *= -0.8f;
                    PlayHit ();
                }
            }

            ball.Vector = ballVector;
            ball.NextPosition = ballNextPosition;

            if (ballNextPosition.Y < courtBounds.Origin.Y - 2 * ball.Radius)
                PlayerScore(2);

            if (ballNextPosition.Y > courtBounds.Origin.Y + courtBounds.Size.Height + 2 * ball.Radius)
                PlayerScore(1);

            // Move pieces to next position
            players[0].Position = players[0].NextPosition;
            players[1].Position = players[1].NextPosition;
            ball.Position = ball.NextPosition;  
        }


        void PlayHit()
        {
            GameView.AudioEngine.PlayEffect("Sounds/hit");
        }

        void PlayerScore(int player)
        {
            GameView.AudioEngine.PlayEffect("Sounds/score");

            ball.Vector = CCPoint.Zero;

            float halfWidth = courtBounds.Size.Width / 2.0f;
            float halfHeight = courtBounds.Size.Height / 2.0f;

            if (player == 1)
            {
                player1Score++;
                player1ScoreLabel.Text = String.Format("{0}", player1Score);
                ball.NextPosition = new CCPoint(courtBounds.Origin.X + halfWidth, 
                    courtBounds.Origin.Y + halfHeight + 2f * ball.Radius);
            }
            else
            {
                player2Score++;
                player2ScoreLabel.Text = String.Format("{0}", player2Score);
                ball.NextPosition = new CCPoint(courtBounds.Origin.X + halfWidth, 
                    courtBounds.Origin.Y + halfHeight - 2f * ball.Radius);

            }

            players[0].Position = new CCPoint(courtBounds.Origin.X + halfWidth, 2 * players[0].Radius);
            players[1].Position = new CCPoint(courtBounds.Origin.X + halfWidth, 
                courtBounds.Origin.Y + courtBounds.Size.Height - 2 * players[0].Radius);

            players[0].Touch = null;
            players[1].Touch = null;
        }

        #endregion Run-loop
    }
}

