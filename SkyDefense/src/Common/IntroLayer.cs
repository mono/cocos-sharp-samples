using System;
using Microsoft.Xna.Framework;
using CocosDenshion;
using System.Collections.Generic;
using System.Linq;
using CocosSharp;
namespace SkyDefense.Common
{

    public class IntroLayer : CCLayerColor
    {

        //.BlendFunc = new CCBlendFunc(CCOGLES.GL_SRC_ALPHA, CCOGLES.GL_SRC_ALPHA);
        //int blendSrc = CCOGLES.GL_SRC_ALPHA;
        //int blendDst = CCOGLES.GL_ONE_MINUS_SRC_ALPHA;

        int kSpriteBomb = 1;
        int kSpriteShockwave = 2;

        int kSpriteMeteor = 3;
        int kSpriteHealth = 4;
        int kSpriteHalo = 5;
        int kSpriteSparkle = 6;

        int kBackground = 1;
        int kMiddleground = 2;
        int kForeground = 3;

        List<CCSprite> _meteorPool;
        int _meteorPoolIndex;

        List<CCSprite> _healthPool;
        int _healthPoolIndex;


        List<CCSprite> _fallingObjects;
        List<CCSprite> _clouds;

        CCSpriteBatchNode _gameBatchNode;
        //CCNode _gameBatchNode;

        CCSprite _bomb;
        CCSprite _shockWave;

        CCSprite _introMessage;
        CCSprite _gameOverMessage;

        CCLabelTtf _energyDisplay;
        CCLabelTtf _scoreDisplay;

        CCAction _growBomb;
        CCAction _rotateSprite;
        CCAction _shockwaveSequence;
        CCAction _swingHealth;

        CCAction _groundHit;
        CCAction _explosion;

        CCSize _screenSize;

        float _meteorInterval;
        float _meteorTimer;
        float _meteorSpeed;
        float _healthInterval;
        float _healthTimer;
        float _healthSpeed;
        float _difficultyInterval;
        float _difficultyTimer;

        int _energy;
        int _score;
        int _shockwaveHits;
        bool _running;


        public void ProcessSprite(CCSprite sprite)
        {
            //sprite.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            //sprite.IsAntialiased = true;

        }
        public void ProcessSprite(CCSpriteBatchNode sprite)
        {
            //sprite.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            //sprite.IsAntialiased = true;
            //sprite.IsOpacityModifyRGB = true;
            //sprite.
        }

        public IntroLayer()
        {

            //GL_ONE, GL_ZERO
            //CCDrawManager.BlendFunc(new CCBlendFunc(CCOGLES.GL_SRC_ALPHA, CCOGLES.GL_SRC_ALPHA));

            //////////////////////////////
            //get screen size
            _screenSize = CCDirector.SharedDirector.WinSize;

            _running = false;

            //create game screen elements
            createGameScreen();

            //create object pools
            createPools();
            //create CCActions
            createActions();

            //create array to store all falling objects (will use it in collision check)
            _fallingObjects = new List<CCSprite>(40);
            //_fallingObjects->retain();


            //listen for touches
            //TouchEnabled = true;

            CCEventListenerTouchAllAtOnce tListener = new CCEventListenerTouchAllAtOnce();
            tListener.OnTouchesBegan = TouchesBegan;
            tListener.OnTouchesCancelled = TouchesCancelled;
            tListener.OnTouchesEnded = TouchesEnded;
            tListener.OnTouchesMoved = TouchesMoved;
            EventDispatcher.AddEventListener(tListener, this);

            //create main loop
            Schedule(Update);

            CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic("music/background", true);

        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (!_running) return;

            int count;
            int i;
            CCSprite sprite;

            //update timers

            _meteorTimer += dt;
            if (_meteorTimer > _meteorInterval)
            {
                _meteorTimer = 0;
                resetMeteor();
            }

            _healthTimer += dt;
            if (_healthTimer > _healthInterval)
            {
                _healthTimer = 0;
                resetHealth();
            }

            _difficultyTimer += dt;
            if (_difficultyTimer > _difficultyInterval)
            {
                _difficultyTimer = 0;
                increaseDifficulty();
            }

            if (_bomb.Visible)
            {
                if (_bomb.ScaleX > 0.3f)
                {
                    if (_bomb.Opacity != 255)
                        _bomb.Opacity = 255;
                }
            }

            //check collision with shockwave
            if (_shockWave.Visible)
            {
                count = _fallingObjects.Count;

                float diffx;
                float diffy;

                for (i = count - 1; i >= 0; i--)
                {
                    sprite = (CCSprite)_fallingObjects[i];
                    diffx = _shockWave.PositionX - sprite.PositionX;
                    diffy = _shockWave.PositionY - sprite.PositionY;
                    if (CCMathExHelper.pow(diffx, 2) + CCMathExHelper.pow(diffy, 2) <= CCMathExHelper.pow(_shockWave.BoundingBox.Size.Width * 0.5f, 2))
                    {
                        sprite.StopAllActions();
                        sprite.RunAction(_explosion);
                        CCSimpleAudioEngine.SharedEngine.PlayEffect("music/boom");
                        if (sprite.Tag == kSpriteMeteor)
                        {
                            _shockwaveHits++;
                            _score += _shockwaveHits * 13 + _shockwaveHits * 2;
                        }
                        //play sound
                        _fallingObjects.RemoveAt(i);
                    }
                }

                _scoreDisplay.Text = String.Format("{0}", _score);
            }

            //move clouds
            count = _clouds.Count;
            for (i = 0; i < count; i++)
            {
                sprite = _clouds[i];
                sprite.PositionX = sprite.PositionX + dt * 20;
                if (sprite.PositionX > _screenSize.Width + sprite.BoundingBox.Size.Width * 0.5f)
                    sprite.PositionX = -sprite.BoundingBox.Size.Width * 0.5f;
            }

        }

        protected override void TouchesBegan(List<CCTouch> touches, CCEvent touchEvent)
        {

            base.TouchesBegan(touches, touchEvent);

            if (!_running)
            {

                //if intro, hide intro message
                if (_introMessage.Visible)
                {
                    _introMessage.Visible = false;

                    //if game over, hide game over message          
                }
                else if (_gameOverMessage.Visible)
                {
                    CCSimpleAudioEngine.SharedEngine.StopAllEffects();
                    _gameOverMessage.Visible = false;

                }

                resetGame();
                return;
            }

            CCTouch touch = touches.FirstOrDefault();

            if (touch != null)
            {

                //if bomb already growing...
                if (_bomb.Visible)
                {
                    //stop all actions on bomb, halo and sparkle
                    _bomb.StopAllActions();
                    CCSprite child;
                    child = (CCSprite)_bomb.GetChildByTag(kSpriteHalo);
                    ProcessSprite(child);
                    //child.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                    child.StopAllActions();
                    child = (CCSprite)_bomb.GetChildByTag(kSpriteSparkle);
                    ProcessSprite(child);
                    //child.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                    child.StopAllActions();

                    //if bomb is the right size, then create shockwave
                    if (_bomb.ScaleX > 0.3f)
                    {

                        _shockWave.Scale = 0.1f;
                        _shockWave.Position = _bomb.Position;
                        _shockWave.Visible = true;
                        _shockWave.RunAction(new CCScaleTo(0.5f, _bomb.ScaleX * 2.0f));
                        _shockWave.RunAction((CCFiniteTimeAction)_shockwaveSequence);
                        CCSimpleAudioEngine.SharedEngine.PlayEffect("music/bombRelease");

                    }
                    else
                    {
                        CCSimpleAudioEngine.SharedEngine.PlayEffect("music/bombFail");
                    }
                    _bomb.Visible = false;
                    //reset hits with shockwave, so we can count combo hits
                    _shockwaveHits = 0;

                    //if no bomb currently on screen, create one
                }
                else
                {

                    CCPoint tap = touch.Location;
                    _bomb.StopAllActions();
                    _bomb.Scale = 0.1f;
                    _bomb.Position = tap;
                    _bomb.Visible = true;
                    _bomb.Opacity = 50;
                    _bomb.RunAction(_growBomb);

                    CCSprite child;
                    child = (CCSprite)_bomb.GetChildByTag(kSpriteHalo);
                    ProcessSprite(child);
                    //child.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                    child.RunAction(_rotateSprite);
                    child = (CCSprite)_bomb.GetChildByTag(kSpriteSparkle);
                    ProcessSprite(child);
                    //child.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                    child.RunAction(_rotateSprite);
                }
            }

        }

        private void createActions()
        {
            //swing action for health drops
            CCFiniteTimeAction easeSwing = new CCSequence(
                           new CCEaseInOut(new CCRotateTo(1.2f, -10), 2),
                           new CCEaseInOut(new CCRotateTo(1.2f, 10), 2));
            _swingHealth = new CCRepeatForever((CCActionInterval)easeSwing);
            //_swingHealth ->retain();

            //action sequence for shockwave: fade out, call back when done
            _shockwaveSequence = new CCSequence(
                            new CCFadeOut(1.0f),
                            new CCCallFunc(shockwaveDone));
            //_shockwaveSequence->retain();

            //action to grow bomb
            _growBomb = new CCScaleTo(6.0f, 1);
            //_growBomb->retain();

            //action to rotate sprites
            CCActionInterval rotate = new CCRotateBy(0.5f, -90);
            _rotateSprite = new CCRepeatForever(rotate);
            //_rotateSprite->retain();


            //sprite animations
            CCAnimation animation;

            animation = new CCAnimation();
            CCSpriteFrame frame;
            int i;
            //animation for ground hit
            for (i = 1; i <= 10; i++)
            {

                frame = CCSpriteFrameCache.Instance[String.Format("boom{0}.png", i)];
                animation.AddSpriteFrame(frame);
            }

            animation.DelayPerUnit = (1 / 10.0f);
            animation.RestoreOriginalFrame = true;
            _groundHit = new CCSequence(
                        new CCMoveBy(0, new CCPoint(0, _screenSize.Height * 0.12f)),
                        new CCAnimate(animation),
                        new CCCallFuncN(animationDone));
            //_groundHit->retain();

            animation = new CCAnimation();
            //animation for falling object explosion
            for (i = 1; i <= 7; i++)
            {

                frame = CCSpriteFrameCache.Instance[String.Format("explosion_small{0}.png", i)];

                animation.AddSpriteFrame(frame);
            }

            animation.DelayPerUnit = 0.5f / 7.0f;
            animation.RestoreOriginalFrame = true;
            _explosion = new CCSequence(
                    new CCAnimate(animation),
                    new CCCallFuncN(animationDone)
                    );
            ;
            // _explosion->retain();
        }

        private void createPools()
        {

            CCSprite sprite;
            int i;

            //create meteor pool
            _meteorPool = new List<CCSprite>(50);
            //_meteorPool->retain();
            _meteorPoolIndex = 0;
            for (i = 0; i < 50; i++)
            {
                sprite = new CCSprite("meteor.png"); //new CCSprite("meteor.png");
                //sprite.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                ProcessSprite(sprite);
                sprite.Visible = false;
                _gameBatchNode.AddChild(sprite, kMiddleground, kSpriteMeteor);
                _meteorPool.Add(sprite);
            }

            //create health pool
            _healthPool = new List<CCSprite>(20);
            //_healthPool->retain();
            _healthPoolIndex = 0;
            for (i = 0; i < 20; i++)
            {
                sprite = new CCSprite("health.png");  //new CCSprite("health.png");
                //sprite.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                ProcessSprite(sprite);
                sprite.Visible = false;
                sprite.AnchorPoint = new CCPoint(0.5f, 0.8f);
                _gameBatchNode.AddChild(sprite, kMiddleground, kSpriteHealth);
                _healthPool.Add(sprite);
            }
        }

        private void createGameScreen()
        {

            //add bg
            CCSprite bg = new CCSprite("bg");
            bg.SetPosition(_screenSize.Width * 0.5f, _screenSize.Height * 0.5f);
            AddChild(bg);

            //create spritebatch node
            CCSpriteFrameCache.Instance.AddSpriteFrames("sprite_sheet.plist");

            _gameBatchNode = new CCSpriteBatchNode("sprite_sheet.png");

            //ProcessSprite(_gameBatchNode);
            //_gameBatchNode.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            AddChild(_gameBatchNode);

            //create cityscape
            CCSprite sprite;
            for (int i = 0; i < 2; i++)
            {

                sprite = new CCSprite("city_dark.png"); //new CCSprite("city_dark.png");
                //sprite.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                ProcessSprite(sprite);

                sprite.SetPosition(_screenSize.Width * (0.25f + i * 0.5f),
                                      sprite.BoundingBox.Size.Height * 0.5f);

                _gameBatchNode.AddChild(sprite, kForeground);

                sprite = new CCSprite("city_light.png");// new CCSprite("city_light.png");
                //sprite.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                ProcessSprite(sprite);

                sprite.SetPosition(_screenSize.Width * (0.25f + i * 0.5f),
                                      sprite.BoundingBox.Size.Height * 0.9f);

                _gameBatchNode.AddChild(sprite, kBackground);

            }

            //add trees
            for (int i = 0; i < 3; i++)
            {

                sprite = new CCSprite("trees.png");// new CCSprite("trees.png");

                //sprite.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                ProcessSprite(sprite);

                sprite.SetPosition(_screenSize.Width * (0.2f + i * 0.3f),
                                        sprite.BoundingBox.Size.Height * 0.5f);

                _gameBatchNode.AddChild(sprite, kForeground);

            }

            //add HUD
            _scoreDisplay = new CCLabelTtf("0", "MarkerFelt", 22);
            _scoreDisplay.AnchorPoint = new CCPoint(1f, 0.5f);
            _scoreDisplay.SetPosition(_screenSize.Width * 0.8f, _screenSize.Height * 0.94f);
            AddChild(_scoreDisplay);

            _energyDisplay = new CCLabelTtf("100%", "MarkerFelt", 22);
            _energyDisplay.SetPosition(_screenSize.Width * 0.3f, _screenSize.Height * 0.94f);
            AddChild(_energyDisplay);

            CCSprite icon = new CCSprite("health_icon.png"); // new CCSprite("health_icon.png");
            icon.SetPosition(_screenSize.Width * 0.15f, _screenSize.Height * 0.94f);

            //icon.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            ProcessSprite(icon);

            _gameBatchNode.AddChild(icon, kBackground);

            //add clouds
            CCSprite cloud;
            _clouds = new List<CCSprite>(4);
            //_clouds

            float cloud_y;
            for (int i = 0; i < 4; i++)
            {
                cloud_y = i % 2 == 0 ? _screenSize.Height * 0.4f : _screenSize.Height * 0.5f;
                cloud = new CCSprite(CCSpriteFrameCache.Instance["cloud.png"]); // new CCSprite("cloud.png");
                ProcessSprite(cloud);
                //cloud.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
                cloud.SetPosition(_screenSize.Width * 0.1f + i * _screenSize.Width * 0.3f, cloud_y);
                _gameBatchNode.AddChild(cloud, kBackground);
                _clouds.Add(cloud);
            }

            //CREATE BOMB SPRITE
            _bomb = new CCSprite("bomb.png");
            //_bomb.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            ProcessSprite(_bomb);
            //_bomb.Texture.GenerateMipmap();
            _bomb.Visible = false;

            CCSize size = _bomb.BoundingBox.Size;

            //add sparkle inside bomb sprite
            CCSprite sparkle = new CCSprite("sparkle.png");
            //sparkle.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            ProcessSprite(sparkle);
            sparkle.SetPosition(size.Width * 0.72f, size.Height * 0.72f);
            _bomb.AddChild(sparkle, kMiddleground, kSpriteSparkle);


            //add halo inside bomb sprite
            CCSprite halo = new CCSprite("halo.png");
            //halo.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            ProcessSprite(halo);
            halo.SetPosition(size.Width * 0.4f, size.Height * 0.4f);
            _bomb.AddChild(halo, kMiddleground, kSpriteHalo);
            _gameBatchNode.AddChild(_bomb, kForeground);

            //add shockwave
            _shockWave = new CCSprite("shockwave.png");
            //_shockWave.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            ProcessSprite(_shockWave);

            _shockWave.Visible = false;
            _gameBatchNode.AddChild(_shockWave);

            //intro message
            _introMessage = new CCSprite("logo.png");
            //_introMessage.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            ProcessSprite(_introMessage);
            _introMessage.SetPosition(_screenSize.Width * 0.5f, _screenSize.Height * 0.6f);
            _introMessage.Visible = true;
            AddChild(_introMessage, kForeground);

            //game over message
            _gameOverMessage = new CCSprite("gameover.png");
            _gameOverMessage.SetPosition(_screenSize.Width * 0.5f, _screenSize.Height * 0.65f);

            //_gameOverMessage.BlendFunc = new CCBlendFunc(blendSrc, blendDst);
            ProcessSprite(_gameOverMessage);
            _gameOverMessage.Visible = false;
            AddChild(_gameOverMessage, kForeground);

        }

        private void resetGame()
        {
            _score = 0;
            _energy = 100;

            //reset timers and "speeds"
            _meteorInterval = 2.5f;
            _meteorTimer = _meteorInterval * 0.99f;
            _meteorSpeed = 10;//in seconds to reach ground
            _healthInterval = 20;
            _healthTimer = 0;
            _healthSpeed = 15;//in seconds to reach ground

            _difficultyInterval = 60;
            _difficultyTimer = 0;

            _running = true;

            //reset labels

            _energyDisplay.Text = String.Format("{0}%", _energy);

            _scoreDisplay.Text = String.Format("{0}", _score);
        }

        private void increaseDifficulty()
        {
            _meteorInterval -= 0.2f;
            if (_meteorInterval < 0.25f) _meteorInterval = 0.25f;
            _meteorSpeed -= 1;
            if (_meteorSpeed < 4) _meteorSpeed = 4;
            _healthSpeed -= 1;
            if (_healthSpeed < 8) _healthSpeed = 8;
        }

        private void resetHealth()
        {
            if (_fallingObjects.Count > 30) return;

            CCSprite health = (CCSprite)_healthPool[_healthPoolIndex];
            _healthPoolIndex++;
            if (_healthPoolIndex == _healthPool.Count) _healthPoolIndex = 0;

            int health_x = CCRandom.GetRandomInt(0, 1000) % (int)(_screenSize.Width * 0.8f + _screenSize.Width * 0.1f);
            int health_target_x = CCRandom.GetRandomInt(0, 1000) % (int)(_screenSize.Width * 0.8f + _screenSize.Width * 0.1f);

            health.StopAllActions();
            health.SetPosition(health_x, _screenSize.Height + health.BoundingBox.Size.Height * 0.5f);

            //create action (swing, move to target, and call function when done)
            CCFiniteTimeAction sequence = new CCSequence(
                   new CCMoveTo(_healthSpeed, new CCPoint(health_target_x, _screenSize.Height * 0.15f)),
                   new CCCallFuncN(fallingObjectDone));

            health.Visible = true;
            health.RunAction(_swingHealth);
            health.RunAction(sequence);
            _fallingObjects.Add(health);
        }

        private void resetMeteor()
        {
            //if too many objects on screen, return
            if (_fallingObjects.Count > 30) return;


            CCSprite meteor = _meteorPool[_meteorPoolIndex];
            _meteorPoolIndex++;
            if (_meteorPoolIndex == _meteorPool.Count) _meteorPoolIndex = 0;

            //pick start and target positions for new meteor
            int meteor_x = CCRandom.GetRandomInt(1, 1000) % (int)((_screenSize.Width * 0.8f) + _screenSize.Width * 0.1f);
            int meteor_target_x = CCRandom.GetRandomInt(1, 1000) % (int)((_screenSize.Width * 0.8f) + _screenSize.Width * 0.1f);

            meteor.StopAllActions();
            meteor.SetPosition(meteor_x, _screenSize.Height + meteor.BoundingBox.Size.Height * 0.5f);

            //create action for meteor (rotate forever, move to target, and call function)
            CCActionInterval rotate = new CCRotateBy(0.5f, -90);
            CCAction repeatRotate = new CCRepeatForever(rotate);
            CCFiniteTimeAction sequence = new CCSequence(
                       new CCMoveTo(_meteorSpeed, new CCPoint(meteor_target_x, _screenSize.Height * 0.15f)),
                       new CCCallFuncN(fallingObjectDone));

            meteor.Visible = true;
            meteor.RunAction(repeatRotate);
            meteor.RunAction(sequence);
            _fallingObjects.Add(meteor);

        }

        private void animationDone(CCNode obj)
        {
            obj.Visible = false; ;
        }

        private void shockwaveDone()
        {
            _shockWave.Visible = false;
        }

        private void fallingObjectDone(CCNode pSenderobj)
        {
            CCSprite pSender = (CCSprite)pSenderobj;
            //remove it from array
            _fallingObjects.Remove(pSender);
            pSender.StopAllActions();
            pSender.Rotation = 0;

            //if object is a meteor...
            if (pSender.Tag == kSpriteMeteor)
            {
                _energy -= 15;
                //show explosion animation
                pSender.RunAction((CCAction)_groundHit);
                //play explosion sound
                CCSimpleAudioEngine.SharedEngine.PlayEffect("boom");

                //if object is a health drop...
            }
            else
            {
                pSender.Visible = false;

                //if energy is full, score points from falling drop
                if (_energy == 100)
                {

                    _score += 25;

                    _scoreDisplay.Text = String.Format("{0}", _score);

                }
                else
                {
                    _energy += 10;
                    if (_energy > 100) _energy = 100;
                }

                //play health bonus sound
                CCSimpleAudioEngine.SharedEngine.PlayEffect("health.png");
            }

            //if energy is less or equal 0, game over
            if (_energy <= 0)
            {
                _energy = 0;
                stopGame();
                CCSimpleAudioEngine.SharedEngine.PlayEffect("fire_truck");
                //show GameOver
                _gameOverMessage.Visible = true;
            }

            _energyDisplay.Text = String.Format("{0}%", _energy);
        }

        private void stopGame()
        {

            _running = false;
            //stop all actions currently running (meteors, health drops, animations...)
            int i;
            int count;
            CCSprite sprite;
            count = _fallingObjects.Count;
            for (i = count - 1; i >= 0; i--)
            {
                sprite = _fallingObjects[i];
                sprite.StopAllActions();
                sprite.Visible = false;
                _fallingObjects.RemoveAt(i);
            }
            if (_bomb.Visible)
            {
                _bomb.StopAllActions();
                _bomb.Visible = false;
                CCSprite child;
                child = (CCSprite)_bomb.GetChildByTag(kSpriteHalo);
                child.StopAllActions();
                child = (CCSprite)_bomb.GetChildByTag(kSpriteSparkle);
                child.StopAllActions();
            }
            if (_shockWave.Visible)
            {
                _shockWave.StopAllActions();
                _shockWave.Visible = false;
            }

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

