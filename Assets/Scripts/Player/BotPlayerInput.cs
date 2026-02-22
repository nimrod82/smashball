using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class BotInput : IPlayerInput
    {
        public Vector2 Move { get; private set; }
        public bool ReleasedThisFrame { get; private set; }

        public void SetMove(Vector2 v)
        {
            Move = v;
        }

        public void Release()
        {
            ReleasedThisFrame = true;
            Move = Vector2.zero;
        }

        public void BeginFrame()
        {
            ReleasedThisFrame = false;
        }
    }
}