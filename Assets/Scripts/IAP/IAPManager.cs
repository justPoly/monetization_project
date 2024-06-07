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
using Firebase.Analytics;

public class IAPManager : MonoBehaviour, IStoreListener, IDetailedStoreListener
{
    [SerializeField]
    private List<UIProduct> uiProducts;
    [SerializeField]
    private GameObject LoadingOverlay;
    [SerializeField]
    private GameObject purchaseFailed;
    [SerializeField]
    private GameObject purchaseSuccessful;
    [SerializeField]
    private bool UseFakeStore = false;
    [SerializeField]
    public TMP_Text diamondText;
    [SerializeField]
    public TMP_Text gemsText;

    private Action OnPurchaseCompleted;
    private IStoreController StoreController;
    private IExtensionProvider ExtensionProvider;

    [Serializable]
    public class SubscriptionProduct
    {
        public string productID;
        public int validityDays;
        // public int validityMinutes;
    }

    public List<SubscriptionProduct> subscriptionProducts;

    private Dictionary<string, Action> productActions;

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
        CheckAndHandleSubscriptions();

        InitializeProductActions();
    }

    public void Start()
    {
        purchaseSuccessful.SetActive(false);
    }

    public void UpdateUI()
    {
        diamondText.text = $": {((int)GameStateManager.EconomyManager.Money).ToString()}";
        gemsText.text = $": {((int)GameStateManager.EconomyManager.Gems).ToString()}";
        GameStateManager.EconomyManager.UpdateCurrency();
    }

    private void HandleIAPCatalogLoaded(AsyncOperation Operation)
    {
        ResourceRequest request = Operation as ResourceRequest;
        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

#if UNITY_EDITOR
        UseFakeStore = true;
        if (UseFakeStore)
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

        // Track button press with Firebase Analytics
        string itemId = Product.definition.id;
        FirebaseAnalytics.LogEvent("store_item_pressed", new Parameter("item_id", itemId));
        Debug.Log($"Firebase Analytics: Store item pressed - {itemId}");
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

        // Track successful purchase with Firebase Analytics
        FirebaseAnalytics.LogEvent("purchase_success", new Parameter("item_id", productId));
        Debug.Log($"Firebase Analytics: Purchase success - {productId}");

        // Save the purchase date and calculate expiration date
        DateTime purchaseDate = DateTime.UtcNow;
        var subscriptionProduct = subscriptionProducts.FirstOrDefault(x => x.productID == productId);
        if (subscriptionProduct != null)
        {
            // For testing, we use 5 minutes instead of days
            DateTime expirationDate = purchaseDate.AddMinutes(5);
            // Calculate expiration in days
            // DateTime expirationDate = purchaseDate.AddDays(subscriptionProduct.validityDays);
            PlayerPrefs.SetString(productId + "_expiration", expirationDate.ToString());
        }

        // Execute the corresponding action
        if (productActions.TryGetValue(productId, out Action action))
        {
            action.Invoke();
        }
        else
        {
            Debug.LogWarning($"Unmapped product ID: {productId}. Reward not added.");
        }

        return PurchaseProcessingResult.Complete;
    }

    private void AddMoneyAndUpdateUI(int moneyToAdd)
    {
        GameStateManager.EconomyManager.AddMoney(moneyToAdd);
        UpdateUI();
        LoadingOverlay.SetActive(true);
        purchaseSuccessful.SetActive(true);
        PlayerPrefs.Save();
    }

    private void AddGemsAndUpdateUI(int gemsToAdd)
    {
        GameStateManager.EconomyManager.AddGems(gemsToAdd);
        UpdateUI();
        LoadingOverlay.SetActive(true);
        purchaseSuccessful.SetActive(true);
        PlayerPrefs.Save();
    }

    private void ActivateNoAdsForTesting(int minutes)
    {
        AdsManager.Instance.DisableAds();
        StartCoroutine(EnableAdsAfterTime(minutes));
    }

    private IEnumerator EnableAdsAfterTime(int minutes)
    {
        yield return new WaitForSeconds(minutes * 60);
        AdsManager.Instance.EnableAds();
    }

    private void CheckAndHandleSubscriptions()
    {
        foreach (var subscription in subscriptionProducts)
        {
            string expirationKey = subscription.productID + "_expiration";
            if (PlayerPrefs.HasKey(expirationKey))
            {
                DateTime expirationDate = DateTime.Parse(PlayerPrefs.GetString(expirationKey));
                if (DateTime.UtcNow > expirationDate)
                {
                    AdsManager.Instance.EnableAds();
                    PlayerPrefs.DeleteKey(expirationKey);
                }
            }
        }
    }

    public void OnPurchaseSuccessful()
    {
        UpdateUI();
        purchaseSuccessful.SetActive(false);
        LoadingOverlay.SetActive(false);
    }

    private void InitializeProductActions()
    {
        productActions = new Dictionary<string, Action>
        {
            { "starter_p", () => AddMoneyAndUpdateUI(10) },
            { "value_p", () => AddMoneyAndUpdateUI(15) },
            { "deluxe_p", () => AddMoneyAndUpdateUI(20) },
            { "premium_p", () => AddMoneyAndUpdateUI(25) },
            { "gems_pack", () => AddGemsAndUpdateUI(50) },
            { "no_ads_subscription", () => ActivateNoAdsForTesting(5) }
            // Add more product actions here as needed
        };
    }

}
