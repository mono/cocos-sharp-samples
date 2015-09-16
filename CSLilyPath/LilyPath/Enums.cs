using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyPath
{
    /// <summary>
    /// The type of arc in closed drawing or fill operations.
    /// </summary>
    public enum ArcType
    {
        /// <summary>
        /// Causes the endpoints of the arc to be connected directly.
        /// </summary>
        Segment,

        /// <summary>
        /// Causes the endpoints of the arc to be connected to the arc center, as in a pie wedge.
        /// </summary>
        Sector,
    }

    /// <summary>
    /// The type of Bezier curve specified in draw operations.
    /// </summary>
    public enum BezierType
    {
        /// <summary>
        /// Specifies a quadratic Bezier curve (1 control point).
        /// </summary>
        Quadratic,

        /// <summary>
        /// Specifies a cubic Bezier curve (2 control points).
        /// </summary>
        Cubic,
    }

    /// <summary>
    /// The style of termination used at the endpoints of stroked paths.
    /// </summary>
    public enum LineCap
    {
        /// <summary>
        /// The stroked path is cut off at the immediate edge of the path's endpoint.
        /// </summary>
        Flat,

        /// <summary>
        /// The stroked path runs half the pen's width past the edge of the path's endpoint.
        /// </summary>
        Square,

        /// <summary>
        /// The stroked path forms a triangular point half the pen's width past the edge of the path's endpoint.
        /// </summary>
        Triangle,

        /// <summary>
        /// The stroked path forms an inverse triangle half the pen's width past the edge of the path's endpoint.
        /// </summary>
        InvTriangle,

        /// <summary>
        /// The stroked path forms an arrow at the path's endpoint.
        /// </summary>
        Arrow,
    }

    /// <summary>
    /// Specifies how to join consecutive line segments in a path.
    /// </summary>
    public enum LineJoin
    {
        /// <summary>
        /// Specifies a mitered join.
        /// </summary>
        Miter,

        /// <summary>
        /// Specifies a beveled join.
        /// </summary>
        Bevel,

        //Round,
    }

    /// <summary>
    /// Whether a path is open or closed in draw operations.
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// The endpoints of the path should not be connected.
        /// </summary>
        Open,

        /// <summary>
        /// The endpoints of the path should be connected.
        /// </summary>
        Closed,
    }

    /// <summary>
    /// The alignment of a path stroked by a <see cref="Pen"/> relative to the ideal path.
    /// </summary>
    public enum PenAlignment
    {
        /// <summary>
        /// The stroked path should be centered directly over the ideal path.
        /// </summary>
        Center,

        /// <summary>
        /// The stroked path should run along the inside edge of the ideal path.
        /// </summary>
        Inset,

        /// <summary>
        /// The stroked path should run along the outside edge of the ideal path.
        /// </summary>
        Outset,
    }

    /// <summary>
    /// Specifies what components of a path should be stroked.
    /// </summary>
    public enum StrokeType
    {
        /// <summary>
        /// Only stroke the path itself.  Should not be confused with filling a shape enclosed by a path.
        /// </summary>
        Fill,

        /// <summary>
        /// Only stroke the outline of the path.
        /// </summary>
        Outline,

        /// <summary>
        /// Stroke both the path and its outline.
        /// </summary>
        Both,
    }

    /// <summary>
    /// Defines figure sort-rendering options.
    /// </summary>
    public enum DrawSortMode
    {
        /// <summary>
        /// Figures are not drawn until <see cref="DrawBatch.End"/> is called.  <see cref="DrawBatch.End"/> will apply graphics
        /// device settings and draw all figures in one batch in the same order drawing calls were received.  <see cref="DrawBatch"/>
        /// defaults to <c>Deferred</c> mode.
        /// </summary>
        Deferred = 0,

        /// <summary>
        /// <see cref="DrawBatch.Begin()"/> will apply new graphics device settings, and figures will be drawn within each drawing call.
        /// </summary>
        Immediate = 1,
    }
}
