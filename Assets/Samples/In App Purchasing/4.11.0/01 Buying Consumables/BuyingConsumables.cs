using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using TMPro;

namespace Samples.Purchasing.Core.BuyingConsumables
{
    public class BuyingConsumables : MonoBehaviour, IDetailedStoreListener
    {
        IStoreController m_StoreController; // The Unity Purchasing system.

        // Your products IDs from the IAP Catalog.
        public string goldProductId = "1";
        public string diamondProductId = "2";
        public string platinumProductId = "3";
        public string silverProductId = "4";

        public TMP_Text DiamondCountText;   // Updated from GoldCountText to DiamondCountText
        public TMP_Text GemCountText;       // Updated from DiamondCountText to GemCountText

        int m_GoldCount;
        int m_DiamondCount;
        int m_PlatinumCount;    // Track Platinum count
        int m_SilverCount;      // Track Silver count

        void Start()
        {
            InitializePurchasing();
            LoadSavedAmounts();
            UpdateUI();
        }

        void LoadSavedAmounts()
        {
            m_GoldCount = PlayerPrefs.GetInt("GoldCount", 0); // Default to 0 if not found
            m_DiamondCount = PlayerPrefs.GetInt("DiamondCount", 0); // Default to 0 if not found
            m_PlatinumCount = PlayerPrefs.GetInt("PlatinumCount", 0); // Default to 0 if not found
            m_SilverCount = PlayerPrefs.GetInt("SilverCount", 0); // Default to 0 if not found
        }

        void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add products that will be purchasable and indicate its type.
            builder.AddProduct(goldProductId, ProductType.Consumable);
            builder.AddProduct(diamondProductId, ProductType.Consumable);
            builder.AddProduct(platinumProductId, ProductType.Consumable);  // Add Platinum product
            builder.AddProduct(silverProductId, ProductType.Consumable);    // Add Silver product

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyCoin()
        {
            BuyProduct(goldProductId);
        }

        public void BuyGem()
        {
            BuyProduct(diamondProductId);
        }

        public void BuyPlatinum()
        {
            BuyProduct(platinumProductId);
        }

        public void BuySilver()
        {
            BuyProduct(silverProductId);
        }

        void BuyProduct(string productId)
        {
            if (m_StoreController != null)
            {
                m_StoreController.InitiatePurchase(productId);
            }
            else
            {
                Debug.LogError("Store controller is not initialized!");
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // Retrieve the purchased product
            var product = args.purchasedProduct;

            // Get the payout quantity from the product's metadata
            int payoutQuantity = GetPayoutQuantity(product.definition.id);

            // Add the purchased product to the player's inventory
            switch (product.definition.id)
            {
                case "1":
                    AddGold(payoutQuantity);
                    break;
                case "2":
                    AddDiamond(payoutQuantity);
                    break;
                case "3":
                    AddPlatinum(payoutQuantity);
                    break;
                case "4":
                    AddSilver(payoutQuantity);
                    break;
            }

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            // Save the updated counts immediately after purchase
            SaveCounts();

            // Return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
                $" Purchase failure reason: {failureDescription.reason}," +
                $" Purchase failure details: {failureDescription.message}");
        }

        void AddGold(int quantity)
        {
            m_GoldCount += quantity;
            UpdateUI();
        }

        void AddDiamond(int quantity)
        {
            m_DiamondCount += quantity;
            UpdateUI();
        }

        void AddPlatinum(int quantity)
        {
            m_GoldCount += quantity;
            UpdateUI();
        }

        void AddSilver(int quantity)
        {
            m_DiamondCount += quantity;
            UpdateUI();
        }

        void UpdateUI()
        {
            DiamondCountText.text = $"Diamonds: {m_DiamondCount}";
            GemCountText.text = $"Gems: {m_GoldCount}";
        }

        int GetPayoutQuantity(string productId)
        {
            // You can modify this method to fetch the payout quantity from your IAP Catalog or database
            // For simplicity, this example returns a fixed quantity based on the product ID
            switch (productId)
            {
                case "1":
                    return 10;
                case "2":
                    return 60; 
                case "3":
                    return 140; 
                case "4":
                    return 300;
                default:
                    return 0; 
            }
        }

        void SaveCounts()
        {
            PlayerPrefs.SetInt("GoldCount", m_GoldCount);
            PlayerPrefs.SetInt("DiamondCount", m_DiamondCount);
            PlayerPrefs.SetInt("PlatinumCount", m_PlatinumCount);
            PlayerPrefs.SetInt("SilverCount", m_SilverCount);
        }

        void OnApplicationQuit()
        {
            SaveCounts();
        }
    }
}
