using UnityEngine;

namespace Smashball.Input
{
    public interface IInputService
    {
        Vector2 Move { get; }
        bool ReleasedThisFrame { get; }

        void SetJoystickMove(Vector2 v);
        void NotifyJoystickReleased();
    }
}