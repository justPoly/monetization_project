using System;
using UnityEngine;

namespace Papae.UnitySDK.Extensions
{
    #region HSB Struct
    [Serializable]
    public struct ColorHSB
    {
        public float h;
        public float s;
        public float b;
        public float a;

        public ColorHSB(float h, float s, float b, float a)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = a;
        }

        public ColorHSB(float h, float s, float b)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = 1f;
        }

        public ColorHSB(Color col)
        {
            ColorHSB temp = FromColor(col);
            h = temp.h;
            s = temp.s;
            b = temp.b;
            a = temp.a;
        }

        public Color ToColor()
        {
            return ToColor(this);
        }

        public override string ToString()
        {
            return "H:" + h + " S:" + s + " B:" + b;
        }

        public static ColorHSB FromColor(Color color)
        {
            ColorHSB ret = new ColorHSB(0f, 0f, 0f, color.a);

            float r = color.r;
            float g = color.g;
            float b = color.b;

            float max = Mathf.Max(r, Mathf.Max(g, b));

            if (max <= 0)
            {
                return ret;
            }

            float min = Mathf.Min(r, Mathf.Min(g, b));
            float dif = max - min;

            if (max > min)
            {
                if (g.FloatEquals(max))
                {
                    ret.h = (b - r) / dif * 60f + 120f;
                }
                else if (b.FloatEquals(max))
                {
                    ret.h = (r - g) / dif * 60f + 240f;
                }
                else if (b > g)
                {
                    ret.h = (g - b) / dif * 60f + 360f;
                }
                else
                {
                    ret.h = (g - b) / dif * 60f;
                }
                if (ret.h < 0)
                {
                    ret.h = ret.h + 360f;
                }
            }
            else
            {
                ret.h = 0;
            }

            ret.h *= 1f / 360f;
            ret.s = (dif / max) * 1f;
            ret.b = max;

            return ret;
        }

        public static Color ToColor(ColorHSB hsbColor)
        {
            float r = hsbColor.b;
            float g = hsbColor.b;
            float b = hsbColor.b;
            if (hsbColor.s.FloatEquals(0))
            {
                float max = hsbColor.b;
                float dif = hsbColor.b * hsbColor.s;
                float min = hsbColor.b - dif;

                float h = hsbColor.h * 360f;

                if (h < 60f)
                {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                }
                else if (h < 120f)
                {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (h < 300f)
                {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
        }

        public static ColorHSB Lerp(ColorHSB a, ColorHSB b, float t)
        {
            float h, s;

            //check special case black (color.b==0): interpolate neither hue nor saturation!
            //check special case grey (color.s==0): don't interpolate hue!
            if (a.b.FloatEquals(0))
            {
                h = b.h;
                s = b.s;
            }
            else if (b.b.FloatEquals(0))
            {
                h = a.h;
                s = a.s;
            }
            else
            {
                if (a.s.FloatEquals(0))
                {
                    h = b.h;
                }
                else if (b.s.FloatEquals(0))
                {
                    h = a.h;
                }
                else
                {
                    // works around bug with LerpAngle
                    float angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);
                    while (angle < 0f)
                        angle += 360f;
                    while (angle > 360f)
                        angle -= 360f;
                    h = angle / 360f;
                }
                s = Mathf.Lerp(a.s, b.s, t);
            }
            return new ColorHSB(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
        }

        public static void Test()
        {
            ColorHSB color;

            color = new ColorHSB(Color.red);
            Debug.Log("red: " + color);

            color = new ColorHSB(Color.green);
            Debug.Log("green: " + color);

            color = new ColorHSB(Color.blue);
            Debug.Log("blue: " + color);

            color = new ColorHSB(Color.grey);
            Debug.Log("grey: " + color);

            color = new ColorHSB(Color.white);
            Debug.Log("white: " + color);

            color = new ColorHSB(new Color(0.4f, 1f, 0.84f, 1f));
            Debug.Log("0.4, 1f, 0.84: " + color);

            Debug.Log("164,82,84   .... 0.643137f, 0.321568f, 0.329411f  :" + ToColor(new ColorHSB(new Color(0.643137f, 0.321568f, 0.329411f))));
        }
    }
    #endregion

    public static class ColorExtensions
    {
        /// <summary>
        /// Brightness offset with 1 is brightest and -1 is darkest
        /// </summary>
        public static Color BrightnessOffset(this Color color, float offset)
        {
            return new Color(
                color.r + offset,
                color.g + offset,
                color.b + offset,
                color.a);
        }

        /// <summary>
        /// Clamp the specified c.
        /// </summary>
        /// <returns>The clamp.</returns>
        /// <param name="c">C.</param>
        public static Color Clamp(this Color c)
        {
            for (int i = 0; i <= 3; i++)
            {
                if (float.IsNaN(c[i]) ||
                    float.IsNegativeInfinity(c[i]))
                    c[i] = 0;
                else if (float.IsPositiveInfinity(c[i]))
                    c[i] = 1;
                else
                    c[i] = Mathf.Clamp(c[i], 0, 1);
            }

            return c;
        }

        /// <summary>
        /// Creates the pixel.
        /// </summary>
        /// <returns>The pixel.</returns>
        /// <param name="c">C.</param>
        public static Texture2D CreatePixel(this Color c)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, c);
            tex.Apply();
            return tex;
        }

        private const float DarkerFactor = 0.9f;
        /// <summary>
        /// Returns a color darker than the given color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Darker(this Color color)
        {
            return new Color(
                color.r - LightOffset,
                color.g - LightOffset,
                color.b - LightOffset,
                color.a);
        }


        private const float LightOffset = 0.0625f;
        /// <summary>
        /// Returns a color lighter than the given color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Lighter(this Color color)
        {
            return new Color(
                color.r + LightOffset,
                color.g + LightOffset,
                color.b + LightOffset,
                color.a);
        }


        /// <summary>
        /// Changes the alpha property of this color and returns it
        /// </summary>
        /// <param name="c"></param>
        /// <param name="newAlpha"></param>
        /// <returns>The modified color</returns>
        public static Color SetAlpha(this Color c, float newAlpha)
        {
            return new Color(c.r, c.g, c.b, newAlpha);
        }

        /// <summary>
        /// Changes the alpha of this color and returns it
        /// </summary>
        /// <param name="c"></param>
        /// <param name="newAlpha"></param>
        /// <returns>The modified color</returns>
        public static Color SetAlpha(this Color c, byte newAlpha)
        {
            Color32 color = c;
            return new Color32(color.r, color.g, color.b, newAlpha);
        }

        /// <summary>
        /// Changes the blue property of this color and returns it
        /// </summary>
        /// <returns>The blue.</returns>
        /// <param name="c">C.</param>
        /// <param name="newBlue">New blue.</param>
        public static Color SetBlue(this Color c, float newBlue)
        {
            return new Color(c.r, c.g, newBlue, c.a);
        }

        /// <summary>
        /// Changes the blue property of this color and returns it
        /// </summary>
        /// <returns>The modified color.</returns>
        /// <param name="c">C.</param>
        /// <param name="newBlue">New blue.</param>
        public static Color SetBlue(this Color c, byte newBlue)
        {
            Color32 color = c;
            return new Color32(color.r, color.g, newBlue, color.a);
        }

        /// <summary>
        /// Changes the green property of this color and returns it
        /// </summary>
        /// <returns>The green.</returns>
        /// <param name="c">C.</param>
        /// <param name="newGreen">New green.</param>
        public static Color SetGreen(this Color c, float newGreen)
        {
            return new Color(c.r, newGreen, c.b, c.a);
        }

        /// <summary>
        /// Changes the green property of this color and returns it
        /// </summary>
        /// <returns>The green.</returns>
        /// <param name="c">C.</param>
        /// <param name="newGreen">New green.</param>
        public static Color SetGreen(this Color c, byte newGreen)
        {
            Color32 color = c;
            return new Color32(color.r, newGreen, color.b, color.a);
        }

        /// <summary>
        /// Changes the red property of this color and returns it
        /// </summary>
        /// <returns>The red.</returns>
        /// <param name="c">C.</param>
        /// <param name="newRed">New red.</param>
        public static Color SetRed(this Color c, float newRed)
        {
            return new Color(newRed, c.g, c.b, c.a);
        }

        /// <summary>
        /// Changes the red property of this color and returns it
        /// </summary>
        /// <returns>The red.</returns>
        /// <param name="c">C.</param>
        /// <param name="newRed">New red.</param>
        public static Color SetRed(this Color c, byte newRed)
        {
            Color32 color = c;
            return new Color32(newRed, color.g, color.b, color.a);
        }

        /// <summary>
        /// Converts this color to its 32 bit equivalent
        /// </summary>
        /// <param name="c">The uint to convert</param>
        /// <returns></returns>
        public static Color32 ToColor32(this Color c)
        {
            byte newRed = (byte)(c.r * 255);
            byte newGreen = (byte)(c.g * 255);
            byte newBlue = (byte)(c.b * 255);
            byte newAlpa = (byte)(c.a * 255);

            return new Color32(newRed, newGreen, newBlue, newAlpa);
        }

        public static ColorHSB ToColorHSB(this Color c)
        {
            ColorHSB ret = new ColorHSB(0f, 0f, 0f, c.a);

            float r = c.r;
            float g = c.g;
            float b = c.b;

            float max = Mathf.Max(r, Mathf.Max(g, b));

            if (max <= 0)
            {
                return ret;
            }

            float min = Mathf.Min(r, Mathf.Min(g, b));
            float dif = max - min;

            if (max > min)
            {
                if (g.FloatEquals(max))
                {
                    ret.h = (b - r) / dif * 60f + 120f;
                }
                else if (b.FloatEquals(max))
                {
                    ret.h = (r - g) / dif * 60f + 240f;
                }
                else if (b > g)
                {
                    ret.h = (g - b) / dif * 60f + 360f;
                }
                else
                {
                    ret.h = (g - b) / dif * 60f;
                }
                if (ret.h < 0)
                {
                    ret.h = ret.h + 360f;
                }
            }
            else
            {
                ret.h = 0;
            }

            ret.h *= 1f / 360f;
            ret.s = (dif / max) * 1f;
            ret.b = max;

            return ret;
        }

        /// <summary>
        /// Converts a color to its hex counterpart 
        /// </summary>
        /// <returns>Returns a HEX version of the given Unity Color, with the # pre-cursor.</returns>
        /// <param name="c">The color to convert.</param>
        /// <param name="includeAlpha">If set to <c>true</c>, also converts the alpha value and returns a hex of 8 characters include alpha,
        /// otherwise doesn't and returns a hex of 6 characters</param>
        public static string ToHex(this Color c, bool includeAlpha = false)
        {
            Color32 color = c.ToColor32();
            string hexAlpha = color.a.ToString("X2");
            string hexColor = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return includeAlpha ? hexColor + hexAlpha : hexColor;
        }

        /// <summary>
        /// Wraps the supplied string in rich text color tags.
        /// </summary>
        /// <param name="color">The color to make the text.</param>
        /// <param name="text">The text to be colored.</param>
        /// <returns><paramref name="text"/> wrapped in color tags.</returns>
        public static string ToRichTextColorString(this Color color, string text)
        {
            return string.Format("<color=#{0}>{1}</color>", color.ToHex(), text);
        }

        public static string HexString(Color c)
        {
            return HexString((Color32)c, false);
        }

        public static string HexString(Color c, bool includeAlpha)
        {
            return HexString((Color32)c, includeAlpha);
        }

        /// <summary>
        /// Converts the given color to its equivalent hex color in the form of #RRGGBBAA (eg. #000000FF for opaque black).
        /// </summary>
        /// <returns>The hex representation of <paramref name="color"/>.</returns>
        /// <param name="aColor">A color.</param>
        /// <param name="includeAlpha">If set to <c>true</c> include alpha.</param>
        public static string HexString(Color32 aColor, bool includeAlpha)
        {
            String rs = Convert.ToString(aColor.r, 16).ToUpper();
            String gs = Convert.ToString(aColor.g, 16).ToUpper();
            String bs = Convert.ToString(aColor.b, 16).ToUpper();
            String a_s = Convert.ToString(aColor.a, 16).ToUpper();

            while (rs.Length < 2) rs = "0" + rs;
            while (gs.Length < 2) gs = "0" + gs;
            while (bs.Length < 2) bs = "0" + bs;
            while (a_s.Length < 2) a_s = "0" + a_s;
            if (includeAlpha) return "#" + rs + gs + bs + a_s;
            return "#" + rs + gs + bs;
        }

        /// <summary>
        /// Converts a color to an uint
        /// </summary>
        /// <param name="c">The uint to convert</param>
        /// <returns></returns>
        public static uint ToUInt(this Color c)
        {
            Color32 color32 = c;
            return (uint)((color32.a << 24) | (color32.r << 16) |
                          (color32.g << 8) | (color32.b << 0));
        }

        /// <summary>
        /// Return this color but with 0 alpha
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ZeroAlpha(this Color color)
        {
            color.a = 0;
            return color;
        }
    }
}
