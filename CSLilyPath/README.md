Purpose
=======
This sample shows how to use CCGeometryNode as a base to create your own geometry routines.  

Port of the excellent LilyPath library by Justin Aquadro to CocosSharp.  DrawBatch extends CCGeometryNode.

** Note ** This was a quick port, changing as little as possible of the Geometry generation code, so is not as efficient as it could be by implementing the Geometry Insances of the CCGeometryNode. 

Port Features
=============
* This sample shows how to use CCGeometryNode as a base to create your own geometry routines.  DrawBatch extends CCGeometryNode.
* Both primitives created by TriangleList and LineList are supported.  v1.6.0 only.
* Texture mapping using TextureBrush with the use of [Triangulator](https://github.com/mono/cocos-sharp-samples/blob/master/CSLilyPath/CSLilyPath/LilyPath/Triangulator.cs) code to map CCTexture2D instances to vertices and indices.
* Ported library as a PCL library.
* SamplerStates of CCTexture2D instances used in mapping a texture to generated geometry can be used to create some interesting effects.  See example demo [TextureFill](https://github.com/mono/cocos-sharp-samples/blob/master/CSLilyPath/CSLilyPath/CSLilyPath/Demos/TextureFill.cs). for more.
```csharp

            var pattern = new CCTexture2D("images/pattern1");
            
            // Create a mirror effect of the texture when used to fill a geometry using the mirrorBrush
            var state = new SamplerState();
            state.AddressU = TextureAddressMode.Mirror;
            state.AddressV = TextureAddressMode.Wrap;
            pattern.SamplerState = state;

            mirrorBrush = new TextureBrush(pattern)
            {
                Color = Microsoft.Xna.Framework.Color.White
            };

```

LilyPath
========
<img src="https://raw.github.com/wiki/jaquadro/LilyPath/images/lilypath.png" align="right" title="Rendered with MSAA on XNA with LilyPath" />

LilyPath is a 2D drawing library for MonoGame and XNA.  LilyPath provides some of the functionality found in `System.Drawing`, such as drawing paths and shapes with configurable pens and brushes.

Instead of creating raster images, LilyPath renders everything directly to your scene or render target.  Complex paths and filled shapes are rendered as polygons, while primitives are rendered as GL or DX lines.

Drawing is handled through a `DrawBatch` object to reduce the number of draw calls needed.  This mirrors the role of `SpriteBatch` for rendering textured quads.  More complex geometry can be compiled ahead of time into `GraphicsPath` objects, which contain the polygon data after arcs, joins, and other calculations have been completed.

Features
--------
* Draw primitive lines, paths, and closed shapes.
* Draw complex lines, paths, and shapes with pens.
* Fill paths and shapes with brushes.
* Basic paths and shapes supported:
  * Arc, Bezier, Circle, Ellipse, Line, Path, Point, Rectangle, Quad
* Pen features supported:
  * Alignment, Color, End Styles, Gradient, Join Styles (Mitering), Width
* Brush features supported:
  * Color, Texture, Transform

Example
-------
Here’s a short code sample for drawing the lily pad in the picture above (without the flower):

```csharp

Vector2 origin = new Vector2(200, 200);
float startAngle = (float)(Math.PI / 16) * 25; // 11:20
float arcLength = (float)(Math.PI / 16) * 30;

drawBatch.FillCircle(new SolidColorBrush(Color.SkyBlue), origin, 175);
drawBatch.FillArc(new SolidColorBrush(Color.LimeGreen), origin, 150, 
    startAngle, arcLength, ArcType.Sector);
drawBatch.DrawClosedArc(new Pen(Color.Green, 15), origin, 150, 
    startAngle, arcLength, ArcType.Sector);

```

Source code for the full image and other examples can be found in the included test project, [LilyPathDemo](https://github.com/jaquadro/LilyPath/tree/master/LilyPathDemo).

