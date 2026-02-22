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
        [SerializeField] private float strikeCooldownSeconds = 0.2f;
        
        private IPlayerInput input;
        private IArenaBounds arenaBounds;
        private IRoundService roundManager;
        private bool isTopPlayer;
        private float nextReleaseAllowedTime;
        
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
            switch (roundManager.State)
            {
                case RoundState.Menu:
                    break;
                case RoundState.Serving:
                    UpdateServe();
                    break;
                case RoundState.Playing:
                    UpdateMovement();
                    UpdateStrike();
                    break;
            }
        }
        
        private bool CanConsumeRelease()
        {
            if (Time.time < nextReleaseAllowedTime)
                return false;

            if (!input.ReleasedThisFrame)
                return false;

            nextReleaseAllowedTime = Time.time + strikeCooldownSeconds;
            return true;
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
            if (!CanConsumeRelease())
                return;
            
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

        private void UpdateServe()
        {
            if (!roundManager.IsStartingPlayer(this))
                return;

            if (!input.ReleasedThisFrame)
                return;

            var otherPlayer = roundManager.GetOtherPlayer(this);
            var ball = roundManager.CurrentBall;

            ball.SetPosition(transform.position);
            ball.gameObject.SetActive(true);

            var dir = (otherPlayer.transform.position - transform.position).normalized;
            float serveQuality = roundManager.ServeQuality;

            roundManager.OnServed();
            ball.ApplyStrike(dir, serveQuality, otherPlayer);
            nextReleaseAllowedTime = Time.time + strikeCooldownSeconds;
        }
    }
}