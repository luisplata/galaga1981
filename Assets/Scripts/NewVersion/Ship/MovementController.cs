using UnityEngine;

namespace NewVersion.Ship
{
    public class MovementController : MonoBehaviour
    {
        private Vector2 velocity;
        private Rigidbody2D rb;
        private IShip ship;

        public void Configure(IShip ship, Vector2 speed)
        {
            this.ship = ship;
            rb = GetComponent<Rigidbody2D>();
            velocity = speed;
        }

        public void MovePointTo(Vector2 worldPosition)
        {
            rb.velocity = worldPosition * velocity * Time.deltaTime;
            ship.IsMovement();
        }
    }
}
