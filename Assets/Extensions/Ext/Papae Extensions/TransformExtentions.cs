using System.Collections.Generic;
using UnityEngine;

namespace Papae.UnitySDK.Extensions
{
    public static class TransformExtentions
    {
        /// <summary>
        /// Finds the nearest parent with component.
        /// </summary>
        /// <returns>The nearest parent with component.</returns>
        /// <param name="transform">Trans.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T FindNearestParentWithComponent<T>(this Transform transform) where T : Component
        {
            for (; transform != null; transform = transform.parent)
            {
                T component = transform.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        public static List<Transform> GetActiveChildren(this Transform parent)
        {
            List<Transform> list = new List<Transform>();
            foreach (Transform child in parent)
            {
                if (child.gameObject.activeSelf) list.Add(child);
            }
            return list;
        }

        /// <summary>
        /// Gets the depth level.
        /// </summary>
        /// <returns>The depth level.</returns>
        /// <param name="transform">Transform.</param>
        public static int GetDepthLevel(this Transform transform)
        {
            if (transform == null)
            {
                return 0;
            }

            return 1 + transform.parent.GetDepthLevel();
        }

        public static string GetHierarchyPath(this Transform transform)
        {
            if (transform == null)
            {
                return null;
            }

            Transform parentTransform = transform.parent;

            if (parentTransform == null)
            {
                return "/" + transform.name;
            }

            return parentTransform.GetHierarchyPath() + "/" + transform.name;
        }

        /// <summary>
        /// Gets the parents.
        /// </summary>
        /// <returns>The parents.</returns>
        /// <param name="transform">Transform.</param>
        public static IEnumerable<Transform> GetParents(this Transform transform)
        {
            if (transform == null)
            {
                yield break;
            }

            foreach (var parent in GetParentsAndSelf(transform.parent))
            {
                yield return parent;
            }
        }

        /// <summary>
        /// Gets the parents and self.
        /// </summary>
        /// <returns>The parents and self.</returns>
        /// <param name="transform">Transform.</param>
        public static IEnumerable<Transform> GetParentsAndSelf(this Transform transform)
        {
            if (transform == null)
            {
                yield break;
            }

            yield return transform;

            foreach (var parent in GetParentsAndSelf(transform.parent))
            {
                yield return parent;
            }
        }

        /// <summary>
        /// Rotates the transform so the forward vector points at target's current position.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="target">Target.</param>
        public static void LookAt2D(this Transform transform, Transform target)
        {
            transform.LookAt2D((Vector2)target.position);
        }

        /// <summary>
        /// Rotates the transform so the forward vector points at worldPosition.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="worldPosition">World position.</param>
        public static void LookAt2D(this Transform transform, Vector3 worldPosition)
        {
            transform.LookAt2D((Vector2)worldPosition);
        }

        /// <summary>
        /// Rotates the transform so the forward vector points at worldPosition.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="worldPosition">World position.</param>
        public static void LookAt2D(this Transform transform, Vector2 worldPosition)
        {
            Vector2 distance = worldPosition - (Vector2)transform.position;
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg);
        }

        /// <summary>
        /// Resets the transform.
        /// </summary>
        /// <param name="transform">Transform.</param>
        public static void LocalReset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void SetProgressBar(this Transform transform)
        {
            RectTransform rectTransform = null;
            if (transform != null)
            {
                rectTransform = transform as RectTransform;
            }
            if (rectTransform != null)
            {
                //barMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value * rectTransform.rect.width);
            }
        }

        public static void SetEulerAngles(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.eulerAngles = transform.eulerAngles.SetValues(x, y, z);
        }

        public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }

        public static void SetLocalEulerAngles(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localEulerAngles = transform.localEulerAngles.SetValues(x, y, z);
        }

        public static void SetLocalPosition(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localPosition = transform.localPosition.SetValues(x, y, z);
        }

        public static void SetLocalScale(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localScale = transform.localScale.SetValues(x, y, z);
        }

        public static void SetPosition(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.position = transform.position.SetValues(x, y, z);
        }

        #region RectTransform Extensions

        public static float GetHeight(this RectTransform rectTransform)
        {
            return rectTransform.rect.height;
        }

        public static Rect GetScreenRect(this RectTransform transform)
        {
            Vector3[] rtCorners = new Vector3[4];
            transform.GetWorldCorners(rtCorners);
            Rect rtRect = new Rect(new Vector2(rtCorners[0].x, rtCorners[0].y), new Vector2(rtCorners[3].x - rtCorners[0].x, rtCorners[1].y - rtCorners[0].y));

            Canvas canvas = transform.GetComponentInParent<Canvas>();
            Vector3[] canvasCorners = new Vector3[4];
            canvas.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);
            Rect cRect = new Rect(new Vector2(canvasCorners[0].x, canvasCorners[0].y), new Vector2(canvasCorners[3].x - canvasCorners[0].x, canvasCorners[1].y - canvasCorners[0].y));

            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            Vector2 size = new Vector2(screenWidth / cRect.size.x * rtRect.size.x, screenHeight / cRect.size.y * rtRect.size.y);
            Rect rect = new Rect(screenWidth * ((rtRect.x - cRect.x) / cRect.size.x), screenHeight * ((-cRect.y + rtRect.y) / cRect.size.y), size.x, size.y);
            return rect;
        }

        public static float GetWidth(this RectTransform rectTransform)
        {
            return rectTransform.rect.width;
        }


        public static void SetAnchor(this RectTransform rectTransform, float left, float top, float right, float bottom)
        {
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(-right, -top);
        }

        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }

        public static void SetPosAndSize(this RectTransform rectTransform, float posX, float posY, float width, float height)
        {
            rectTransform.anchoredPosition = new Vector2(posX, posY);
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        public static void SetPosX(this RectTransform rectTransform, float posX)
        {
            rectTransform.anchoredPosition = new Vector2(posX, rectTransform.anchoredPosition.y);
        }

        public static void SetPosY(this RectTransform rectTransform, float posY)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posY);
        }

        public static void SetPosXY(this RectTransform rectTransform, float posX, float posY)
        {
            rectTransform.anchoredPosition = new Vector2(posX, posY);
        }

        public static void SetSize(this RectTransform rectTransform, float width, float height)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }

        /// <summary>
        /// Sets the given RectTransform to stretch to the exact borders of its parent
        /// </summary>
        public static void StretchToFill(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMax = new Vector2(0, 0);
            rt.offsetMin = new Vector2(0, 0);
        }

        #region Left, Right, Top, Bottom

        public static void SetLeft(this RectTransform rectTransform, float left)
        {
            rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
        }

        public static void SetRight(this RectTransform rectTransform, float right)
        {
            rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
        }

        public static void SetTop(this RectTransform rectTransform, float top)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rectTransform, float bottom)
        {
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
        }

        public static void SetLeftTopRightBottom(this RectTransform rectTransform, float left, float top, float right, float bottom)
        {
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(-right, -top);
        }

        #endregion

        #endregion

        /*
		/// <summary>
		/// Set the layer on this and any of its children, grandchildren etc
		/// </summary>
		/// <param name="trans">Trans.</param>
		/// <param name="layerName">Layer name.</param>
		/// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
		public static void SetLayerOnAll(this Transform trans, string layerName, bool includeInactive)
		{
			foreach (Transform t in trans.GetComponentsInChildren(includeInactive))
			{
				t.gameObject.layer = LayerMask.NameToLayer(layerName);
			}
		}

		/// <summary>
		/// // Set the tag on this and any of its children, grandchildren etc
		/// </summary>
		/// <param name="trans">Trans.</param>
		/// <param name="tagName">Tag name.</param>
		/// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
		public static void SetTagOnAll(this Transform trans, string tagName, bool includeInactive) 
		{
			foreach (Transform t in trans.GetComponentsInChildren(includeInactive)) 
			{
				t.gameObject.tag = tagName;
			}
		}
        */
    }
}
