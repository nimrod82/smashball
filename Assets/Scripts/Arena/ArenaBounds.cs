using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class ArenaBounds : MonoBehaviour, IArenaBounds
    {
        [SerializeField] private BoxCollider courtBounds;
        [SerializeField] private Transform bottomPlayerSpawn;
        [SerializeField] private Transform topPlayerSpawn;

        public Vector3 TopPlayerSpawnPosition => topPlayerSpawn.position;
        public Vector3 BottomPlayerSpawnPosition => bottomPlayerSpawn.position;
        
        private void Awake()
        {
            Services.Register<IArenaBounds>(this);
        }

        public Bounds GetBounds()
        {
            return courtBounds.bounds;
        }
        
        public Vector3 Clamp(Vector3 pos, float padding)
        {
            var bounds = courtBounds.bounds;

            float minX = bounds.min.x + padding;
            float maxX = bounds.max.x - padding;
            float minZ = bounds.min.z + padding;
            float maxZ = bounds.max.z - padding;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

            return pos;
        }

        public Vector3 ClampToHalf(Vector3 pos, bool isTopPlayer, float padding)
        {
            var bounds = courtBounds.bounds;

            float minX = bounds.min.x + padding;
            float maxX = bounds.max.x - padding;
            float minZ = bounds.min.z + padding;
            float maxZ = bounds.max.z - padding;

            float midZ = (bounds.min.z + bounds.max.z) * 0.5f;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

            if (isTopPlayer)
                pos.z = Mathf.Clamp(pos.z, midZ + padding, maxZ);
            else
                pos.z = Mathf.Clamp(pos.z, minZ, midZ - padding);

            return pos;
        }
    }
}