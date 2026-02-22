using System.Threading;
using System.Threading.Tasks;
using Smashball.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Smashball.UI
{
    public sealed class GameOverUI : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button restartButton;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float maxScale = 1.6f;
        [SerializeField] private float holdMs = 600f;

        private CancellationTokenSource cts;
        private IRoundService roundManager;

        private void Awake()
        {
            root.localScale = Vector3.zero;
            root.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }

        private void RestartGame()
        {
            Hide();
            gameObject.SetActive(false);
            Services.Get<UIManager>().Play();
        }

        public async Task ShowAsync(bool victory)
        {
            roundManager ??= Services.Get<IRoundService>();

            titleText.text = victory ? "Victory!" : "Defeat";
            titleText.color = victory ? roundManager.PlayerColor : roundManager.OpponentColor;

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

                if (holdMs > 0f)
                    await Task.Delay((int)holdMs, token);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }

        public void Hide()
        {
            cts?.Cancel();
            root.gameObject.SetActive(false);
            root.localScale = Vector3.zero;
        }
    }
}