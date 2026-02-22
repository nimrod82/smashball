using UnityEngine;
using UnityEngine.InputSystem;

namespace Smashball.Input
{
    public sealed class InputService : MonoBehaviour, IInputService
    {
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private float heldThreshold = 0.15f;

        public Vector2 Move { get; private set; }
        public bool IsHeld { get; private set; }
        public bool ReleasedThisFrame { get; private set; }

        private bool wasHeld;

        private Vector2 joystickMove;
        private bool joystickHeld;
        private bool joystickReleasedThisFrame;

        private void Awake()
        {
            Services.Register<IInputService>(this);
        }

        private void OnEnable()
        {
            if (moveAction != null)
                moveAction.action.Enable();
        }

        private void OnDisable()
        {
            if (moveAction != null)
                moveAction.action.Disable();
        }

        private void Update()
        {
            ReleasedThisFrame = false;

            Vector2 v = joystickHeld ? joystickMove : ReadActionMove();
            bool held = v.magnitude >= heldThreshold;

            if (joystickReleasedThisFrame)
            {
                ReleasedThisFrame = true;
                v = Vector2.zero;
                held = false;
            }
            else if (wasHeld && !held)
            {
                ReleasedThisFrame = true;
            }

            Move = v;
            IsHeld = held;
            wasHeld = held;

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

        private Vector2 ReadActionMove()
        {
            return moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        }
    }
}