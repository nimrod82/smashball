using UnityEngine;

namespace Smashball.Input
{
    public sealed class InputService : MonoBehaviour, IInputService
    {
        public Vector2 Move { get; private set; }
        public bool ReleasedThisFrame { get; private set; }

        private bool wasHeld;

        private Vector2 joystickMove;
        private bool joystickHeld;
        private bool joystickReleasedThisFrame;

        private void Awake()
        {
            Services.Register<IInputService>(this);
        }

        private void Update()
        {
            ReleasedThisFrame = false;

            Vector2 v = joystickMove;

            if (joystickReleasedThisFrame)
            {
                ReleasedThisFrame = true;
                v = Vector2.zero;
            }
            else if (wasHeld && !joystickHeld)
            {
                ReleasedThisFrame = true;
            }

            Move = v;
            wasHeld = joystickHeld;

            joystickReleasedThisFrame = false;
        }

        public void SetJoystickMove(Vector2 v)
        {
            joystickMove = v;
            joystickHeld = true;
        }

        public void NotifyJoystickReleased()
        {
            joystickMove = Vector2.zero;
            joystickHeld = false;
            joystickReleasedThisFrame = true;
        }
    }
}