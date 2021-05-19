using UnityEngine;

namespace NewVersion.Ship
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private float velocity;
        private Rigidbody2D rb;
        private IShip ship;
        private bool isConfigurated = false;

        public void Configure(IShip ship)
        {
            this.ship = ship;
            rb = GetComponent<Rigidbody2D>();
            isConfigurated = true;
        }

        public void MovePointTo(Vector2 worldPosition)
        {
            rb.velocity = worldPosition * velocity;
            ship.IsMovement();
        }
    }
}
