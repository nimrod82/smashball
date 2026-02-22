using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class CameraPoseController : MonoBehaviour
    {
        [SerializeField] private Transform cameraChild;
        [SerializeField] private float transitionSeconds = 0.35f;

        [Header("Gameplay Pose (Rig space)")]
        [SerializeField] private Vector3 gameplayLocalPos = new(0f, 12.99f, -9.72f);
        [SerializeField] private Vector3 gameplayLocalEuler = new(60f, 0f, 0f);

        [Header("Serve Pose (Rig space)")]
        [SerializeField] private Vector3 serveLocalPos = new(0f, 5f, -9.72f);
        [SerializeField] private Vector3 serveLocalEuler = new(30f, 0f, 0f);

        private CancellationTokenSource cts;

        private void Awake()
        {
            if (cameraChild == null)
                cameraChild = Camera.main != null ? Camera.main.transform : null;

            ApplyPoseInstant(gameplayLocalPos, gameplayLocalEuler);
        }

        public Task ToGameplayAsync() => LerpPoseAsync(gameplayLocalPos, gameplayLocalEuler);
        public Task ToServeAsync() => LerpPoseAsync(serveLocalPos, serveLocalEuler);

        private void ApplyPoseInstant(Vector3 localPos, Vector3 localEuler)
        {
            transform.localPosition = localPos;
            transform.localRotation = Quaternion.Euler(localEuler);
        }

        private async Task LerpPoseAsync(Vector3 targetPos, Vector3 targetEuler)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;

            Quaternion targetRot = Quaternion.Euler(targetEuler);

            float t = 0f;
            float inv = transitionSeconds > 0.0001f ? 1f / transitionSeconds : 9999f;

            try
            {
                while (t < 1f)
                {
                    if (token.IsCancellationRequested)
                        return;

                    t += Time.deltaTime * inv;
                    float u = Mathf.Clamp01(t);

                    transform.localPosition = Vector3.Lerp(startPos, targetPos, u);
                    transform.localRotation = Quaternion.Slerp(startRot, targetRot, u);

                    await Task.Yield();
                }

                transform.localPosition = targetPos;
                transform.localRotation = targetRot;
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }
    }
}