using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EconomyReward : MonoBehaviour
{
    public Transform coinPrefab;
    public Transform gemPrefab; // Gem prefab
    public Transform coinTargetArea;
    public Transform gemTargetArea; // Gem target area
    public int coinCount = 6; // Number of coins in the collection
    public int gemCount = 6; // Number of gems in the collection
    public float spawnDelay = 0.1f;
    public float moveDuration = 1f;
    public Vector3 coinTargetOffset = new Vector3(-100, 100, 0); // Coin top-left offset
    public Vector3 gemTargetOffset = new Vector3(-100, 100, 0); // Gem top-left offset
    public Canvas canvas;

    private Transform lastButtonTransform; // To store the button's transform

    void Start()
    {
        // Example: Set up button click events for store buttons
        Button[] storeButtons = GetComponentsInChildren<Button>();
        foreach (Button button in storeButtons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
    }

    public void OnButtonClick(Button button)
    {
        // Store the button's transform
        lastButtonTransform = button.transform;

        // You can call the method that starts the coin collection effect from here or from another place
        // StartCoinCollection();
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
