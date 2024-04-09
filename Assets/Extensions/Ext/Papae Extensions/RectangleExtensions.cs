using UnityEngine;

namespace Papae.UnitySDK.Extensions
{
    public static class RectangleExtensions
    {
        
        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side and then checks if a given point is inside the resulting rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>True if the position is inside the extended rect.</returns>
        public static bool Contains(this Rect rect, Vector2 position, float extendDistance)
        {
            return (position.x > rect.xMin + extendDistance) &&
                   (position.y > rect.yMin + extendDistance) &&
                   (position.x < rect.xMax - extendDistance) &&
                   (position.y < rect.yMax - extendDistance);
        }

        /// <summary>
        /// Extends/shrinks the rect by extendDistance to each side.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The rect, extended/shrunken by extendDistance to each side.</returns>
        public static Rect Extend(this Rect rect, float extendDistance)
        {
            var copy = rect;
            copy.xMin -= extendDistance;
            copy.xMax += extendDistance;
            copy.yMin -= extendDistance;
            copy.yMax += extendDistance;
            return copy;
        }

        /// <summary>
        /// Creates an array containing the four corner points of a Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>An array containing the four corner points of the Rect.</returns>
        public static Vector2[] GetCornerPoints(this Rect rect)
        {
            return new[]
                       {
                           new Vector2(rect.xMin, rect.yMin),
                           new Vector2(rect.xMax, rect.yMin),
                           new Vector2(rect.xMax, rect.yMax),
                           new Vector2(rect.xMin, rect.yMax)
                       };
        }

        public static Rect SetX(this Rect rect, float value)
        {
            return new Rect(value, rect.y, rect.width, rect.height);
        }

        public static Rect SetY(this Rect rect, float value)
        {
            return new Rect(rect.x, value, rect.width, rect.height);
        }

        public static Rect SetOrOverrideWidth(this Rect rect, float value)
        {
            return new Rect(rect.x, rect.y, value, rect.height);
        }

        public static Rect SetHeight(this Rect rect, float value)
        {
            return new Rect(rect.x, rect.y, rect.width, value);
        }

        public static Rect SetPosition(this Rect rect, float x, float y)
        {
            return new Rect(x, y, rect.width, rect.height);
        }

        public static Rect SetSize(this Rect rect, float width, float height)
        {
            return new Rect(rect.x, rect.y, width, height);
        }
    }
}