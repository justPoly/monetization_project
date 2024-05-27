using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EconomyReward : MonoBehaviour
{
    public Transform coinPrefab;
    public Transform targetArea;
    public int coinCount = 6; // Number of coins in the collection
    public float spawnDelay = 0.1f;
    public float moveDuration = 1f;
    public Vector3 targetOffset = new Vector3(-100, 100, 0); // Top-left offset
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

    IEnumerator SpawnCoinsCoroutine(Transform spawnArea)
    {
        for (int i = 0; i < coinCount; i++)
        {
            Transform coin = Instantiate(coinPrefab, spawnArea.position, Quaternion.identity, canvas.transform);
            MoveCoin(coin);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void MoveCoin(Transform coin)
    {
        Vector3 targetPosition = targetArea.position + targetOffset;

        coin.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => Destroy(coin.gameObject));
    }

}
