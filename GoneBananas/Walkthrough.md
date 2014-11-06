# CocosSharp GoneBananas Tutorial

This tutorial shows how to build a game using CocosSharp, a game framework that allows cross-platform game development with C#. The tutorial shows how to go from setting up a new project to building a complete game. It shows how to work with various CocosSharp concepts such as sprites, actions, sounds, layers, parallax, particle systems, scenes and physics. When you are done you will have built your first CocosSharp game.

## Getting CocosSharp

CocosSharp is available as either PCLs or platform specific libraries in [NuGet](https://github.com/mono/CocosSharp/wiki/NuGet-Packages). Additionally, the CocosSharp source can be pulled from github [here](https://github.com/mono/CocosSharp).

## Game Walkthrough

In this tutorial we're going to build a game called _GoneBananas_. The object of the game is to move a monkey around the screen to capture as many falling bananas as possible. A screenshot of the game is shown below:

![GoneBananas](screenshots/GoneBananas.png?raw=true "Gone Bananas")

We're going to create a game for iOS and Android in this tutorial. However, CocosSharp runs on many other platforms as well. See the CocosSharp repo for the full list of supported platforms. 

In the first part of the tutorial we'll cover some basics such as setting up the game, working with sprites and transitioning between screens. Then, we'll show how to work with more advanced concepts such as particle systems and physics. Let's get started by creating a new application called **GoneBananas** using the **iOS** > **iPhone Empty Project** template.

After creating the project, we need to add CocosSharp and its dependencies to the solution. Simply add the CocosSharp package from NuGet.

Also add a new Android project named GoneBananasAndroid using the **Android** > **Ice Cream Sandwich Application** template, and add the CocosSharp NuGet package there as well.

We're going to do the majority of the work here within a shared project, so that the game logic will be reusable across platforms. Add a new shared project to the solution named GoneBananasShared.

With the projects created and CocosSharp added, we are now ready to create the game.

### Creating the Application Delegate Class

The first thing we need to create is the CCApplicationDelegate subclass. A CCApplicationDelegate is conceptually similar to a UIApplicationDelegate in iOS. This is the class that handles application lifecycle events, as outlined below:

- ApplicationDidFinishLaunching – Runs after the application has launched.
- ApplicationDidEnterBackground – This is where we stop all running animations. Also, if playing background audio, that needs to be stopped here as well.
- ApplicationWillEnterForeground - This is where we resume the animations and audio that we stopped when the application entered the background.

Add a new class named GoneBananasApplicationDelegate with the following implementation:

    using CocosDenshion;
    using CocosSharp;

    namespace GoneBananas
    {
        public class GoneBananasApplicationDelegate : CCApplicationDelegate
        {

            public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
            {
                application.PreferMultiSampling = false;
                application.ContentRootDirectory = "Content";
                application.ContentSearchPaths.Add("hd");

                CCSimpleAudioEngine.SharedEngine.PreloadEffect ("Sounds/tap");
                
                CCSize winSize = mainWindow.WindowSizeInPixels;
                mainWindow.SetDesignResolutionSize(winSize.Width, winSize.Height, CCSceneResolutionPolicy.ExactFit);

                CCScene scene = GameStartLayer.GameStartLayerScene(mainWindow);
                mainWindow.RunWithScene (scene);
            }

            public override void ApplicationDidEnterBackground (CCApplication application)
            {
                // stop all of the animation actions that are running.
                application.Paused = true;
    			
                // if you use SimpleAudioEngine, your music must be paused
                CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic ();
            }

            public override void ApplicationWillEnterForeground (CCApplication application)
            {
                application.Paused = false;
    			
                // if you use SimpleAudioEngine, your background music track must resume here. 
                CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic ();
            }
        }
    }

In the GoneBananasApplicationDelegate ApplicationFinishedLaunching method, we initialize the mainWindow so that the game will run in portrait for this example. We then preload an audio effect, and run the initial scene.

### Content Folder

In the GoneBananasApplicationDelegate class we set the application.ContentRootDirectory to "Content" for the name of the project folder that contains resources such as fonts, images and sounds. This folder, along with subfolder containing various resources, is available in the completed sample project that accompanies this article. Copy this folder to the **GoneBananas** iOS and Android projects respectively (the Android folder should be added under Assets).

### CCApplication Class

The CCApplication class is used to start the game. We'll need to create this in the platform specific project. This is the only code that will be needed in the platform specific projects.

On iOS, add the the following code in the AppDelegate:

    public partial class AppDelegate : UIApplicationDelegate
    {
        public override void FinishedLaunching (UIApplication app)
        {
            var application = new CCApplication ();
            application.ApplicationDelegate = new GoneBananasApplicationDelegate ();
            application.StartGame ();
        }
    }

Similarly, for Android, add code to create the application in the MainActivity:

    public class MainActivity : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var application = new CCApplication();
            application.ApplicationDelegate = new GoneBananasApplicationDelegate();
            SetContentView(application.AndroidContentView);
            application.StartGame();
        }
    }

### Creating the First Scene

In the GoneBananasApplicationDelegate class, we created an instance of the first scene to run using this line:

    CCScene scene = GameStartLayer.GameStartLayerScene(mainWindow);

CocosSharp uses scenes, implemented in the CCScene class, to manage game logic for various portions of the game. Each scene subsequently contains layers to present the user interface for the scene. Each layer is responsible for returning its parent scene. For example, in this game, we'll create three layers:

- GameStartLayer – Provides the introductory screen. In this game, tapping this layer will start the actual gameplay.
- GameLayer – Contains the actual gameplay level.
- GameOverLayer – Contains the screen that is displayed when the game has ended.

To implement the GameStartLayer, add a class with the following code:

    public class GameStartLayer : CCLayerColor
    {
        public GameStartLayer () : base ()
        {
            var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene (GameLayer.GameScene (Window));

            AddEventListener (touchListener, this);

            Color = CCColor3B.Black;
            Opacity = 255;
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            var label = new CCLabelTtf ("Tap Screen to Go Bananas!", "arial", 22) {
                Position = VisibleBoundsWorldspace.Center,
                Color = CCColor3B.Green,
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
                Dimensions = ContentSize
            };

            AddChild (label);
        }

        public static CCScene GameStartLayerScene (CCWindow mainWindow)
        {
            var scene = new CCScene (mainWindow);
            var layer = new GameStartLayer ();

            scene.AddChild (layer);

            return scene;
        }
    }

We use a CCLayerColor for the base class so we can set the layer's background color. The code displays a label and transitions the user to the GameLayer's scene when the screen is tapped. The label uses a arial font that is included in the project's **fonts** folder that we created earlier when we copied in the **Content** folder.

The following screenshot shows the completed scene:

![GameStart](screenshots/GameStart.png?raw=true "Game Start")

### Transitioning to the GameLayer's Scene

Calling Window.DefaultDirector.ReplaceScene, passing it the scene to display, does the actual transitioning between scenes. In this case, it transitions to the GameLayer's scene with the following line:

    Window.DefaultDirector.ReplaceScene (GameLayer.GameScene (Window));

### Implementing the GameLayer

For the GameLayer, create a class named GameLayer, which again inherits from CCLayerColor. We'll be using a variety of class variables throughout this tutorial, so let's go ahead and create them here:

    public class GameLayer : CCLayerColor
    {
        const float MONKEY_SPEED = 350.0f;
        const float GAME_DURATION = 60.0f; // game ends after 60 seconds or when the monkey hits a ball, whichever comes first

        // point to meter ratio for physics
        const int PTM_RATIO = 32;

        float elapsedTime;
        CCSprite monkey;
        List<CCSprite> visibleBananas;
        List<CCSprite> hitBananas;

        // monkey walking animation
        CCAnimation walkAnim;
        CCRepeatForever walkRepeat;
        CCCallFuncN walkAnimStop = new CCCallFuncN (node => node.StopAllActions ());

        // background sprite
        CCSprite grass;

        // particles
        CCParticleSun sun;

        // circle layered behind sun
        CCDrawNode circleNode;

        // parallax node for clouds
        CCParallaxNode parallaxClouds;
            
        // define our banana rotation action
        CCRotateBy rotateBanana = new CCRotateBy (0.8f, 360);

        // define our completion action to remove the banana once it hits the bottom of the screen
        CCCallFuncN moveBananaComplete = new CCCallFuncN (node => node.RemoveFromParent ());

        ...
    }

#### Adding a Background Sprite

A sprite, represented by a CCSprite instance, is added to a layer's node heirarchy by calling AddChild. The background sprite is loaded from a file in the Content folder. Add the following code to create a background:

    void AddGrass ()
    {
        grass = new CCSprite ("grass");
        AddChild (grass);
    }

#### Creating Banana Sprites

Each banana that falls from the screen is represented by a sprite created from an image. The banana image is contained in a sprite sheet. A sprite sheet is an image that packs several images together so they can be loaded efficiently. An associated plist file contains information about where particular images, such as the banana, are located within the sprite sheet.

Add a method called AddBanana to create a banana sprite and add it to the layer as follows:

    CCSprite AddBanana ()
    {
        var spriteSheet = new CCSpriteSheet ("animations/monkey.plist");
        var banana = new CCSprite (spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("Banana")));

        var p = GetRandomPosition (banana.ContentSize);
        banana.Position = p;
        banana.Scale = 0.5f;

        AddChild (banana);

		//TODO: animate banana falling

        return banana;
    }

This code adds a sprite at a random x location along the top of the screen. Let's see how to animate each sprite so it moves down the screen until it falls off the bottom.

#### Using Actions to Animate Bananas

CocosSharp includes various CCAction classes to perform different tasks on a node, which includes sprites since CCSprite inherits from CCNode. In this example we are going to use a CCMoveTo action to animate the banana sprites and a CCCallFuncN action to run code that will remove the sprite when the animation completes. We accomplish this by including both actions in a CCSequence, which itself is an action that includes other actions to run sequentially.

Add the following code to the AddBanana method just before returning the banana sprite:

    CCSprite AddBanana ()
    {
        var spriteSheet = new CCSpriteSheet ("animations/monkey.plist");
        var banana = new CCSprite (spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("Banana")));

        var p = GetRandomPosition (banana.ContentSize);
        banana.Position = p;
        banana.Scale = 0.5f;

        AddChild (banana);
					
        // animate banana falling
        var moveBanana = new CCMoveTo (5.0f, new CCPoint (banana.Position.X, 0));
        banana.RunActions (moveBanana, moveBananaComplete);
        banana.RepeatForever (rotateBanana);

        return banana;
    }

#### Adding the Monkey Sprite

Next, add the following constructor code to the GameLayer class:

    public GameLayer ()
    {
        var touchListener = new CCEventListenerTouchAllAtOnce ();
        touchListener.OnTouchesEnded = OnTouchesEnded;

        Color = new CCColor3B (CCColor4B.White);
        Opacity = 255;

        visibleBananas = new List<CCSprite> ();
        hitBananas = new List<CCSprite> ();

        AddGrass ();
        AddSun (); // we'll implement this later using a particle system
        AddMonkey ();

    }

The visibleBananas and hitBananas will be used to manage scoring and hit testing for the game, which we will implement shortly. The monkey sprite is also created from the sprite sheet, only for the monkey, we'll use several images to animate the monkey walking.

    void AddMonkey ()
    {
        var spriteSheet = new CCSpriteSheet ("animations/monkey.plist");
        var animationFrames = spriteSheet.Frames.FindAll ((x) => x.TextureFilename.StartsWith ("frame"));

        walkAnim = new CCAnimation (animationFrames, 0.1f);
        walkRepeat = new CCRepeatForever (new CCAnimate (walkAnim));
        monkey = new CCSprite (animationFrames.First ()) { Name = "Monkey" };
        monkey.Scale = 0.25f;

        AddChild (monkey);
    }

#### Moving the Monkey Sprite on Touch

To animate the monkey sprite to the location where the user touches the screen, we override TouchesEnded and create another MoveTo action as follows:

    void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
    {
        monkey.StopAllActions ();

        var location = touches [0].LocationOnScreen;
        location = WorldToScreenspace (location);
        float ds = CCPoint.Distance (monkey.Position, location);

        var dt = ds / MONKEY_SPEED;

        var moveMonkey = new CCMoveTo (dt, location);

        monkey.RunAction (walkRepeat);
        monkey.RunActions (moveMonkey, walkAnimStop);
    }

#### Collision Detection

For the collision detection between the monkey and the banana, we'll simply do a test on the bounding box for each sprite. We'll also keep track of the bananas that are "hit" by the monkey in order to remove them from the layer, as well as keep score.

Add the following code to check collisions:

    void CheckCollision ()
    {
        visibleBananas.ForEach (banana => {
            bool hit = banana.BoundingBoxTransformedToParent.IntersectsRect (monkey.BoundingBoxTransformedToParent);
            if (hit) {
                hitBananas.Add (banana);
                CCSimpleAudioEngine.SharedEngine.PlayEffect ("Sounds/tap");
                Explode (banana.Position); // we'll implement this later using a particle systtem
                banana.RemoveFromParent ();
            }
        });

        hitBananas.ForEach (banana => visibleBananas.Remove (banana));
    }

#### Scheduling 

To run code at a repeated interval in order to add bananas, check collisions and test for the game ending logic we can use the Schedule method, which acts as a timer. We can schedule code for our purposes at the end of the constructor we created earlier as follows:

    public GameLayer ()
    {
        ...

        Schedule (t => {
            visibleBananas.Add (AddBanana ());
            elapsedTime += t;
            if (ShouldEndGame ()) {
                EndGame ();
            }
        }, 1.0f);

        Schedule (t => CheckCollision ());
    }

#### Playing a Sound Effect

Let's play a sound effect the monkey collides with a banana. The file tap.mp3 is included in the **Content/Sounds** folder.

To play a sound effect, we use the CCSimpleAudioEngine class. Add the following line to the end of the TouchesEnded method:

    CCSimpleAudioEngine.SharedEngine.PlayEffect ("Sounds/tap");

#### Adding Game Ending Logic

We also need some way to end the game. For this example, we'll simply end the game after 60 seconds using the following code:

    bool ShouldEndGame ()
    {
        return elapsedTime > GAME_DURATION;
    }

    void EndGame ()
    {
        var gameOverScene = GameOverLayer.SceneWithScore (Window, hitBananas.Count);
        var transitionToGameOver = new CCTransitionMoveInR (0.3f, gameOverScene);
        Director.ReplaceScene (transitionToGameOver);
    }

#### Adding Clouds with Parallax

CocosSharp includes a CCParallaxNode that we can use to add sprites that should move at different relative speeds.

Add the following method to include cloud sprits that move at different speeds relative to the monkey's motion:

    void AddClouds ()
    {
        float h = VisibleBoundsWorldspace.Size.Height;

        parallaxClouds = new CCParallaxNode {
            Position = new CCPoint (0, h)
        };
         
        AddChild (parallaxClouds);

        var cloud1 = new CCSprite ("cloud");
        var cloud2 = new CCSprite ("cloud");
        var cloud3 = new CCSprite ("cloud");

        float yRatio1 = 1.0f;
        float yRatio2 = 0.15f;
        float yRatio3 = 0.5f;

        parallaxClouds.AddChild (cloud1, 0, new CCPoint (1.0f, yRatio1), new CCPoint (100, -100 + h - (h * yRatio1)));
        parallaxClouds.AddChild (cloud2, 0, new CCPoint (1.0f, yRatio2), new CCPoint (250, -200 + h - (h * yRatio2)));
        parallaxClouds.AddChild (cloud3, 0, new CCPoint (1.0f, yRatio3), new CCPoint (400, -150 + h - (h * yRatio3)));
    }

#### Adding Particle Systems

CocosSharp includes several particle systems for creating a variety of interesting effects such as fire, smoke, sun and explosion to name just a few.

In GoneBananas, let's add an explosion when the monkey collides with a banana:

    void Explode (CCPoint pt)
    {
        var explosion = new CCParticleExplosion (pt);
        explosion.TotalParticles = 10;
        explosion.AutoRemoveOnFinish = true;
        AddChild (explosion);
    }

Also, let create a sun in the upper right, which will animate with a subtle effect.

    void AddSun ()
    {
        circleNode = new CCDrawNode ();
        circleNode.DrawSolidCircle (CCPoint.Zero, 30.0f, CCColor4B.Yellow);
        AddChild (circleNode);

        sun = new CCParticleSun (CCPoint.Zero);
        sun.StartColor = new CCColor4F (CCColor3B.Red);
        sun.EndColor = new CCColor4F (CCColor3B.Yellow);
        AddChild (sun);
    }

#### Inintializing the Scene

We also need to initialize several of the things we've implemented, such as initial sprite positions. We can do this in the AddedToScene method, as shown below:

    protected override void AddedToScene ()
    {
        base.AddedToScene ();

        Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.NoBorder;

        grass.Position = VisibleBoundsWorldspace.Center;
        monkey.Position = VisibleBoundsWorldspace.Center;

        var b = VisibleBoundsWorldspace;
        sun.Position = b.UpperRight.Offset (-100, -100);

        circleNode.Position = sun.Position;

        AddClouds ();
    }

#### Creating the Scene

We need to provide a way for the layer to return its scene, just like we did earlier for the GameStartLayer. Add the following static property to accomplish this:

    public static CCScene GameScene (CCWindow mainWindow)
    {
        var scene = new CCScene (mainWindow);
        var layer = new GameLayer ();
        
        scene.AddChild (layer);

        return scene;
    }

#### Adding Physics

Let's spruc eup the game a bit by adding a fetaure that uses physics. We'll be using the C# port of Box 2D in this case to add bouncing balls to the game. The object is to avoid the bouncing balls while collecting bananas.

We'll need a couple class variables for the physics world and ball sprites respectively:

    // physics world
    b2World world;
    
    // balls sprite batch
    CCSpriteBatchNode ballsBatch;
    CCTexture2D ballTexture;

We can then add code to create the batch node for the ball sprites in the constructor.

    // batch node for physics balls
    ballsBatch = new CCSpriteBatchNode ("balls", 100);
    ballTexture = ballsBatch.Texture;
    AddChild (ballsBatch, 1, 1);

A CCSpriteBatchNode renders all its sprites together, which is more effecticient for the GPU.

Next, add the following code to initialize the physics world and add the ball sprites:

    void InitPhysics ()
    {
        CCSize s = Layer.VisibleBoundsWorldspace.Size;

        var gravity = new b2Vec2 (0.0f, -10.0f);
        world = new b2World (gravity);

        world.SetAllowSleeping (true);
        world.SetContinuousPhysics (true);

        var def = new b2BodyDef ();
        def.allowSleep = true;
        def.position = b2Vec2.Zero;
        def.type = b2BodyType.b2_staticBody;
        b2Body groundBody = world.CreateBody (def);
        groundBody.SetActive (true);

        b2EdgeShape groundBox = new b2EdgeShape ();
        groundBox.Set (b2Vec2.Zero, new b2Vec2 (s.Width / PTM_RATIO, 0));
        b2FixtureDef fd = new b2FixtureDef ();
        fd.shape = groundBox;
        groundBody.CreateFixture (fd);
    }

    void AddBall ()
    {
        int idx = (CCRandom.Float_0_1 () > .5 ? 0 : 1);
        int idy = (CCRandom.Float_0_1 () > .5 ? 0 : 1);
        var sprite = new CCPhysicsSprite (ballTexture, new CCRect (32 * idx, 32 * idy, 32, 32), PTM_RATIO);

        ballsBatch.AddChild (sprite);

        CCPoint p = GetRandomPosition (sprite.ContentSize);

        sprite.Position = new CCPoint (p.X, p.Y);

        var def = new b2BodyDef ();
        def.position = new b2Vec2 (p.X / PTM_RATIO, p.Y / PTM_RATIO);
        def.linearVelocity = new b2Vec2(0.0f, - 1.0f);
        def.type = b2BodyType.b2_dynamicBody;
        b2Body body = world.CreateBody (def);

        var circle = new b2CircleShape ();
        circle.Radius = 0.5f;

        var fd = new b2FixtureDef ();
        fd.shape = circle;
        fd.density = 1f;
        fd.restitution = 0.85f;
        fd.friction = 0f;
        body.CreateFixture (fd);

        sprite.PhysicsBody = body;

        Console.WriteLine ("sprite batch node count = {0}", ballsBatch.ChildrenCount);
    }

    public override void OnEnter ()
    {
        base.OnEnter ();

        InitPhysics ();
    }

Also add a collision check to determine if a monkey collides with a ball in the CheckCollision method:

    hitBananas.ForEach (banana => visibleBananas.Remove (banana));

    int ballHitCount = ballsBatch.Children.Count (ball => ball.BoundingBoxTransformedToParent.IntersectsRect (monkey.BoundingBoxTransformedToParent));

    if (ballHitCount > 0) {
        EndGame ();
    }

Finally, we need to call AddBall in the scheduler that we added earlier to add bananas, as well as an additional scheduler to timestep the physics world:

    Schedule (t => {
        visibleBananas.Add (AddBanana ());
        elapsedTime += t;
        if (ShouldEndGame ()) {
            EndGame ();
        }
        AddBall ();
    }, 1.0f);

    Schedule (t => {
        world.Step (t, 8, 1);

        foreach (CCPhysicsSprite sprite in ballsBatch.Children) {
            if (sprite.Visible && sprite.PhysicsBody.Position.x < 0f || sprite.PhysicsBody.Position.x * PTM_RATIO > ContentSize.Width) {
                world.DestroyBody (sprite.PhysicsBody);
                sprite.Visible = false;
                sprite.RemoveFromParent ();
            } else {
                sprite.UpdateBallTransform ();
            }
        }
    });

Running the game now demonstrates the game play.


### Implementing the Game Over Layer

In the GameLayer we added code to transition to the GameOverLayer's scene when the game ends. The GameOverLayer, will be very similar to the GameStartLayer, the main difference being it takes a parameter for the score to display in a label. Also, when the user touches the GameOverLayer, it starts a new game.

Add a new class named GameOverLayer with the following implementation:

    public class GameOverLayer : CCLayerColor
    {

        string scoreMessage = string.Empty;

        public GameOverLayer (int score)
        {

            var touchListener = new CCEventListenerTouchAllAtOnce ();
            touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene (GameLayer.GameScene (Window));

            AddEventListener (touchListener, this);

            scoreMessage = String.Format ("Game Over. You collected {0} bananas!", score);

            Color = new CCColor3B (CCColor4B.Black);

            Opacity = 255;
        }

        public void AddMonkey ()
        {
            var spriteSheet = new CCSpriteSheet ("animations/monkey.plist");
            var frame = spriteSheet.Frames.Find ((x) => x.TextureFilename.StartsWith ("frame"));
           
            var monkey = new CCSprite (frame) {
                Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X + 20, VisibleBoundsWorldspace.Size.Center.Y + 300),
                Scale = 0.5f
            };

            AddChild (monkey);
        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            var scoreLabel = new CCLabelTtf (scoreMessage, "arial", 22) {
                Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X, VisibleBoundsWorldspace.Size.Center.Y + 50),
                Color = new CCColor3B (CCColor4B.Yellow),
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
                Dimensions = ContentSize
            };

            AddChild (scoreLabel);

            var playAgainLabel = new CCLabelTtf ("Tap to Play Again", "arial", 22) {
                Position = VisibleBoundsWorldspace.Size.Center,
                Color = new CCColor3B (CCColor4B.Green),
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle,
                Dimensions = ContentSize
            };

            AddChild (playAgainLabel);

            AddMonkey ();
        }

        public static CCScene SceneWithScore (CCWindow mainWindow, int score)
        {
            var scene = new CCScene (mainWindow);
            var layer = new GameOverLayer (score);

            scene.AddChild (layer);

            return scene;
        }
    }

The final scene showing the GameOverLayer is shown below:

![GameOver](screenshots/GameOver.png?raw=true "Game Over")

You can now run the game and play it on either the simulator or a device. Congratulations, you created your first game using CocosSharp!

## Summary

In this walkthrough we created a game using CocosSharp. We created scenes with layers to display the game and manage game logic. In doing so, we covered how to add sprites and animate them with actions, handle touch, add sound effects and transition between scenes. We also added particle systems and parallax to include interesting visual effects, and physics simulation to add more realistic animation to the game.
