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


public class IAPManager : MonoBehaviour, IStoreListener
{
    [SerializeField]
    private UIProduct UIProductPrefab;
    [SerializeField]
    private VerticalLayoutGroup ContentPanel; 
    [SerializeField]
    private GameObject LoadingOverlay;
    [SerializeField]
    private bool UseFakeStore = false;
    [SerializeField]
    public TMP_Text diamondText;
    [SerializeField]
    public TMP_Text gemsText;

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
        // Update moneyText and gemsText directly based on the current counts and economy manager values
        diamondText.text = $"Diamonds: {((int)GameStateManager.EconomyManager.Money).ToString()}";
        gemsText.text = $"Gems: {((int)GameStateManager.EconomyManager.Gems).ToString()}";
    }

    private void HandleIAPCatalogLoaded(AsyncOperation Operation)
    {
        ResourceRequest request = Operation as ResourceRequest;
        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

        if (UseFakeStore) // Use bool in editor to control fake store behavior.
        {
            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser; // Comment out this line if you are building the game for publishing.
            StandardPurchasingModule.Instance().useFakeStoreAlways = true; // Comment out this line if you are building the game for publishing.
        }

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

        foreach(ProductCatalogItem item in catalog.allProducts)
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
        StoreIconProvider.Initialize(StoreController.products);
        StoreIconProvider.OnLoadComplete += HandleAllIconsLoaded;
        // throw new System.NotImplementedException();
    }

    // public void OnInitializeFailed(OnInitializationFailureReason error)
    // {
        //old implementation
    // }
    
    private void HandleAllIconsLoaded()
    {
        StartCoroutine(CreateUI());
    }

    private IEnumerator CreateUI()
    {
        List<Product> sortedProducts = StoreController.products.all
        .OrderBy(item => item.metadata.localizedPrice)
        .ToList();

        foreach(Product product in sortedProducts)
        {
            UIProduct uiProduct = Instantiate(UIProductPrefab);
            uiProduct.OnPurchase += HandlePurchase;
            uiProduct.Setup(product);
            uiProduct.transform.SetParent(ContentPanel.transform, false);
            yield return null;
        }
    }

    private void HandlePurchase(Product Product, Action OnPurchaseCompleted)
    {
          LoadingOverlay.SetActive(true);
          StoreController.InitiatePurchase(Product);
          this.OnPurchaseCompleted = OnPurchaseCompleted;
    }

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {
       OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
       //implementation for new
       Debug.LogError($"Error initializing IAP because of {error}." + 
       $"\r\nShow a message to the player depending on the error.");
       throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Failed to purchase {product.definition.id} because {failureReason}");
        OnPurchaseCompleted?.Invoke();
        OnPurchaseCompleted = null;
        LoadingOverlay.SetActive(false);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
        OnPurchaseCompleted?.Invoke();
        OnPurchaseCompleted = null;
        LoadingOverlay.SetActive(false);

        //do something, like give the player their currency, unlock the item,
        //update some metrics or analytics, etc.

        return PurchaseProcessingResult.Complete;
    }
}