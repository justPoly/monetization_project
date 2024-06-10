using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EconomyReward : MonoBehaviour
{
    public static EconomyReward Instance { get; private set; }

    public Transform coinPrefab;
    public Transform gemPrefab;
    public Transform coinTargetArea;
    public Transform gemTargetArea;
    public int coinCount = 6;
    public int gemCount = 6;
    public float spawnDelay = 0.1f;
    public float moveDuration = 1f;
    public Vector3 coinTargetOffset = new Vector3(-100, 26, 0);
    public Vector3 gemTargetOffset = new Vector3(-100, 26, 0);
    public Canvas canvas;
    
    private Transform lastButtonTransform;
    private RewardType lastRewardType;

    [Serializable]
    public enum RewardType
    {
        Coins,
        Gems,
        Both
    }

    [Serializable]
    public class ButtonReward
    {
        public Button button;
        public RewardType rewardType;
    }

    [Header("Button Reward Settings")]
    public List<ButtonReward> buttonRewards;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the singleton alive across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures that there is only one instance
        }
    }

    void Start()
    {
        foreach (ButtonReward buttonReward in buttonRewards)
        {
            buttonReward.button.onClick.AddListener(() => OnButtonClick(buttonReward.button, buttonReward.rewardType));
        }
    }

    public void OnButtonClick(Button button, RewardType rewardType)
    {
        lastButtonTransform = button.transform;
        lastRewardType = rewardType;
    }

    public void SetLastButtonTransform(Transform buttonTransform, RewardType rewardType)
    {
        lastButtonTransform = buttonTransform;
        lastRewardType = rewardType;
    }

    public void TriggerRewardEffect()
    {
        SetRewardEffect(lastRewardType);
    }

    public void SetRewardEffect(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.Coins:
                StartCoinCollection();
                break;
            case RewardType.Gems:
                StartGemCollection();
                break;
            case RewardType.Both:
                StartBothCollections();
                break;
            default:
                Debug.LogError("Unknown reward type.");
                break;
        }
    }

    public void StartCoinCollection()
    {
        if (lastButtonTransform != null)
        {
            StartCoroutine(SpawnCoinsCoroutine(lastButtonTransform));
        }
        else
        {
            Debug.LogError("No button transform stored. Please click a button first.");
        }
    }

    public void StartGemCollection()
    {
        if (lastButtonTransform != null)
        {
            StartCoroutine(SpawnGemsCoroutine(lastButtonTransform));
        }
        else
        {
            Debug.LogError("No button transform stored. Please click a button first.");
        }
    }

    public void StartBothCollections()
    {
        if (lastButtonTransform != null)
        {
            StartCoroutine(SpawnCoinsCoroutine(lastButtonTransform));
            StartCoroutine(SpawnGemsCoroutine(lastButtonTransform));
        }
        else
        {
            Debug.LogError("No button transform stored. Please click a button first.");
        }
    }

    IEnumerator SpawnCoinsCoroutine(Transform spawnArea)
    {
        for (int i = 0; i < coinCount; i++)
        {
            Transform coin = Instantiate(coinPrefab, spawnArea.position, Quaternion.identity, canvas.transform);
            MoveCoin(coin);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator SpawnGemsCoroutine(Transform spawnArea)
    {
        for (int i = 0; i < gemCount; i++)
        {
            Transform gem = Instantiate(gemPrefab, spawnArea.position, Quaternion.identity, canvas.transform);
            MoveGem(gem);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void MoveCoin(Transform coin)
    {
        Vector3 targetPosition = coinTargetArea.position + coinTargetOffset;

        coin.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => Destroy(coin.gameObject));
    }

    void MoveGem(Transform gem)
    {
        Vector3 targetPosition = gemTargetArea.position + gemTargetOffset;

        gem.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => Destroy(gem.gameObject));
    }
}
