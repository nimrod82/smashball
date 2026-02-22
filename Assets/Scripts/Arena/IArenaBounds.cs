using UnityEngine;

namespace Smashball.Gameplay
{
    public interface IArenaBounds
    {
        Bounds GetBounds();
        Vector3 ClampToHalf(Vector3 pos, bool isTopPlayer, float padding);
        Vector3 TopPlayerSpawnPosition { get; }
        Vector3 BottomPlayerSpawnPosition { get; }
    }
}