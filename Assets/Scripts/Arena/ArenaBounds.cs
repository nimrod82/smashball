using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class ArenaBounds : MonoBehaviour, IArenaBounds
    {
        public enum SplitAxis { X, Z }

        [SerializeField] private BoxCollider courtBounds;
        [SerializeField] private SplitAxis splitAxis = SplitAxis.Z;

        public Bounds Court => courtBounds.bounds;
        
        private void Awake()
        {
            Services.Register<IArenaBounds>(this);
        }

        public Vector3 ClampToHalf(Vector3 pos, bool isPositiveSide, float padding)
        {
            var b = Court;

            float minX = b.min.x + padding;
            float maxX = b.max.x - padding;
            float minZ = b.min.z + padding;
            float maxZ = b.max.z - padding;

            float midX = (b.min.x + b.max.x) * 0.5f;
            float midZ = (b.min.z + b.max.z) * 0.5f;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

            if (splitAxis == SplitAxis.Z)
            {
                if (isPositiveSide)
                    pos.z = Mathf.Clamp(pos.z, midZ + padding, maxZ);
                else
                    pos.z = Mathf.Clamp(pos.z, minZ, midZ - padding);
            }
            else
            {
                if (isPositiveSide)
                    pos.x = Mathf.Clamp(pos.x, midX + padding, maxX);
                else
                    pos.x = Mathf.Clamp(pos.x, minX, midX - padding);
            }

            return pos;
        }
    }
}