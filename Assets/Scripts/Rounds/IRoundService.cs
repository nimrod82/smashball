using UnityEngine;

namespace Smashball.Gameplay
{
    public interface IRoundService
    {
        BallController CurrentBall { get; }
        void OnPlayerHitByBall(PlayerController player);
        PlayerController GetOtherPlayer(PlayerController player);
    }
}