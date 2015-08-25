using System;
using System.Collections.Generic;
using CocosSharp;
using CocosSharpBox2d.Shared;
using Box2D.Common;
using Box2D.Collision.Shapes;
using Box2D.Dynamics;

namespace CocosSharpBox2d
{
    public class IntroLayer : CCLayerColor
    {

        private b2World world;

        private List<CCPhysicsSprite> sprites = new List<CCPhysicsSprite>();

        public static readonly int PTM_RATIO = 32;

        Box2DDebug debugDraw;
        CCCustomCommand renderDebugCommand;

        public IntroLayer()
            : base(CCColor4B.Blue)
        {

            renderDebugCommand = new CCCustomCommand(RenderDebug);

            InitPhysics();

            StartDebugging();

            Schedule(Run);
        }

        void InitPhysics()
        {
            var gravity = new b2Vec2(0.0f, -10.0f);
            world = new b2World(gravity);

            world.SetAllowSleeping(true);
            world.SetContinuousPhysics(true);

            var def = new b2BodyDef();
            def.allowSleep = true;
            def.position = b2Vec2.Zero;
            def.type = b2BodyType.b2_staticBody;

            b2Body groundBody = world.CreateBody(def);
            groundBody.SetActive(true);

            b2EdgeShape groundBox = new b2EdgeShape();
            groundBox.Set(b2Vec2.Zero, new b2Vec2(900, 100));

            b2FixtureDef fd = new b2FixtureDef();
            fd.friction = 0.3f;
            fd.restitution = 0.1f;
            fd.shape = groundBox;

            groundBody.CreateFixture(fd);
        }

        private void Run(float time)
        {
            world.Step(time, 8, 8);

            foreach (var sprite in sprites)
            {
                sprite.UpdateBodyTransform();
            }
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            var bounds = VisibleBoundsWorldspace;

            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);

        }

        void StartDebugging()
        {
            var debugNode = new CCDrawNode();
            AddChild(debugNode, 1000);
            debugDraw = new Box2DDebug(debugNode, PTM_RATIO);

            debugDraw.Flags = b2DrawFlags.e_shapeBit | b2DrawFlags.e_aabbBit | b2DrawFlags.e_centerOfMassBit | b2DrawFlags.e_jointBit;
            world.SetDebugDraw(debugDraw);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                AddShape(world, touches[0].Location);
            }
        }

        protected override void VisitRenderer(ref CCAffineTransform worldTransform)
        {
            base.VisitRenderer(ref worldTransform);

            if (debugDraw != null && debugDraw.Flags != 0)
                Renderer.AddCommand(renderDebugCommand);
        }

        void RenderDebug()
        {
            if (debugDraw != null)
            {
                debugDraw.Begin();
                world.DrawDebugData();
                debugDraw.End();
            }
        }

        private void AddShape(b2World world, CCPoint position)
        {
            b2Vec2 positionVec = new b2Vec2(position.X, position.Y);

            var box = new CCPhysicsSprite("hd/images/cloud", IntroLayer.PTM_RATIO);
            box.Position = position;

            var def = new b2BodyDef();
            def.position = new b2Vec2(positionVec.x / IntroLayer.PTM_RATIO, positionVec.y / IntroLayer.PTM_RATIO);
            def.linearVelocity = new b2Vec2(0.0f, -1.0f);
            def.type = b2BodyType.b2_dynamicBody;
            b2Body body = world.CreateBody(def);

            // Polygon Shape
            //var shape = new b2PolygonShape();
            //shape.SetAsBox(50f / IntroLayer.PTM_RATIO, 50f / IntroLayer.PTM_RATIO);

            // Circle Shape
            var shape = new b2CircleShape();
            shape.Radius = 50f / IntroLayer.PTM_RATIO;

            var fd = new b2FixtureDef();
            fd.shape = shape;
            fd.density = 1f;
            fd.restitution = 0f;
            fd.friction = 0.2f;
            body.CreateFixture(fd);

            box.PhysicsBody = body;

            sprites.Add(box);
            AddChild(box);
        }
    }
}

