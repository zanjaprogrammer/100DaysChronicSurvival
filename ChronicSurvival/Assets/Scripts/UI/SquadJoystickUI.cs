using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChronicSurvival.UI
{
    public class SquadJoystickUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform stickArea;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float handleRange = 72f;

        public Vector2 Value { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateHandle(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateHandle(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector2.zero;
            if (handle != null)
            {
                handle.anchoredPosition = Vector2.zero;
            }
        }

        private void UpdateHandle(PointerEventData eventData)
        {
            if (stickArea == null || handle == null)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(stickArea, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            {
                return;
            }

            Vector2 clamped = Vector2.ClampMagnitude(localPoint, handleRange);
            handle.anchoredPosition = clamped;
            Value = Vector2.ClampMagnitude(clamped / Mathf.Max(handleRange, 1f), 1f);
        }
    }
}
