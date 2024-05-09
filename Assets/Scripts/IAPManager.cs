using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IStoreListener, IDetailedStoreListener
{
    [SerializeField]
    private List<UIProduct> uiProducts;
    [SerializeField]
    private GameObject LoadingOverlay;
    [SerializeField]
    private GameObject purchaseFailed;
    [SerializeField]
    private bool UseFakeStore = false;
    [SerializeField]
    public TMP_Text coinText;

    private Action OnPurchaseCompleted;
    private IStoreController StoreController;
    private IExtensionProvider ExtensionProvider;

    private async void Awake()
    {
        InitializationOptions options = new InitializationOptions()
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        .SetEnvironmentName("test");
        #else
        .SetEnvironmentName("production");
        #endif
        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
        UpdateUI();
    }

    public void UpdateUI()
    {
        coinText.text = $" {((int)GameStateManager.EconomyManager.Money).ToString()}";
    }

    private void HandleIAPCatalogLoaded(AsyncOperation Operation)
    {
        ResourceRequest request = Operation as ResourceRequest;
        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

        #if UNITY_EDITOR 
        UseFakeStore = true;
        if (UseFakeStore) // Use bool in editor to control fake store behavior.
        {
            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
            StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        }
        #endif

        #if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay)
        );
        #elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
        #else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.NotSpecified)
        );
        #endif

        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            builder.AddProduct(item.id, item.type);
        }
        Debug.Log($"Initializing Unity IAP with {builder.products.Count} products");
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        StoreController = controller;
        ExtensionProvider = extensions;
        CreateUI();
    }

    private void CreateUI()
    {
        List<Product> sortedProducts = StoreController.products.all
            .OrderBy(item => item.metadata.localizedPrice)
            .ToList();

        for (int i = 0; i < uiProducts.Count; i++)
        {
            if (i < sortedProducts.Count)
            {
                uiProducts[i].OnPurchase += HandlePurchase;
                uiProducts[i].Setup(sortedProducts[i]);
            }
            else
            {
                Debug.LogWarning("Not enough products to fill all UIProduct instances.");
                break;
            }
        }
    }

    private void HandlePurchase(Product Product, Action OnPurchaseCompleted)
    {
        LoadingOverlay.SetActive(true);
        this.OnPurchaseCompleted = OnPurchaseCompleted;
        StoreController.InitiatePurchase(Product);
    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Error initializing IAP because of {error}." +
            $"\r\nShow a message to the player depending on the error.");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"Error initializing IAP because of {error}." +
        $"\r\nShow a message to the player depending on the error.");
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Failed to purchase {product.definition.id} because {failureReason}");
        OnPurchaseCompleted?.Invoke();
        OnPurchaseCompleted = null;
        LoadingOverlay.SetActive(true);
        purchaseFailed.SetActive(true);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Failed to purchase {product.definition.id} because {failureDescription}");
        OnPurchaseCompleted?.Invoke();
        OnPurchaseCompleted = null;
        LoadingOverlay.SetActive(true);
        purchaseFailed.SetActive(true);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
        OnPurchaseCompleted?.Invoke();
        OnPurchaseCompleted = null;
        LoadingOverlay.SetActive(false);

        string productId = purchaseEvent.purchasedProduct.definition.id;

        switch (productId)
        {
            case "starter_pack":
                AddMoneyAndUpdateUI(25);
                break;
            case "value":
                AddMoneyAndUpdateUI(20);
                break;
            case "deluxe":
                AddMoneyAndUpdateUI(15);
                break;
            case "19.99":
                AddMoneyAndUpdateUI(10);
                break;
            case "no_ads":
                AddMoneyAndUpdateUI(5);
                break;
            default:
                Debug.LogWarning($"Unmapped product ID: {productId}. Money not added.");
                break;
        }
        return PurchaseProcessingResult.Complete;
    }

    private void AddMoneyAndUpdateUI(int moneyToAdd)
    {
        GameStateManager.EconomyManager.AddMoney(moneyToAdd);
        UpdateUI();
    }
}
