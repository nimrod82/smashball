using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class CameraShake : MonoBehaviour, ICameraShake
    {
        [SerializeField] private float baseDuration = 0.15f;
        [SerializeField] private float maxAmplitude = 0.35f;
        [SerializeField] private float frequency = 35f;
        [SerializeField] private float noiseOffsetA = 11.7f;
        [SerializeField] private float noiseOffsetB = 23.4f;
        [SerializeField] private float minAmplitude = 0.08f;
        [SerializeField] private float minDuration = 0.08f;
        
        private float shakeTimer;
        private float shakeDuration;
        private float amplitude;

        private Vector3 initialLocalPos;

        private float phase;
        private float seedX;
        private float seedY;

        private void Awake()
        {
            initialLocalPos = transform.localPosition;
            seedX = Random.value * 1000f;
            seedY = Random.value * 1000f;
            Services.Register<ICameraShake>(this);
        }

        private void LateUpdate()
        {
            Vector3 pos = initialLocalPos;

            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;

                float normalized = 1f - (shakeTimer / shakeDuration);
                float damper = 1f - normalized;

                phase += Time.deltaTime * frequency;

                float noiseX = (Mathf.PerlinNoise(seedX + phase, seedY + noiseOffsetA) - 0.5f) * 2f;
                float noiseY = (Mathf.PerlinNoise(seedX + noiseOffsetB, seedY + phase) - 0.5f) * 2f;

                pos += new Vector3(noiseX, noiseY, 0f) * (amplitude * damper);
            }

            transform.localPosition = pos;
        }

        public void Shake(float amount)
        {
            float amountClamped = Mathf.Clamp01(amount);

            amplitude     = Mathf.Lerp(minAmplitude, maxAmplitude, amountClamped);
            shakeDuration = Mathf.Lerp(minDuration, baseDuration, amountClamped);

            shakeTimer = shakeDuration;
            phase = Random.value * 1000f;
        }
    }
}