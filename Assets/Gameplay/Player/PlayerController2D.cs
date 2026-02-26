using UnityEngine;

namespace Gameplay.Player
{
    public sealed class PlayerController2D : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 4f;

        private void Update()
        {
            float x = UnityEngine.Input.GetAxisRaw("Horizontal");
            float y = UnityEngine.Input.GetAxisRaw("Vertical");
            var dir = new Vector2(x, y).normalized;

            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }
    }
}