using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotSwipeArea : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField, Tooltip("스와이프로 인식할 최소 가로 이동 픽셀")]
    float swipeThreshold = 50f;

    public Action OnSwipeLeft;
    public Action OnSwipeRight;

    Vector2 startPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dx = eventData.position.x - startPos.x;
        if (Mathf.Abs(dx) < swipeThreshold)
            return;
        if (dx < 0f)
            OnSwipeLeft?.Invoke();
        else
            OnSwipeRight?.Invoke();
    }
}
