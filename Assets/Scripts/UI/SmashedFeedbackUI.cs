using System.Threading;
using System.Threading.Tasks;
using Smashball.Gameplay;
using TMPro;
using UnityEngine;

namespace Smashball.UI
{
    public sealed class SmashedFeedbackUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private RectTransform root;
        [SerializeField] private Vector3 worldOffset = new(0f, 2.2f, 0f);
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float duration = 0.4f;
        [SerializeField] private float maxScale = 1.4f;
        [SerializeField] private int holdMs = 300;

        private Transform target;
        private Camera cam;
        private RectTransform parentRect;
        private Canvas canvas;
        private CancellationTokenSource cts;
        private IRoundService roundManager;

        private void Awake()
        {
            cam = Camera.main;
            canvas = root.GetComponentInParent<Canvas>();
            parentRect = root.parent as RectTransform;
            root.localScale = Vector3.zero;
            root.gameObject.SetActive(false);
        }

        public async Task ShowAsync(Transform targetTransform, bool opponentHit)
        {
            roundManager ??= Services.Get<IRoundService>();
            feedbackText.color = opponentHit ? roundManager.PlayerColor : roundManager.OpponentColor;
            target = targetTransform;

            cts?.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            root.localScale = Vector3.zero;
            root.gameObject.SetActive(true);

            float t = 0f;

            try
            {
                while (t < duration)
                {
                    if (token.IsCancellationRequested)
                        return;

                    t += Time.deltaTime;
                    float normalized = Mathf.Clamp01(t / duration);

                    float curveValue = scaleCurve.Evaluate(normalized);
                    float scale = Mathf.LerpUnclamped(0f, maxScale, curveValue);

                    root.localScale = Vector3.one * scale;

                    await Task.Yield();
                }

                root.localScale = Vector3.one;
            }
            finally
            {
                await Task.Delay(holdMs, token);
                Hide();
            }
        }

        private void Hide()
        {
            target = null;
            root.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 w = target.position + worldOffset;
            Vector3 sp = cam.WorldToScreenPoint(w);

            Camera eventCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, sp, eventCam, out Vector2 local);
            root.anchoredPosition = local;
        }
    }
}