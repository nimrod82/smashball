using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class BallController : MonoBehaviour
    {
        [SerializeField] private SphereCollider sphereCollider;
        [SerializeField] private float turnRateDegPerSec = 180f;
        [SerializeField] private float minSpeed = 8f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float yOffset = 1.5f;
        [SerializeField] private float boundsPadding = 1f;
        [SerializeField] private float collisionGraceSeconds = 0.12f;
        [SerializeField] private BallTrailFX trailFx;
        
        public float BallRadius => sphereCollider.radius;
        public PlayerController LastStriker { get; private set; }
        public float IgnorePlayerCollisionUntilTime { get; private set; }
        public Vector3 PreviousPosition { get; private set; }
        
        private IArenaBounds arenaBounds;
        private Vector3 velocity;
        private PlayerController targetPlayer;

        public void Init(IArenaBounds bounds)
        {
            arenaBounds = bounds;
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            PreviousPosition = transform.position;
            Vector3 prevPos = transform.position;
            IntegrateWithBounce(prevPos, velocity, dt, out var nextPos, out var nextVel);

            transform.position = nextPos;
            velocity = nextVel;
            
            ApplyHoming(dt);
        }
        
        private bool IntegrateWithBounce(Vector3 prevPos, Vector3 vel, float dt, out Vector3 newPos, out Vector3 newVel)
        {
            newPos = prevPos;
            newVel = vel;

            var b = arenaBounds.GetBounds();
            var minX = b.min.x + boundsPadding;
            var maxX = b.max.x - boundsPadding;
            var minZ = b.min.z + boundsPadding;
            var maxZ = b.max.z - boundsPadding;

            bool bounced = false;
            float remaining = dt;

            // Handle up to 2 bounces per physics step to support corner hits.
            for (int iter = 0; iter < 2; iter++)
            {
                Vector3 target = newPos + newVel * remaining;

                float tHit = 1f;
                Vector3 normal = Vector3.zero;

                if (newVel.x > 0f && target.x > maxX)
                {
                    float t = (maxX - newPos.x) / (newVel.x * remaining);
                    if (t >= 0f && t < tHit) { tHit = t; normal = Vector3.left; }
                }
                else if (newVel.x < 0f && target.x < minX)
                {
                    float t = (minX - newPos.x) / (newVel.x * remaining);
                    if (t >= 0f && t < tHit) { tHit = t; normal = Vector3.right; }
                }

                if (newVel.z > 0f && target.z > maxZ)
                {
                    float t = (maxZ - newPos.z) / (newVel.z * remaining);
                    if (t >= 0f && t < tHit) { tHit = t; normal = Vector3.back; }
                }
                else if (newVel.z < 0f && target.z < minZ)
                {
                    float t = (minZ - newPos.z) / (newVel.z * remaining);
                    if (t >= 0f && t < tHit) { tHit = t; normal = Vector3.forward; }
                }

                if (normal == Vector3.zero)
                {
                    newPos = target;
                    break;
                }

                float dtToHit = remaining * tHit;
                newPos += newVel * dtToHit;

                newVel = Vector3.Reflect(newVel, normal);
                bounced = true;

                remaining -= dtToHit;

                newPos += normal * 0.0005f;

                if (remaining <= 0f)
                    break;
            }

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
            
            return bounced;
        }
        
        private void ApplyHoming(float dt)
        {
            if (targetPlayer == null)
                return;
            
            Vector3 aimPos = targetPlayer.transform.position;

            Vector3 toTarget = aimPos - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.0001f) return;

            Vector3 desiredDir = toTarget.normalized;

            Vector3 v = velocity;
            v.y = 0f;
            float speed = v.magnitude;
            if (speed < 0.001f) return;

            Vector3 currentDir = v / speed;

            float maxRadians = turnRateDegPerSec * Mathf.Deg2Rad * dt;
            Vector3 newDir = Vector3.RotateTowards(currentDir, desiredDir, maxRadians, 0f);

            velocity = newDir * speed;
        }

        public void ApplyStrike(Vector3 direction, float quality, PlayerController target, PlayerController striker)
        {
            LastStriker = striker;
            targetPlayer = target;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector3.forward;
            else
                direction.Normalize();

            float q = Mathf.Clamp01(quality);
            float speed = Mathf.Lerp(minSpeed, maxSpeed, q);
            velocity = direction * speed;
            IgnorePlayerCollisionUntilTime = Time.time + collisionGraceSeconds;
            trailFx?.SetIntensityFromQuality(q);
        }

        public void SetPosition(Vector3 position)
        {
            position.y = yOffset;
            transform.position = position;
            PreviousPosition = position;
        }
    }
}