using System;

using Microsoft.Xna.Framework;
using System.Collections.Generic;

using CocosDenshion;
using System.Linq;
using CocosSharp;

namespace RocketThrought.Common
{

    enum gamestates
    {
        kGameIntro = 1,
        kGamePaused = 2,
        kGamePlay = 3,
        kGameOver = 4
    };

    public class IntroLayer : CCLayerColor
    {

        //public int ROTATE_CLOCKWISE = 1;

        //public int ROTATE_COUNTER = 1;

        float RAND_MAX = 32767;

        int kBackground = 1;
        int kMiddleground = 2;
        int kForeground = 3;

        int kSpriteRocket = 1;
        int kSpritePlanet = 2;
        int kSpriteBoost = 3;
        int kSpriteStar = 4;

        Rocket _rocket;
        LineContainer _lineContainer;

        CCSpriteBatchNode _gameBatchNode;
        CCLabelTtf _scoreDisplay;

        GameSprite _pauseBtn;
        CCSprite _intro;
        CCSprite _gameOver;
        CCSprite _paused;

        CCParticleSystem _star;
        CCParticleSystem _jet;
        CCParticleSystem _boom;
        CCParticleSystem _comet;
        CCParticleSystem _pickup;
        CCParticleSystem _warp;

        List<GameSprite> _planets;
        CCSize _screenSize;

        gamestates _state;

        bool _drawing;
        bool _running;

        List<CCPoint> _grid;
        int _gridIndex;

        int _minLineLength;
        float _cometInterval;
        float _cometTimer;


        int _score;
        float _timeBetweenPickups;

        // Splash loading
        int numOfParticleSystems = 6;
        int loadedParticleSystems = 0;
        bool isLoading = false;

        public IntroLayer()
        {

            _grid = new List<CCPoint>();

            //init game values
            _screenSize = CCDirector.SharedDirector.WinSize;
            _drawing = false;
            _minLineLength = (int)(_screenSize.Width * 0.07f);
            _state = gamestates.kGameIntro;

            createGameScreen();

#if WINDOWS_PHONE
            createParticlesAsync();
#else
            createParticles();
#endif

            createStarGrid();

            //listen for touches
            CCEventListenerTouchAllAtOnce lTouch = new CCEventListenerTouchAllAtOnce();
            lTouch.OnTouchesBegan = TouchesBegan;
            lTouch.OnTouchesEnded = TouchesEnded;
            lTouch.OnTouchesMoved = TouchesMoved;

            EventDispatcher.AddEventListener(lTouch, this);


            //create main loop
            //this->schedule(schedule_selector(GameLayer::update));
            Schedule(Update);
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (!_running) return;

            if (_lineContainer._lineType != lineTypes.LINE_NONE)
            {
                _lineContainer._tip = _rocket.Position;
            }

            //track collision with sides
            if (_rocket.collidedWithSides())
            {
                _lineContainer._lineType = lineTypes.LINE_NONE;
            }

            _rocket.Update(dt);

            //update jet particle so it follow rocket
            if (!_jet.IsActive)
                _jet.ResetSystem();

            _jet.RotationX = _rocket.RotationX;
            _jet.RotationY = _rocket.RotationY;


            _jet.Position = _rocket.Position;
            //_jet->setRotation(_rocket->getRotation());
            //_jet->setPosition(_rocket->getPosition());


            //update timers
            _cometTimer += dt;
            float newY;
            if (_cometTimer > _cometInterval)
            {
                _cometTimer = 0;
                if (_comet.Visible == false)
                {
                    _comet.PositionX = 0;

                    newY = CCRandom.Float_0_1() / (RAND_MAX / _screenSize.Height * 0.6f) + _screenSize.Height * 0.2f;

                    if (newY > _screenSize.Height * 0.9f)
                        newY = _screenSize.Height * 0.9f;

                    _comet.PositionY = newY;
                    _comet.Visible = true;
                    _comet.ResetSystem();
                }
            }

            if (_comet.Visible)
            {

                //collision with comet
                if ((float)Math.Pow(_comet.PositionX - _rocket.PositionX, 2) +
                    (float)Math.Pow(_comet.PositionY - _rocket.PositionY, 2) <= (float)Math.Pow(_rocket._radius, 2))
                {
                    if (_rocket.Visible)
                        killPlayer();
                }
                _comet.PositionX = _comet.PositionX + 50 * dt;
                if (_comet.PositionX > _screenSize.Width * 1.5f)
                {
                    _comet.StopSystem();
                    _comet.Visible = false;
                }
            }

            _lineContainer.Update(dt);
            _rocket.Opacity = (byte)(_lineContainer._energy * 255);

            //collision with planets
            int count = _planets.Count;
            GameSprite planet;
            for (int i = 0; i < count; i++)
            {
                planet = _planets[i];

                if ((float)Math.Pow(planet.PositionX - _rocket.PositionX, 2)
                    + (float)Math.Pow(planet.PositionY - _rocket.PositionY, 2) <=
                    (float)Math.Pow(_rocket._radius * 0.8f + planet._radius * 0.65f, 2))
                {

                    if (_rocket.Visible)
                        killPlayer();

                    break;
                }
            }

            //collision with star
            if ((float)Math.Pow(_star.PositionX - _rocket.PositionX, 2)
                + (float)Math.Pow(_star.PositionY - _rocket.PositionY, 2) <=
                (float)Math.Pow(_rocket._radius * 1.2f, 2))
            {

                _pickup.Position = _star.Position;
                _pickup.ResetSystem();

                if (_lineContainer._energy + 0.25f < 1)
                {
                    _lineContainer._energy = _lineContainer._energy + 0.25f;
                }
                else
                {
                    _lineContainer._energy = 1.0f;
                }
                _rocket._speed = _rocket._speed + 2;
                _lineContainer.setEnergyDecrement(0.001f);

                CCSimpleAudioEngine.SharedEngine.PlayEffect("pickup");
                resetStar();

                int points = 100 - (int)_timeBetweenPickups;
                if (points < 0) points = 0;

                _score += points;

                _scoreDisplay.Text = String.Format("{0}", _score);

                _timeBetweenPickups = 0;

            }

            _timeBetweenPickups += dt;
            if (_lineContainer._energy == 0)
            {
                if (_rocket.Visible)
                    killPlayer();
            }

        }

        private void resetStar()
        {
            CCPoint position = _grid[_gridIndex];
            _gridIndex++;
            if (_gridIndex == _grid.Count()) _gridIndex = 0;
            _star.Position = position;
            _star.Visible = true;
            _star.ResetSystem(); // ->resetSystem();
        }

        public void createGameScreen()
        {

            CCSprite bg = new CCSprite("bg.png");
            bg.Position = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.5f);
            AddChild(bg, kBackground);

            _lineContainer = LineContainer.Create();
            AddChild(_lineContainer);

            CCSpriteFrameCache.Instance.AddSpriteFrames("sprite_sheet.plist");
            _gameBatchNode = new CCSpriteBatchNode("sprite_sheet.png", 100);

            AddChild(_gameBatchNode, kForeground);

            _rocket = Rocket.Create();
            _rocket.Position = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.1f);
            _gameBatchNode.AddChild(_rocket, kForeground, kSpriteRocket);

            //add planets
            GameSprite planet;
            _planets = new List<GameSprite>();
            // _planets->retain();

            planet = new GameSprite("planet_1.png");
            planet.Position = new CCPoint(_screenSize.Width * 0.25f,
                                    _screenSize.Height * 0.8f);

            _gameBatchNode.AddChild(planet, kBackground, kSpritePlanet);
            _planets.Add(planet);

            planet = new GameSprite("planet_2.png");
            planet.Position = new CCPoint(_screenSize.Width * 0.8f,
                                    _screenSize.Height * 0.45f);
            _gameBatchNode.AddChild(planet, kBackground, kSpritePlanet);
            _planets.Add(planet);

            planet = new GameSprite("planet_3.png");
            planet.Position = new CCPoint(_screenSize.Width * 0.75f,
                                    _screenSize.Height * 0.8f);
            _gameBatchNode.AddChild(planet, kBackground, kSpritePlanet);
            _planets.Add(planet);

            planet = new GameSprite("planet_4.png");
            planet.Position = new CCPoint(_screenSize.Width * 0.5f,
                                    _screenSize.Height * 0.5f);
            _gameBatchNode.AddChild(planet, kBackground, kSpritePlanet);
            _planets.Add(planet);

            planet = new GameSprite("planet_5.png");
            planet.Position = new CCPoint(_screenSize.Width * 0.18f,
                                    _screenSize.Height * 0.45f);
            _gameBatchNode.AddChild(planet, kBackground, kSpritePlanet);
            _planets.Add(planet);

            planet = new GameSprite("planet_6.png");
            planet.Position = new CCPoint(_screenSize.Width * 0.8f,
                                    _screenSize.Height * 0.15f);
            _gameBatchNode.AddChild(planet, kBackground, kSpritePlanet);
            _planets.Add(planet);

            planet = new GameSprite("planet_7.png");
            planet.Position = new CCPoint(_screenSize.Width * 0.18f,
                                    _screenSize.Height * 0.1f);
            _gameBatchNode.AddChild(planet, kBackground, kSpritePlanet);
            _planets.Add(planet);

            CCSprite scoreLabel = new GameSprite("label_score.png");
            scoreLabel.Position = new CCPoint(_screenSize.Width * 0.4f, _screenSize.Height * 0.95f);
            _gameBatchNode.AddChild(scoreLabel, kBackground);

            _scoreDisplay = new CCLabelTtf("0", "MarkerFelt", 22);

            _scoreDisplay.AnchorPoint = new CCPoint(0, 0.5f);
            _scoreDisplay.Position = new CCPoint(_screenSize.Width * 0.48f, _screenSize.Height * 0.95f);
            AddChild(_scoreDisplay, kBackground);

            _pauseBtn = new GameSprite("btn_pause_off.png");
            _pauseBtn.Visible = false;
            _pauseBtn.Position = new CCPoint(_screenSize.Width * 0.06f, _screenSize.Height * 0.95f);
            _gameBatchNode.AddChild(_pauseBtn, kBackground);


            _intro = new GameSprite("logo.png");
            _intro.Position = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.55f);
            CCSprite play = new CCSprite("label_play.png");
            play.Position = new CCPoint(_intro.BoundingBox.Size.Width * 0.5f, -_intro.BoundingBox.Size.Height * 0.5f);
            _intro.AddChild(play);
            _gameBatchNode.AddChild(_intro, kForeground);

            _gameOver = new GameSprite("gameOver.png");
            _gameOver.Position = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.55f);
            _gameOver.Visible = false;
            _gameBatchNode.AddChild(_gameOver, kForeground);

            _paused = new GameSprite("label_paused.png");
            _paused.Position = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.55f);
            _paused.Visible = false;
            _gameBatchNode.AddChild(_paused, kForeground);

        }

        public void createParticlesAsync()
        {

            isLoading = true;

            CCSize size = CCDirector.SharedDirector.WinSize;

            var label = new CCLabelTtf("Loading...", "MarkerFelt", 22);
            label.Position = size.Center;
            label.Position = new CCPoint(size.Center.X, size.Height * 0.75f);

            label.Visible = false;
            label.Name = "Loading";
            AddChild(label, 10);

            var scale = new CCScaleBy(0.3f, 2);
            label.RunActions(new CCDelayTime(1.0f), new CCShow());
            label.RepeatForever(scale, scale.Reverse());

            CCParticleSystemCache.SharedParticleSystemCache.AddParticleSystemAsync("jet.plist",

                (jetConfig) =>
                {
                    _jet = new CCParticleSystemQuad(jetConfig);
                    _jet.SourcePosition = new CCPoint(-_rocket._radius * 0.8f, 0);
                    _jet.Angle = 180;
                    _jet.StopSystem();
                    AddChild(_jet, kBackground);

                    loadedParticleSystems++;
                    updateLoading();
                });

            CCParticleSystemCache.SharedParticleSystemCache.AddParticleSystemAsync("boom.plist",

                (boomConfig) =>
                {
                    _boom = new CCParticleSystemQuad(boomConfig);
                    _boom.StopSystem();
                    AddChild(_boom, kForeground);
                    loadedParticleSystems++;
                    updateLoading();
                });

            CCParticleSystemCache.SharedParticleSystemCache.AddParticleSystemAsync("comet.plist",

                (cometConfig) =>
                {
                    _comet = new CCParticleSystemQuad(cometConfig);
                    _comet.StopSystem();
                    _comet.Position = new CCPoint(0, _screenSize.Height * 0.6f);
                    _comet.Visible = false;
                    AddChild(_comet, kForeground);
                    loadedParticleSystems++;
                    updateLoading();
                });


            CCParticleSystemCache.SharedParticleSystemCache.AddParticleSystemAsync("star.plist",

               (starConfig) =>
               {
                   _star = new CCParticleSystemQuad(starConfig);
                   _star.StopSystem();
                   _star.Visible = false;
                   AddChild(_star, kBackground, kSpriteStar);
                   loadedParticleSystems++;
                   updateLoading();
               });

            CCParticleSystemCache.SharedParticleSystemCache.AddParticleSystemAsync("plink.plist",

                (plinkConfig) =>
                {
                    _pickup = new CCParticleSystemQuad(plinkConfig);
                    _pickup.StopSystem();
                    AddChild(_pickup, kMiddleground);
                    loadedParticleSystems++;
                    updateLoading();
                });


            CCParticleSystemCache.SharedParticleSystemCache.AddParticleSystemAsync("warp.plist",

                (warpConfig) =>
                {
                    _warp = new CCParticleSystemQuad(warpConfig);
                    _warp.Position = _rocket.Position;
                    AddChild(_warp, kBackground);
                    loadedParticleSystems++;
                    updateLoading();
                });


        }

        void updateLoading()
        {
            if (loadedParticleSystems == numOfParticleSystems)
            {
                foreach (var c in Children)
                {
                    if (c.Name == "Loading")
                    {
                        c.RemoveFromParent();

                    }
                }
                isLoading = false;
            }
        }

        public void createParticles()
        {

            _jet = new CCParticleSystemQuad("jet.plist");
            _jet.SourcePosition = new CCPoint(-_rocket._radius * 0.8f, 0);
            _jet.Angle = 180;
            _jet.StopSystem();
            AddChild(_jet, kBackground);

            _boom = new CCParticleSystemQuad("boom.plist");
            _boom.StopSystem();
            AddChild(_boom, kForeground);

            _comet = new CCParticleSystemQuad("comet.plist");
            _comet.StopSystem();
            _comet.Position = new CCPoint(0, _screenSize.Height * 0.6f);
            _comet.Visible = false;
            AddChild(_comet, kForeground);

            _pickup = new CCParticleSystemQuad("plink.plist");
            _pickup.StopSystem();
            AddChild(_pickup, kMiddleground);

            _warp = new CCParticleSystemQuad("warp.plist");
            _warp.Position = _rocket.Position;
            AddChild(_warp, kBackground);

            _star = new CCParticleSystemQuad("star.plist");
            _star.StopSystem();
            _star.Visible = false;
            AddChild(_star, kBackground, kSpriteStar);

        }

        public void createStarGrid()
        {

            //create grid
            float gridFrame = _screenSize.Width * 0.1f;
            int tile = 32;
            int rows = (int)((_screenSize.Height - 4 * gridFrame) / tile);
            int cols = (int)((_screenSize.Width - 2 * gridFrame) / tile);

            int count = _planets.Count;
            GameSprite planet;
            CCPoint cell;
            bool overlaps;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    cell = new CCPoint(gridFrame + c * tile, 2 * gridFrame + r * tile);
                    overlaps = false;
                    for (int j = 0; j < count; j++)
                    {
                        planet = _planets[j];
                        if ((float)Math.Pow(planet.PositionX - cell.X, 2) + (float)Math.Pow(planet.PositionY - cell.Y, 2) <= (float)Math.Pow(planet._radius * 1.2f, 2))
                        {
                            overlaps = true;
                        }
                    }

                    if (!overlaps)
                        _grid.Add(cell);
                    //TODO: _grid.push_back(cell);
                }
            }
            //Console.WriteLine("POSSIBLE STARS: %i", _grid.Count);

        }

        protected void TouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {

            if (!_running) return;

            CCTouch touch = touches.FirstOrDefault();

            if (touch != null)
            {

                var tap = touch.Location;

                //track if tapping on ship
                float dx = _rocket.PositionX - tap.X;
                float dy = _rocket.PositionY - tap.Y;

                if (dx * dx + dy * dy <= (float)Math.Pow(_rocket._radius, 2))
                {
                    _lineContainer._lineType = lineTypes.LINE_NONE;
                    _rocket._rotationOrientation = Rocket.ROTATE_NONE; // ->setRotationOrientation(ROTATE_NONE);
                    _drawing = true;
                }
            }
        }

        protected void TouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
        {

            if (!_running) return;

            if (_drawing)
            {

                CCTouch touch = (CCTouch)touches.FirstOrDefault();

                if (touch != null)
                {

                    CCPoint tap = touch.Location;

                    float dx = _rocket.PositionX - tap.X;
                    float dy = _rocket.PositionY - tap.Y;

                    if (dx * dx + dy * dy > (float)Math.Pow(_minLineLength, 2))
                    {
                        _rocket.select(true);
                        _lineContainer._pivot = tap;
                        _lineContainer._lineType = lineTypes.LINE_TEMP;
                    }
                    else
                    {
                        _rocket.select(false);
                        _lineContainer._lineType = lineTypes.LINE_NONE;
                    }

                }

            }

        }

        protected void TouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {

            if (_state == gamestates.kGameIntro && !isLoading)
            {

                _intro.Visible = false;
                _pauseBtn.Visible = true;
                _state = gamestates.kGamePlay;
                resetGame();
                return;

            }
            else if (_state == gamestates.kGamePaused)
            {

                _pauseBtn.DisplayFrame = CCSpriteFrameCache.Instance["btn_pause_off.png"];
                _paused.Visible = false;
                _state = gamestates.kGamePlay;
                _running = true;
                return;

            }
            else if (_state == gamestates.kGameOver)
            {

                _gameOver.Visible = false;
                _pauseBtn.Visible = true;
                _state = gamestates.kGamePlay;
                resetGame();
                return;

            }

            if (!_running) return;

            CCTouch touch = touches.FirstOrDefault();

            if (touch != null)
            {

                CCPoint tap = touch.Location;

                if (_pauseBtn.BoundingBox.ContainsPoint(tap))
                {
                    _paused.Visible = true;
                    _state = gamestates.kGamePaused;
                    _pauseBtn.DisplayFrame = CCSpriteFrameCache.Instance["btn_pause_on.png"]; // CCSpriteFrameCache::sharedSpriteFrameCache()->spriteFrameByName ("btn_pause_on.png"));
                    _running = false;
                    return;
                }

                //track if tapping on ship
                _drawing = false;
                _rocket.select(false);

                //if we are showing a temp line
                if (_lineContainer._lineType == lineTypes.LINE_TEMP)
                {
                    //set up dashed line
                    _lineContainer._pivot = tap;
                    _lineContainer._lineLength = CCPoint.Distance(_rocket.Position, tap);

                    //set up rocket
                    _rocket._pivot = tap;
                    float circle_length = _lineContainer._lineLength * 2 * CCMathHelper.Pi;

                    int iterations = (int)Math.Floor(circle_length / _rocket._speed);
                    _rocket._angularSpeed = 2 * CCMathHelper.Pi / iterations;

                    CCPoint clockwise = CCPoint.Perp(_rocket.Position - _rocket._pivot);

                    float dot = CCPoint.Dot(clockwise, _rocket._vector);

                    if (dot > 0)
                    {
                        _rocket._angularSpeed = (_rocket._angularSpeed * -1);
                        _rocket._rotationOrientation = Rocket.ROTATE_CLOCKWISE;
                        _rocket.setTargetRotation(MathHelper.ToDegrees((float)Math.Atan2(clockwise.Y, clockwise.X)));
                    }
                    else
                    {

                        _rocket._rotationOrientation = Rocket.ROTATE_COUNTER;
                        _rocket._targetRotation = MathHelper.ToDegrees((float)Math.Atan2(-1 * clockwise.Y, -1 * clockwise.X));
                    }

                    _lineContainer._lineType = lineTypes.LINE_DASHED;
                }

            }

        }

        private void resetGame()
        {
            _rocket.Position = new CCPoint(_screenSize.Width * 0.5f, _screenSize.Height * 0.1f);
            _rocket.Opacity = 255;
            _rocket.Visible = true;
            _rocket.reset();

            _cometInterval = 4;
            _cometTimer = 0;
            _timeBetweenPickups = 0.0f;

            _score = 0;
            _scoreDisplay.Text = String.Format("{0}", _score);

            _lineContainer.reset();

            //TODO: shuffle grid cells
            //random_shuffle(_grid.begin(), _grid.end());

            _gridIndex = 0;

            resetStar();

            _warp.StopSystem();

            _running = true;

            CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic("music/background", true);
            CCSimpleAudioEngine.SharedEngine.StopAllEffects();
            CCSimpleAudioEngine.SharedEngine.PlayEffect("music/rocket", true);
        }

        public void killPlayer()
        {

            CCSimpleAudioEngine.SharedEngine.StopBackgroundMusic();
            CCSimpleAudioEngine.SharedEngine.StopAllEffects();
            CCSimpleAudioEngine.SharedEngine.PlayEffect("music/shipBoom");

            _boom.Position = _rocket.Position;
            _boom.ResetSystem();
            _rocket.Visible = false;
            _jet.StopSystem(); // ->stopSystem();
            _lineContainer._lineType = lineTypes.LINE_NONE;

            _running = false;
            _state = gamestates.kGameOver;
            _gameOver.Visible = true;
            _pauseBtn.Visible = false;

        }

        public static CCScene Scene
        {
            get
            {
                // 'scene' is an autorelease object.
                var scene = new CCScene();

                // 'layer' is an autorelease object.
                var layer = new IntroLayer();

                // add layer as a child to scene
                scene.AddChild(layer);

                // return the scene
                return scene;

            }

        }


    }


}

