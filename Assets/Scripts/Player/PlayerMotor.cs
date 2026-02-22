using Smashball.Input;
using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private bool isTopPlayer;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotateSpeed = 16f;
        [SerializeField] private float boundsPadding = 1f;
        
        private IInputService input;
        private IArenaBounds bounds;
  
        private void Start()
        {
            input = Services.Get<IInputService>();
            bounds = Services.Get<IArenaBounds>();
        }

        private void Update()
        {
            Vector2 move = input.Move;

            Vector3 dir = new Vector3(move.x, 0f, move.y);
            if (dir.sqrMagnitude > 1f)
                dir.Normalize();

            Vector3 nextPos = transform.position + moveSpeed * Time.deltaTime * dir;
            nextPos = bounds.ClampToHalf(nextPos, isTopPlayer, boundsPadding);

            transform.position = nextPos;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
        }
    }
}