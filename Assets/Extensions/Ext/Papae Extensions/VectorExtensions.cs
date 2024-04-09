using UnityEngine;

namespace Papae.UnitySDK.Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// A simple clone method for a Vector3
        /// </summary>
        /// <param name="self">The vector to clone</param>
        /// <returns></returns>
        public static Vector3 Clone(this Vector3 self)
        {
            return new Vector3(self.x, self.y, self.z);
        }

        /// <summary>
        /// A simple clone method for a Vector2
        /// </summary>
        /// <param name="self">The vector to clone</param>
        /// <returns></returns>
        public static Vector2 Clone(this Vector2 self)
        {
            return new Vector2(self.x, self.y);
        }

        public static float DistanceSquared(this Vector3 v1, Vector3 v2)
        {
            return (v2 - v1).sqrMagnitude;
        }

        public static Vector3 DirectionTo(this Vector3 v1, Vector3 v2)
        {
            var diff = v2 - v1;
            diff.Normalize();
            return diff;
        }

        public static Vector2 DivideBy(this Vector2 self, float number)
        {
            return new Vector2(self.x / number, self.y / number);
        }

        public static Vector3 Multiply(this Vector3 v1, Vector3 v2)
        {
            var result = v1;

            result.x *= v2.x;
            result.y *= v2.y;
            result.z *= v2.z;

            return result;
        }

        /// <summary>
        /// Flip all points over the center of the points.
        /// </summary>
        /// <param name="points">Points.</param>
        public static void FlipPointsX(Vector2[] points)
        {
            Vector2 min, max;
            GetBounds(points, out min, out max);
            Vector2 center = min + ((max - min) * .5f);

            for (int i = 0; i != points.Length; ++i)
            {
                points[i].x = (-(points[i].x - center.x)) + center.x;
            }
        }

        /// <summary>
        /// Flip all points over the center of the points.
        /// </summary>
        /// <param name="points">Points.</param>
        public static void FlipPointsY(Vector2[] points)
        {
            Vector2 min, max;
            GetBounds(points, out min, out max);
            Vector2 center = min + ((max - min) * .5f);

            for (int i = 0; i != points.Length; ++i)
            {
                points[i].y = (-(points[i].y - center.y)) + center.y;
            }
        }

        /// <summary>
        /// Get bounds of point cluster.
        /// </summary>
        /// <param name="points">Points.</param>
        /// <param name="min">Min value.</param>
        /// <param name="max">Max value.</param>
        public static void GetBounds(Vector2[] points, out Vector2 min, out Vector2 max)
        {
            min = points[0];
            max = points[0];
            for (int i = 0; i != points.Length; ++i)
            {
                var point = points[i];
                if (point.x < min.x) min.x = point.x;
                if (point.x > max.x) max.x = point.x;

                if (point.y < min.y) min.y = point.y;
                if (point.y > max.y) max.y = point.y;
            }
        }

        public static Vector2 Rotate(this Vector2 self, float angleInDeg)
        {
            float angleInRadians = Mathf.Deg2Rad * angleInDeg;
            float cosine = Mathf.Cos(angleInRadians);
            float sine = Mathf.Sin(angleInRadians);

            Vector2 vector2 = Vector2.zero;
            vector2.x = (self.x * cosine) - (self.y * sine);
            vector2.y = (self.x * sine) + (self.y * cosine);

            return vector2;
        }

        /// <summary>
        /// Get a ray intersect point.
        /// </summary>
        /// <param name="vector">Point.</param>
        /// <param name="rayOrigin">Ray Orgin.</param>
        /// <param name="rayDirection">Ray Direction.</param>
        /// <returns>Return value.</returns>
        public static Vector3 InersectRay(this Vector3 vector, Vector3 rayOrigin, Vector3 rayDirection)
        {
            return (rayDirection * Vector3.Dot(vector - rayOrigin, rayDirection)) + rayOrigin;
        }

        /// <summary>
        /// Get a ray intersect point.
        /// </summary>
        /// <param name="vector">Point.</param>
        /// <param name="rayOrigin">Ray Orgin.</param>
        /// <param name="rayDirection">Ray Direction.</param>
        /// <returns>Return value.</returns>
        public static Vector2 InersectRay(this Vector2 vector, Vector2 rayOrigin, Vector2 rayDirection)
        {
            return (rayDirection * Vector2.Dot(vector - rayOrigin, rayDirection)) + rayOrigin;
        }

        /// <summary>
        /// Rotate vectors around axis.
        /// </summary>
        /// <param name="vectors">Vectors.</param>
        /// <param name="angle">Rotation Angle.</param>
        public static void RotateVectors(Vector2[] vectors, float angle)
        {
            var rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, new Vector3(0, 0, 1));
            for (int i = 0; i != vectors.Length; ++i)
            {
                vectors[i] = rotation * vectors[i];
            }
        }

        public static Vector3 SetValues(this Vector3 self,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            if (!float.IsNaN(x))
                self.x = x;

            if (!float.IsNaN(y))
                self.y = y;

            if (!float.IsNaN(z))
                self.z = z;

            return self;
        }

        public static Vector2 SetValues(this Vector2 self,
            float x = float.NaN, float y = float.NaN)
        {
            if (!float.IsNaN(x))
                self.x = x;

            if (!float.IsNaN(y))
                self.y = y;

            return self;
        }

        public static Vector3 SetX(this Vector3 self, float value)
        {
            return new Vector3(value, self.y, self.z);
        }

        public static Vector2 SetX(this Vector2 self, float value)
        {
            return new Vector2(value, self.y);
        }

        public static Vector3 SetXY(this Vector3 self, float xValue, float yValue)
        {
            return new Vector3(xValue, yValue, self.z);
        }

        public static Vector3 SetXZ(this Vector3 self, float xValue, float zValue)
        {
            return new Vector3(xValue, self.y, zValue);
        }

        public static Vector3 SetY(this Vector3 self, float value)
        {
            return new Vector3(self.x, value, self.z);
        }

        public static Vector2 SetY(this Vector2 self, float value)
        {
            return new Vector2(self.x, value);
        }

        public static Vector3 SetYZ(this Vector3 self, float yValue, float zValue)
        {
            return new Vector3(self.x, yValue, zValue);
        }

        public static Vector3 SetZ(this Vector3 self, float value)
        {
            return new Vector3(self.x, self.y, value);
        }

        public static Vector2 XY(this Vector3 self)
        {
            return new Vector2(self.x, self.y);
        }

        public static Vector2 XZ(this Vector3 self)
        {
            return new Vector2(self.x, self.z);
        }

        public static Vector2 YZ(this Vector3 self)
        {
            return new Vector2(self.y, self.z);
        }

        /// <summary>
        /// Convert to Vector3.
        /// </summary>
        /// <param name="self">Local value.</param>
        /// <returns>Return value.</returns>
        public static Vector3 ToVector3(this Vector2 self)
        {
            Vector3 vector3;
            vector3.x = self.x;
            vector3.y = self.y;
            vector3.z = 0;
            return vector3;
        }

        /// <summary>
        /// Convert to Vector3.
        /// </summary>
        /// <param name="self">Local value.</param>
        /// <param name="z">Z value.</param>
        /// <returns>Return value.</returns>
        public static Vector3 ToVector3(this Vector2 self, float z)
        {
            Vector3 vector3;
            vector3.x = self.x;
            vector3.y = self.y;
            vector3.z = z;
            return vector3;
        }

        /// <summary>
        /// Convert to Vector2
        /// </summary>
        /// <param name="self">Local value.</param>
        /// <returns>Return value.</returns>
        public static Vector2 ToVector2(this Vector3 self)
        {
            Vector2 vector2;
            vector2.x = self.x;
            vector2.y = self.y;
            return vector2;
        }
    }
}

