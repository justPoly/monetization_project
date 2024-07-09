using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Managers/Economy Manager")]
public class EconomyManager : ScriptableObject
{
    [Header("Money")]
    [ReadOnly]
    [SerializeField] private double m_Money;

    [Header("Total Money")]
    [ReadOnly]
    [SerializeField] private double m_TotalMoney;

    [Header("Gems")]
    [ReadOnly]
    [SerializeField] private double m_Gems;

    [BoxGroup("Events")]
    public UnityEvent<int> onGemsChanged;

    public UnityEvent<float> onMoneyChanged;
    public UnityEvent<float> onTotalMoneyIncreased;

    [Header("Config")]
    [Tooltip("The amount of money the player starts the game with")]
    public float startingMoney = 50f;

    [Tooltip("The amount of gems the player starts the game with")]
    public float startingGems = 5f;

    public double Gems
    {
        get { return m_Gems; }
        set
        {
            m_Gems = value;
            onGemsChanged?.Invoke((int)m_Gems);
        }
    }

    public double Money
    {
        get { return m_Money; }
        set
        {
            m_Money = value;
            onMoneyChanged?.Invoke((float)m_Money);
        }
    }

    public double TotalMoney
    {
        get { return m_TotalMoney; }
        set
        {
            m_TotalMoney = value;
            onTotalMoneyIncreased?.Invoke((float)m_TotalMoney);
        }
    }

    public void InitializeValues()
    {
        Money = PlayerPrefs.GetFloat("Money", startingMoney);
        Gems = PlayerPrefs.GetInt("Gems", (int)startingGems);
        TotalMoney = PlayerPrefs.GetFloat("TotalMoney", 0);

        SaveData();
    }

    public void SetTotalMoney(double amount)
    {
        TotalMoney = amount;
        SaveData();
    }

    public void AddToTotalMoney(double amount)
    {
        TotalMoney += amount;
        SaveData();
    }

    public void SetMoney(double amount)
    {
        Money = amount;
        SaveData();
    }

    public void AddMoney(double amount)
    {
        Money += amount;
        SaveData();
    }

    public void SpendMoney(double amount)
    {
        Money -= amount;
        if (Money < 0)
        {
            Money = 0;
        }
        SaveData();
    }

    public void AddGems(double amount)
    {
        Gems += amount;
        SaveData();
    }

    public void ReduceGems(double amount)
    {
        Gems -= amount;
        if (Gems < 0)
        {
            Gems = 0;
        }
        SaveData();
    }

    public void SaveData()
    {
        PlayerPrefs.SetFloat("Money", (float)Money);
        PlayerPrefs.SetFloat("TotalMoney", (float)TotalMoney);
        PlayerPrefs.SetInt("Gems", (int)Gems);
        PlayerPrefs.Save();
    }

    [Button]
    public void AddStartingBalances()
    {
        Money = startingMoney;
        Gems = startingGems;
        SaveData();
    }

    [Button]
    public void TestAddPlayerGems(int testGemsAddAmount)
    {
        AddGems(testGemsAddAmount);
    }

    [Button]
    public void TestAddPlayerMoney(float testAddMoneyAmount)
    {
        AddMoney(testAddMoneyAmount);
    }

    public void UpdateCurrency()
    {
        GameManager.Instance.UpdateUI();
    }
}
