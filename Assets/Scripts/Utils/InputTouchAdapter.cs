using UnityEngine;
using NewVersion.Ship;

namespace Utils
{
    public class InputTouchAdapter : IInputAdapter
    {
        private Camera camera;
        private GameObject pointToFollow;
        private GameObject ship;
        private Vector2 lastPosition;
        private readonly ShipControllerMediator _shipControllerMediator;

        public InputTouchAdapter(Camera camera, GameObject pointToFollow, GameObject ship)
        {
            this.camera = camera;
            this.pointToFollow = pointToFollow;
            this.ship = ship;
        }

        public InputTouchAdapter(Camera camera, ShipControllerMediator shipControllerMediator)
        {
            this.camera = camera;
            _shipControllerMediator = shipControllerMediator;
            pointToFollow = shipControllerMediator.PointToFollow;
            ship = shipControllerMediator.gameObject;
        }

        public Vector2 GetDirection()
        {
            //if (!_shipControllerMediator.IsFiring()) return Vector2.zero;
            lastPosition = camera.ScreenToWorldPoint(_shipControllerMediator.GetMousePosition());
            pointToFollow.transform.position = lastPosition;
            var diff = pointToFollow.transform.position - ship.transform.position;
            return diff.normalized;
        }

        public bool GetButton(string name)
        {
            return true;
        }

        public bool CanMove()
        {
            return Vector2.Distance(_shipControllerMediator.PointToFollow.transform.position, ship.transform.position) >= 0.1f; 
        }
    }
}