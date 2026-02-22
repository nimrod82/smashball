using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class BotController : MonoBehaviour
    {
        private BotInput input;
        private IArenaBounds bounds;
        private IRoundService roundManager;

        public IPlayerInput Input => input;

        public void Init()
        {
            bounds = Services.Get<IArenaBounds>();
            roundManager = Services.Get<IRoundService>();
            input = new BotInput();
        }

        private void Update()
        {
            input.BeginFrame();
            
            Vector3 botPos = transform.position;
            Vector3 ballPos = roundManager.CurrentBall.transform.position;
            
            Vector3 toBall = ballPos - botPos;
            toBall.y = 0f;
            
            input.Release();
        }
    }
}