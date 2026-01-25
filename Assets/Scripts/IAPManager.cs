using UnityEngine;
using UnityEngine.Purchasing;
using System;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance { get; private set; }
    
    [SerializeField] private string multiplayerProductId = "com.yourcompany.711route.multiplayer";
    
    public bool IsMultiplayerUnlocked { get; private set; }
    
    public event Action<bool> OnRestoreComplete;
    public event Action OnPurchaseSuccess;
    public event Action<string> OnPurchaseError;
    
    private const string MULTIPLAYER_UNLOCKED_KEY = "MultiplayerUnlocked";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPurchaseState();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void LoadPurchaseState()
    {
        IsMultiplayerUnlocked = PlayerPrefs.GetInt(MULTIPLAYER_UNLOCKED_KEY, 0) == 1;
    }
    
    private void SavePurchaseState()
    {
        PlayerPrefs.SetInt(MULTIPLAYER_UNLOCKED_KEY, IsMultiplayerUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == multiplayerProductId)
        {
            IsMultiplayerUnlocked = true;
            SavePurchaseState();
            OnPurchaseSuccess?.Invoke();
            Debug.Log("Multiplayer unlocked!");
        }
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        if (product.definition.id == multiplayerProductId)
        {
            Debug.LogError($"Purchase failed: {description.reason} - {description.message}");
            OnPurchaseError?.Invoke(description.message);
        }
    }
    
    public void OnProductFetched(Product product)
    {
        if (product.definition.id == multiplayerProductId)
        {
            Debug.Log($"Product fetched: {product.metadata.localizedPriceString}");
        }
    }
    
    public void RestorePurchases()
    {
#if UNITY_IOS
        var apple = CodelessIAPStoreListener.Instance.GetStoreExtensions<IAppleExtensions>();
        if (apple != null)
        {
            apple.RestoreTransactions(OnRestoreCallback);
        }
        else
        {
            CheckReceiptDirectly();
        }
#else
        CheckReceiptDirectly();
#endif
    }
    
    private void OnRestoreCallback(bool success, string error)
    {
        if (success)
        {
            CheckReceiptDirectly();
        }
        OnRestoreComplete?.Invoke(success);
        Debug.Log(success ? "Restore completed" : $"Restore failed: {error}");
    }
    
    private void CheckReceiptDirectly()
    {
        var storeController = CodelessIAPStoreListener.Instance.StoreController;
        if (storeController == null)
        {
            OnRestoreComplete?.Invoke(false);
            return;
        }
        
        var product = storeController.products.WithID(multiplayerProductId);
        if (product != null && product.hasReceipt)
        {
            IsMultiplayerUnlocked = true;
            SavePurchaseState();
        }
        
        OnRestoreComplete?.Invoke(true);
    }
}          