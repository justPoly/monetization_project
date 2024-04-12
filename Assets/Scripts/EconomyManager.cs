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

    public delegate void OnMoneyChanged(double newAmount);
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
    public float startingMoney = 2500f;

    [BoxGroup("Config")]
    [Tooltip("The amount of gems the player starts the game with")]
    public float startingGems = 250f;

    [BoxGroup("Level Amount")]
    [Tooltip("Money earned after the player has completed a level")]
    public float completedLevelReward = 200;
    public int TotalStars { get; set; }
    public int TotalSecrets { get; set; }

    public void AddTotalStar(int _numberOf)
    {
        TotalStars += _numberOf;
        PlayerPrefs.SetInt("TotalStars", TotalStars);
    }

    public void AddTotalSecret(int _numberOf)
    {
        TotalSecrets += _numberOf;
        PlayerPrefs.SetInt("TotalSecrets", TotalSecrets); ;
    }

    public double Gems
    {
        get { return m_Gems; }
        set
        {
            m_Gems = value;
            PlayerPrefs.SetInt("Gems", (int)m_Gems);
            PlayerPrefs.Save();

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
            PlayerPrefs.SetFloat("Money", (float)m_Money);
            PlayerPrefs.Save();

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
        Money = PlayerPrefs.GetFloat("Money", (int)startingMoney);
        Gems = PlayerPrefs.GetInt("Gems", (int)startingGems);
        TotalStars = PlayerPrefs.GetInt("TotalStars", 0);
        TotalSecrets = PlayerPrefs.GetInt("TotalSecrets", 0);

        onMoneyChanged.Raise((float)Money);
        onGemsChanged.Raise((int)Gems);
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
        PlayerPrefs.SetFloat("TotalMoney", (float)TotalMoney);
        onTotalMoneyIncreased.Raise((float)TotalMoney);
    }

    public void SetMoney(double amount)
    {
        Money = amount;
        PlayerPrefs.SetFloat("Money", (float)Money);

        if (m_onMoneyChanged != null)
            m_onMoneyChanged.Invoke(Money);

        onMoneyChanged.Raise((float)Money);
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
        PlayerPrefs.SetFloat("Money", (float)Money);
    }

    public void ReduceMoney(double amount)
    {
        Money -= amount;
        PlayerPrefs.SetFloat("Money", (float)Money);

        if (Money < 0)
        {
            Money = 0;
            PlayerPrefs.SetFloat("Money", (float)Money);
        }

        if (m_onMoneyChanged != null)
            m_onMoneyChanged(Money);

        onMoneyChanged?.Raise((float)Money);
    }

    public void AddGems(double amount)
    {
        Gems += amount;
        PlayerPrefs.SetInt("Gems", (int)Gems);
        onGemsChanged?.Raise((int)Gems);
        m_onGemsChanged?.Invoke((int)Gems);
    }

    public void ReduceGems(double amount)
    {
        Gems -= amount;
        PlayerPrefs.SetInt("Gems", (int)Gems);
        if (Gems < 0)
        {
            Gems = 0;
        }
        onGemsChanged?.Raise((int)Gems);
        m_onGemsChanged?.Invoke((int)Gems);
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("Money", Money.ToString());
        PlayerPrefs.SetString("TotalMoney", TotalMoney.ToString());
        PlayerPrefs.SetInt("Gems", (int)Gems);
    }

    [Button]
    public void AddStartingBalances()
    {
        Money = startingMoney;
        Gems = startingGems;
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
}
