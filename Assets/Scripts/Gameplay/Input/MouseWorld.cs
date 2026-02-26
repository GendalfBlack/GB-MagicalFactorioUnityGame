using UnityEngine;

namespace Gameplay.Input
{
    public static class MouseWorld
    {
        public static Vector2 GetWorld2D(Camera cam)
        {
            var p = cam.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            return new Vector2(p.x, p.y);
        }
    }
}
