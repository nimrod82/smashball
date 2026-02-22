using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 3f;
        [SerializeField] private float xLimit = 2f;

        private Vector3 initialPosition;
        private bool enabledFollow = true;
        private void Awake()
        {
            initialPosition = transform.position;
        }
        
        private void LateUpdate()
        {
            if (!enabledFollow || target == null)
                return;

            float targetX = Mathf.Clamp(target.position.x, -xLimit, xLimit);

            Vector3 p = transform.position;
            p.x = Mathf.Lerp(p.x, targetX, smoothSpeed * Time.deltaTime);
            transform.position = p;
        }
        
        public void SetEnabled(bool enabled) => enabledFollow = enabled;

        public void SetTarget(Transform t)
        {
            target = t;
        }
    }
}