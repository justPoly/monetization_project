using UnityEngine;
using System.Collections;

namespace Papae.UnitySDK.Extensions
{
    public static class MaterialExtentions
    {
        public static void SetAlpha(this Material material, float value)
        {
            if (material.HasProperty("_Color"))
            {
                Color color = material.color;
                color.a = value;
                material.color = color;
            }
        }
    }
}
