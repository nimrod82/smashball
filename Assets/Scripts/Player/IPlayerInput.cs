using UnityEngine;

namespace Smashball.Gameplay
{
    public interface IPlayerInput
    {
        Vector2 Move { get; }
        bool ReleasedThisFrame { get; }
    }
}