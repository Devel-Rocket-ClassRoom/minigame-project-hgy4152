using UnityEngine;

namespace Assets.Scripts.UI.Util
{
    /// <summary>
    /// UI 패널이 일정 크기 이상 커지지 않도록 제한하면서, 화면이 작아질 때는 유연하게 줄어들게 하는 컴포넌트입니다.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class ResponsiveMaxSize : MonoBehaviour
    {
        [Header("Constraints")]
        [Tooltip("패널이 가질 수 있는 최대 크기입니다.")]
        public Vector2 maxSize = new Vector2(1200, 800);
        
        [Tooltip("화면 끝과 패널 사이의 최소 여백입니다.")]
        public Vector2 padding = new Vector2(100, 100);

        private RectTransform _rect;
        private RectTransform _parentRect;

        private void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            _rect = GetComponent<RectTransform>();
            if (transform.parent != null)
            {
                _parentRect = transform.parent.GetComponent<RectTransform>();
            }

            // 제어를 용이하게 하기 위해 앵커를 중앙으로 설정합니다.
            if (!Application.isPlaying)
            {
                _rect.anchorMin = new Vector2(0.5f, 0.5f);
                _rect.anchorMax = new Vector2(0.5f, 0.5f);
                _rect.pivot = new Vector2(0.5f, 0.5f);
            }
        }

        private void LateUpdate()
        {
            if (_rect == null) return;
            
            // 런타임에 부모가 바뀌었을 수 있으므로 체크합니다.
            if (_parentRect == null && transform.parent != null)
            {
                _parentRect = transform.parent.GetComponent<RectTransform>();
            }

            if (_parentRect == null) return;

            // 부모(Canvas 또는 부모 패널)의 현재 크기
            Vector2 parentSize = _parentRect.rect.size;

            // 목표 크기 계산: (부모 크기 - 여백)과 (최대 크기) 중 작은 값 선택
            float targetWidth = Mathf.Min(parentSize.x - padding.x, maxSize.x);
            float targetHeight = Mathf.Min(parentSize.y - padding.y, maxSize.y);

            // 크기가 음수가 되지 않도록 클램핑
            targetWidth = Mathf.Max(targetWidth, 0);
            targetHeight = Mathf.Max(targetHeight, 0);

            Vector2 newSize = new Vector2(targetWidth, targetHeight);

            // 변화가 있을 때만 적용 (부동소수점 오차 고려)
            if (Vector2.Distance(_rect.sizeDelta, newSize) > 0.01f)
            {
                _rect.sizeDelta = newSize;
            }
        }
    }
}
