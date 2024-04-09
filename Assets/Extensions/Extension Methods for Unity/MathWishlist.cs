using UnityEngine;

public static class MathWishlist
{

    // Mathf
    public const float TAU = 6.28318530717959f;
    public static float Frac(float x) => x - Mathf.Floor(x);
    public static float Smooth01(float x) => x * x * (3 - 2 * x);
    public static float InverseLerpUnclamped(float a, float b, float value) => (value - a) / (b - a);
    public static float Remap(this float value, float iMin, float iMax, float oMin, float oMax)
    {
        float t = Mathf.InverseLerp(iMin, iMax, value);
        return Mathf.Lerp(oMin, oMax, t);
    }
    public static float RemapUnclamped(float iMin, float iMax, float oMin, float oMax, float value)
    {
        float t = InverseLerpUnclamped(iMin, iMax, value);
        return Mathf.LerpUnclamped(oMin, oMax, t);
    }

    // Vector2
    public static Vector2 AngToDir(float aRad) => new Vector2(Mathf.Cos(aRad), Mathf.Sin(aRad));
    public static float DirToAng(Vector2 dir) => Mathf.Atan2(dir.y, dir.x);
    public static float Determinant/*or Cross*/(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x; // 2D "cross product"
    public static Vector2 Rotate90CW(Vector2 v) => new Vector2(v.y, -v.x);
    public static Vector2 Rotate90CCW(Vector2 v) => new Vector2(-v.y, v.x);
    public static Vector2 Rotate(Vector2 v, float angRad)
    {
        float ca = Mathf.Cos(angRad);
        float sa = Mathf.Sin(angRad);
        return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
    }
    public static Vector2 RotateAround(Vector2 v, Vector2 pivot, float angRad) => Rotate(v - pivot, angRad) + pivot;

    // Vector2/3/4
    public static Vector2 Abs(Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
    public static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    public static Vector4 Abs(Vector4 v) => new Vector4(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z), Mathf.Abs(v.w));
    public static Vector2 Round(Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    public static Vector3 Round(Vector3 v) => new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    public static Vector4 Round(Vector4 v) => new Vector4(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z), Mathf.Round(v.w));
    public static Vector2 Floor(Vector2 v) => new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
    public static Vector3 Floor(Vector3 v) => new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
    public static Vector4 Floor(Vector4 v) => new Vector4(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z), Mathf.Floor(v.w));
    public static Vector2 Ceil(Vector2 v) => new Vector2(Mathf.Ceil(v.x), Mathf.Ceil(v.y));
    public static Vector3 Ceil(Vector3 v) => new Vector3(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z));
    public static Vector4 Ceil(Vector4 v) => new Vector4(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z), Mathf.Ceil(v.w));

}