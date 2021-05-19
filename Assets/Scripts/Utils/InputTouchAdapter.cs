using UnityEngine;

namespace Utils
{
    public class InputTouchAdapter : IInputAdapter
    {
        private Camera camera;
        private GameObject pointToFollow;
        private GameObject ship;
        private Vector2 lastPosition;

        public InputTouchAdapter(Camera camera, GameObject pointToFollow, GameObject ship)
        {
            this.camera = camera;
            this.pointToFollow = pointToFollow;
            this.ship = ship;
        }
    
        public Vector2 GetDirection()
        {
            if (!Input.GetButton("Fire") && !Input.GetMouseButton(0)) return Vector2.zero;
            lastPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            pointToFollow.transform.position = lastPosition;
            var diff = pointToFollow.transform.position - ship.transform.position;
            return diff;
        }

        public bool GetButton(string name)
        {
            return true;
        }
    }
}