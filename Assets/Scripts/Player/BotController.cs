using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class BotController : MonoBehaviour
    {
        [SerializeField] private float minServeDelay = 0.4f;
        [SerializeField] private float maxServeDelay = 1.2f;

        private float serveDelayTimer;
        private float currentServeDelay;
        private bool waitingServe;
        private BotInput input;
        private IRoundService roundManager;
        private PlayerController playerController;

        public IPlayerInput Input => input;

        public void Init()
        {
            roundManager = Services.Get<IRoundService>();
            playerController = GetComponent<PlayerController>();
            input = new BotInput();
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
                    UpdateMovementAndStrike();
                    break;
            }
        }

        private void UpdateMovementAndStrike()
        {
            input.BeginFrame();
            input.Release();
        }
        
        private void UpdateServe()
        {
            input.BeginFrame();

            if (!roundManager.IsStartingPlayer(playerController))
                return;

            if (!waitingServe)
            {
                waitingServe = true;
                serveDelayTimer = 0f;
                currentServeDelay = Random.Range(minServeDelay, maxServeDelay);
            }

            serveDelayTimer += Time.deltaTime;

            if (serveDelayTimer >= currentServeDelay)
            {
                input.Release();
                waitingServe = false;
            }
        }
    }
}