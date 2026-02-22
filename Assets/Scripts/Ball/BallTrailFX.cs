using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class BallTrailFX : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private float minWidth = 0.06f;
        [SerializeField] private float maxWidth = 0.22f;
        [SerializeField] private float minTime = 0.10f;
        [SerializeField] private float maxTime = 0.30f;
        [SerializeField] private float smoothSpeed = 12f;

        private float targetWidth;
        private float targetTime;

        private void Awake()
        {
            if (trail == null)
                trail = GetComponent<TrailRenderer>();

            targetWidth = trail.widthMultiplier;
            targetTime = trail.time;
        }

        private void Update()
        {
            if (trail == null)
                return;

            trail.widthMultiplier = Mathf.Lerp(trail.widthMultiplier, targetWidth, smoothSpeed * Time.deltaTime);
            trail.time = Mathf.Lerp(trail.time, targetTime, smoothSpeed * Time.deltaTime);
        }

        public void SetIntensityFromQuality(float quality)
        {
            float q = Mathf.Clamp01(quality);
            q = q * q;

            targetWidth = Mathf.Lerp(minWidth, maxWidth, q);
            targetTime = Mathf.Lerp(minTime, maxTime, q);
        }

        public void Clear()
        {
            if (trail != null)
                trail.Clear();
        }
    }
}