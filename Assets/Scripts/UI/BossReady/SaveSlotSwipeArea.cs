using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotSwipeArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField, Tooltip("스와이프로 인식할 최소 가로 이동 픽셀")]
    float swipeThreshold = 50f;

    public Action OnSwipeLeft;
    public Action OnSwipeRight;

    Vector2 startPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
        Debug.Log($"[Swipe] BeginDrag at {startPos}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // EventSystem이 드래그 타겟을 IDragHandler로 찾기 때문에 빈 구현이라도 필요
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dx = eventData.position.x - startPos.x;
        Debug.Log($"[Swipe] EndDrag dx={dx}, threshold={swipeThreshold}");
        if (Mathf.Abs(dx) < swipeThreshold)
            return;
        if (dx < 0f)
        {
            Debug.Log("[Swipe] OnSwipeLeft invoke");
            OnSwipeLeft?.Invoke();
        }
        else
        {
            Debug.Log("[Swipe] OnSwipeRight invoke");
            OnSwipeRight?.Invoke();
        }
    }
}
