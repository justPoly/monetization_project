using UnityEngine;

/* *****************************************************************************
 * File:    UnityTransformExtensions.cs
 * Author:  Philip Pierce - Monday, September 29, 2014
 * Description:
 *  Extensions for transforms and vector3
 *  
 * History:
 *  Monday, September 29, 2014 - Created
 * ****************************************************************************/

/// <summary>
/// Extensions for transforms and vector3
/// </summary>
public static class UnityTransformExtensions
{
    #region SetPositionX

    /// <summary>
    /// Sets the X position value
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newX"></param>
    public static void SetPositionX(this Transform t, float newX)
    {
        t.position = t.position.SetPositionX(newX);
    }

    /// <summary>
    /// Sets the X position value
    /// </summary>
    /// <param name="v3"></param>
    /// <param name="newX"></param>
    public static Vector3 SetPositionX(this Vector3 v3, float newX)
    {
        return new Vector3(newX, v3.y, v3.z);
    }

    // SetPositionX
    #endregion

    #region SetPositionY

    /// <summary>
    /// Sets the Y position value
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newY"></param>
    public static void SetPositionY(this Transform t, float newY)
    {
        t.position = t.position.SetPositionY(newY);
    }

    /// <summary>
    /// Sets the Y position value
    /// </summary>
    /// <param name="v3"></param>
    /// <param name="newY"></param>
    public static Vector3 SetPositionY(this Vector3 v3, float newY)
    {
        return new Vector3(v3.x, newY, v3.z);
    }

    // SetPositionY
    #endregion

    #region SetPositionZ

    /// <summary>
    /// Sets the Z position value
    /// </summary>
    /// <param name="t"></param>
    /// <param name="newZ"></param>
    public static void SetPositionZ(this Transform t, float newZ)
    {
        t.position = t.position.SetPositionZ(newZ);
    }

    /// <summary>
    /// Sets the Z position value
    /// </summary>
    /// <param name="v3"></param>
    /// <param name="newZ"></param>
    public static Vector3 SetPositionZ(this Vector3 v3, float newZ)
    {
        return new Vector3(v3.x, v3.y, newZ);
    }

    public static Vector3 AddPositionZ(this Vector3 v3, float newZ)
    {
        return new Vector3(v3.x, v3.y, v3.z + newZ);
    }

    // SetPositionZ
    #endregion

    #region GetPositionX

    /// <summary>
    /// Returns X of position
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetPositionX(this Transform t)
    {
        return t.position.x;
    }

    // GetPositionX
    #endregion

    #region GetPositionY

    /// <summary>
    /// Returns Y of position
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetPositionY(this Transform t)
    {
        return t.position.y;
    }

    // GetPositionY
    #endregion

    #region GetPositionZ

    /// <summary>
    /// Returns Z of position
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static float GetPositionZ(this Transform t)
    {
        return t.position.z;
    }

    // GetPositionZ
    #endregion


    /// <summary>
    ///     Calculus of the location of this object. Whether it is located at the top or bottom. -1 and 1 respectively.
    /// </summary>
    /// <returns></returns>
    public static int CloserEdge(this Transform transform, Camera camera, int width, int height)
    {
        //edge points according to the screen/camera
        var worldPointTop = camera.ScreenToWorldPoint(new Vector3(width / 2, height));
        var worldPointBot = camera.ScreenToWorldPoint(new Vector3(width / 2, 0));

        //distance from the pivot to the screen edge
        var deltaTop = Vector2.Distance(worldPointTop, transform.position);
        var deltaBottom = Vector2.Distance(worldPointBot, transform.position);

        return deltaBottom <= deltaTop ? 1 : -1;
    }
}