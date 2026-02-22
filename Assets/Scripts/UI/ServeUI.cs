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
            
            Hide();
        }

        private void Start()
        {
            roundManager = Services.Get<IRoundService>();
        }

        public void Show()
        {
            running = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            running = false;
            gameObject.SetActive(false);
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