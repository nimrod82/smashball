using Smashball.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Smashball.UI
{
    public sealed class VirtualJoystickUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float maxRadius = 80f;
        [SerializeField] private float deadZone = 0.12f;
        [SerializeField] private Vector2 defaultPosition;

        private Canvas rootCanvas;
        private Camera uiCamera;
        private IInputService inputService;

        private Vector2 value;
        private bool isHeld;

        private void Awake()
        {
            rootCanvas = GetComponentInParent<Canvas>();
            uiCamera = rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? rootCanvas.worldCamera
                : null;

            ResetVisuals();
        }

        private void Start()
        {
            inputService = Services.Get<IInputService>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var parentRect = background.parent as RectTransform;
            if (parentRect == null) return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, uiCamera, out var localInParent))
                return;

            background.anchoredPosition = localInParent;

            isHeld = true;
            UpdateFromEvent(eventData);

            inputService?.SetJoystickMove(value);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isHeld) return;

            UpdateFromEvent(eventData);
            inputService?.SetJoystickMove(value);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHeld = false;
            value = Vector2.zero;
            inputService?.NotifyJoystickReleased();
            ResetVisuals();
        }

        private void UpdateFromEvent(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, uiCamera, out var local))
                return;

            Vector2 clamped = Vector2.ClampMagnitude(local, maxRadius);
            handle.anchoredPosition = clamped;

            Vector2 normalized = clamped / maxRadius;
            if (normalized.sqrMagnitude < deadZone * deadZone)
                normalized = Vector2.zero;
            
            value = normalized;
        }

        private void ResetVisuals()
        {
            background.anchoredPosition = defaultPosition;
            handle.anchoredPosition = Vector2.zero;
        }
    }
}