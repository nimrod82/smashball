using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject rangeFeedback;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotateSpeed = 16f;
        [SerializeField] private float boundsPadding = 1f;
        [SerializeField] private float strikeRadius = 3f;
        [SerializeField] private float toleranceRadius = 1f;
        
        private IPlayerInput input;
        private IArenaBounds arenaBounds;
        private IRoundService roundManager;
        private bool isTopPlayer;
        
        public void Init(bool isTopPlayer, IPlayerInput input)
        {
            rangeFeedback.SetActive(!isTopPlayer);
            rangeFeedback.transform.localScale = 2f * new Vector3(strikeRadius, strikeRadius, 1f);
            this.isTopPlayer = isTopPlayer;
            this.input = input;
            arenaBounds = Services.Get<IArenaBounds>();
            roundManager = Services.Get<IRoundService>();
            transform.position = isTopPlayer ? arenaBounds.TopPlayerSpawnPosition : arenaBounds.BottomPlayerSpawnPosition;
        }

        private void Update()
        {
            UpdateMovement();
            UpdateStrike();
        }

        private void UpdateMovement()
        {
            Vector2 move = input.Move;

            Vector3 dir = new Vector3(move.x, 0f, move.y);
            if (dir.sqrMagnitude > 1f)
                dir.Normalize();

            Vector3 nextPos = transform.position + moveSpeed * Time.deltaTime * dir;
            nextPos = arenaBounds.ClampToHalf(nextPos, isTopPlayer, boundsPadding);

            transform.position = nextPos;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
        }

        private void UpdateStrike()
        {
            if (!input.ReleasedThisFrame) return;

            var ball = roundManager.CurrentBall;
            if (ball == null) return;

            Vector3 playerPos = transform.position;
            Vector3 ballPos = ball.transform.position;

            Vector3 toBall = ballPos - playerPos;
            float dist = toBall.magnitude;
            if (dist > strikeRadius + toleranceRadius) return;

            float quality = ComputeQuality(dist, strikeRadius, toleranceRadius);

            toBall.y = 0f;
            Vector3 dir = toBall.sqrMagnitude < 0.0001f ? transform.forward : toBall.normalized;

            ball.ApplyStrike(dir, quality, roundManager.GetOtherPlayer(this));
        }
        
        private static float ComputeQuality(float dist, float perfectR, float tolerance)
        {
            float error = Mathf.Abs(dist - perfectR);
            return Mathf.Clamp01(1f - error / tolerance);
        }

        public void StartServing()
        {
            roundManager.CurrentBall.SetPosition(transform.position);
            roundManager.CurrentBall.gameObject.SetActive(true);
            roundManager.CurrentBall.ApplyStrike(Vector3.forward, 1f, roundManager.GetOtherPlayer(this));
        }
    }
}