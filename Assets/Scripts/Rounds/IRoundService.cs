using UnityEngine;

namespace Smashball.Gameplay
{
    public interface IRoundService
    {
        RoundState State { get; }
        float ServeQuality { get; }
        BallController CurrentBall { get; }
        void OnServed();
        void OnPlayerHitByBall(PlayerController player);
        PlayerController GetOtherPlayer(PlayerController player);
        bool IsStartingPlayer(PlayerController playerController);
    }
}