using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BounceButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private Button button;
    private Vector2 originalScale;


    private void Start()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(new Vector3(originalScale.x * 0.9f, originalScale.y * 0.9f, 1f), 0.2f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(originalScale, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("Pointer Clicked");
    }
}
