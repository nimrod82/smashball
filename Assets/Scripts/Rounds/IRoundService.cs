using System.Threading.Tasks;
using UnityEngine;

namespace Smashball.Gameplay
{
    public interface IRoundService
    {
        Color PlayerColor { get; }
        Color OpponentColor { get; }
        RoundState State { get; }
        float ServeQuality { get; }
        BallController CurrentBall { get; }
        void OnServed();
        Task OnPlayerHitByBall(PlayerController player);
        PlayerController GetOtherPlayer(PlayerController player);
        bool IsStartingPlayer(PlayerController playerController);
        void StartGame();
    }
}