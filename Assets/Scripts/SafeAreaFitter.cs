using UnityEngine;

// Attach to a child Panel of a Canvas.
// Resizes the RectTransform every frame to match Screen.safeArea,
// so all UI placed inside is kept away from notches and rounded corners.
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform _rt;
    Rect _applied = Rect.zero;

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        Apply();
    }

#if UNITY_EDITOR
    void Update() => Apply(); // reacts to Game View resolution changes in editor
#endif

    void Apply()
    {
        var safe = Screen.safeArea;
        if (safe == _applied) return;
        _applied = safe;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        var pixelRect = canvas.pixelRect;
        var ancMin = new Vector2(safe.xMin / pixelRect.width, safe.yMin / pixelRect.height);
        var ancMax = new Vector2(safe.xMax / pixelRect.width, safe.yMax / pixelRect.height);

        _rt.anchorMin = ancMin;
        _rt.anchorMax = ancMax;
        _rt.offsetMin = Vector2.zero;
        _rt.offsetMax = Vector2.zero;
    }
}
