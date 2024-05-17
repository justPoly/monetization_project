using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Economy Manager")]
public class EconomyManager : ScriptableObject
{
    [FancyHeader("$  ECONOMY MANAGER  $", 1.5f, "lime", 5.5f, order = 0)]
    [Label("Money Balance")]
    [ReadOnly]
    [SerializeField]
    private double m_Money;

    [Label("Total amount accumulated")]
    [ReadOnly]
    [SerializeField]
    private double m_TotalMoney;

    [Label("Gem Balance")]
    [ReadOnly]
    [SerializeField]
    private double m_Gems;

    [BoxGroup("SO Events")]
    [Required]
    public IntEvent onGemsChanged;

    public delegate void OnMoneyChanged(float newAmount);
    public static OnMoneyChanged m_onMoneyChanged;

    public delegate void OnGemsChanged(int newAmount);
    public static OnGemsChanged m_onGemsChanged;

    [BoxGroup("SO Events")]
    [Required]
    public FloatEvent onMoneyChanged;

    [Space, BoxGroup("SO Events"), Required]
    public FloatEvent onTotalMoneyIncreased;

    [BoxGroup("Config")]
    [Tooltip("The amount of money the player starts the game with")]
    public float startingMoney = 50f;

    [BoxGroup("Config")]
    [Tooltip("The amount of gems the player starts the game with")]
    public float startingGems = 5f;

    public double Gems
    {
        get { return m_Gems; }
        set
        {
            m_Gems = value;
            onGemsChanged?.Raise((int)m_Gems);
            m_onGemsChanged?.Invoke((int)m_Gems);
        }
    }

    public double Money
    {
        get { return m_Money; }
        set
        {
            m_Money = value;
            onMoneyChanged?.Raise((float)m_Money);
            m_onMoneyChanged?.Invoke((float)m_Money);
        }
    }

    public double TotalMoney
    {
        get { return m_TotalMoney; }
        set { m_TotalMoney = value; }
    }

    public void InitializeValues()
    {
        if (!PlayerPrefs.HasKey("Money"))
        {
            Money = startingMoney;
        }
        else
        {
            Money = PlayerPrefs.GetFloat("Money", (float)startingMoney);
        }

        if (!PlayerPrefs.HasKey("Gems"))
        {
            Gems = startingGems;
        }
        else
        {
            Gems = PlayerPrefs.GetInt("Gems", (int)startingGems);
        }

        TotalMoney = PlayerPrefs.GetFloat("TotalMoney", 0);

        onMoneyChanged.Raise((float)Money);
        onGemsChanged.Raise((int)Gems);
        onTotalMoneyIncreased.Raise((float)TotalMoney);
    }

    public void SetTotalMoney(double amount)
    {
        TotalMoney = amount;
        onTotalMoneyIncreased.Raise((float)TotalMoney);
        SaveData();
    }

    public void AddToTotalMoney(double amount)
    {
        TotalMoney += amount;
        onTotalMoneyIncreased.Raise((float)TotalMoney);
        SaveData();
    }

    public void SetMoney(double amount)
    {
        Money = amount;
        onMoneyChanged.Raise((float)Money);
        m_onMoneyChanged?.Invoke((float)Money);
        SaveData();
    }

    public void AddMoney(float a)
    {
        AddMoney((double)Mathf.Round(a));
    }

    public void SpendMoney(float a)
    {
        ReduceMoney((double)Mathf.Round(a));
    }

    public void AddMoney(double amount, bool addTotal = true)
    {
        Money += amount;
        if (addTotal) AddToTotalMoney(amount);
        SaveData();
    }

    public void ReduceMoney(double amount)
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

    public float testAddMoneyAmount;
    public int testGemsAddAmount;

    [Button]
    public void TestAddPlayerGems()
    {
        AddGems(testGemsAddAmount);
    }

    [Button]
    public void TestAddPlayerMoney()
    {
        AddMoney(testAddMoneyAmount);
    }

    public void UpdateCurrency()
    {
        GameManager.Instance.UpdateUI();
    }
}
