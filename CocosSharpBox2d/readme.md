Project showing how to create a box2d debugging module using CCDrawNode.  This replaces any examples that uses the obsolete CCDrawPrimitives.

* Module 'Box2DDebug.cs' draws the debugging output using a 'CCDrawNode' as it's rendering node.
* Module 'CCPhysicsSprite.cs' is a subclass of 'CCSprite' that implements UpdateBodyTransform() to be used as a backing sprite.

'UpdateBodyTransform()' is called from the world updating method 'Run()'.
