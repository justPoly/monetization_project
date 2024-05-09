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
