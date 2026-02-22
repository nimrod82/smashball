using UnityEngine;

namespace Smashball.Gameplay
{
    public interface IArenaBounds
    {
        Vector3 ClampToHalf(Vector3 pos, bool isPositiveSide, float padding);
    }
}