using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class StoreIconProvider
{
    public static void Initialize(ProductCollection Products)
    {
        // No changes needed in this method since icon loading is removed.
    }

    public static Texture2D GetIcon(string Id)
    {
        throw new InvalidOperationException("StoreIconProvider.GetIcon() should not be called.");
    }
}
