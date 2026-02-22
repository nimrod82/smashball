using Smashball.Gameplay;
using UnityEngine;

namespace Smashball.UI
{
    public class ServeUI : MonoBehaviour
    {
        [SerializeField] private RectTransform arrow;
        [SerializeField] private float speed;

        private IRoundService roundManager;
        private float minY;
        private float maxY;
        private bool running;

        private void Awake()
        {
            var rectTransform = transform as RectTransform;
            float halfHeight = rectTransform.rect.height * 0.5f;

            minY = -halfHeight;
            maxY = halfHeight;
        }

        private void Start()
        {
            roundManager = Services.Get<IRoundService>();
        }

        public void Show(bool show)
        {
            running = show;
            gameObject.SetActive(show);
        }

        private void Update()
        {
            if (!running)
                return;
            
            float t = roundManager.ServeQuality;
            float y = Mathf.Lerp(minY, maxY, t);

            var pos = arrow.anchoredPosition;
            pos.y = y;
            arrow.anchoredPosition = pos;
        }
    }
}