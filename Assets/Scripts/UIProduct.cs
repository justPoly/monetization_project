using System;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UIProduct : MonoBehaviour
{
   [SerializeField]
   private TextMeshProUGUI nameText;
   [SerializeField]
   private TextMeshProUGUI descriptionText;
   [SerializeField]
   private Image Icon;
   [SerializeField]
   private TextMeshProUGUI priceText;
   [SerializeField]
   private Button PurchaseButton;

   public delegate void PurchaseEvent(Product Model, Action OnComplete);
   public event PurchaseEvent OnPurchase;

   private Product Model;

   public void Setup(Product Product)
   {
        Model = Product;
        nameText.SetText(Product.metadata.localizedTitle);
        descriptionText.SetText(Product.metadata.localizedDescription);
        priceText.SetText($"{Product.metadata.localizedPriceString} " +
          $"{Product.metadata.isoCurrencyCode}");
        Texture2D texture = StoreIconProvider.GetIcon(Product.definition.id);

        if(texture != null)
        {
            Sprite sprite = Sprite.Create(texture, 
            new Rect(0, 0, texture.width, texture.height),
            Vector2.one / 2f
            );

            Icon.sprite = sprite;
        }
        else 
        {
            Debug.LogError($"No Sprite found for {Product.definition.id}!");
        }
   }

   public void Purchase()
   {
        PurchaseButton.enabled = false;
        OnPurchase?.Invoke(Model, HandlePurchaseComplete);
   }

   private void HandlePurchaseComplete()
   {
        PurchaseButton.enabled = true;
   }
}
