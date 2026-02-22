using Smashball.Input;
using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class HumanPlayerInput : IPlayerInput
    {
        private readonly IInputService input;

        public HumanPlayerInput(IInputService input)
        {
            this.input = input;
        }
        public Vector2 Move => input.Move;
        public bool ReleasedThisFrame => input.ReleasedThisFrame;
    }
}