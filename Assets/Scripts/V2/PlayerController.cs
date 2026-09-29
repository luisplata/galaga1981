using UnityEngine;

namespace V2
{
    /// <summary>
    /// Movimiento relativo 1:1: x = shipStartWorld.x + dragDeltaWorld.x.
    /// El offset vertical queda fijo (el dedo nunca tapa la nave) y sin inercia al soltar.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float minX = -4.8f;
        [SerializeField] private float maxX = 4.8f;
        [SerializeField] private float fixedY = -7.5f;
        [SerializeField] private InputDrag inputDrag;
        [SerializeField] private Camera gameCamera;

        private Vector3 shipStartWorld;
        private bool dragging;

        private void OnEnable()
        {
            if (inputDrag != null)
            {
                inputDrag.DragStarted += OnDragStarted;
                inputDrag.DragEnded += OnDragEnded;
            }
        }

        private void OnDisable()
        {
            if (inputDrag != null)
            {
                inputDrag.DragStarted -= OnDragStarted;
                inputDrag.DragEnded -= OnDragEnded;
            }
        }

        private void OnDragStarted()
        {
            dragging = true;
            shipStartWorld = transform.position;
        }

        private void OnDragEnded()
        {
            dragging = false;
        }

        private void Update()
        {
            if (!dragging || inputDrag == null) return;

            Camera cam = gameCamera != null ? gameCamera : Camera.main;
            if (cam == null) return;

            Vector3 origin = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 10f));
            Vector3 worldPoint = cam.ScreenToWorldPoint(
                new Vector3(inputDrag.DragScreenDelta.x, inputDrag.DragScreenDelta.y, 10f));
            Vector3 deltaWorld = worldPoint - origin;

            float x = Mathf.Clamp(shipStartWorld.x + deltaWorld.x, minX, maxX);
            transform.position = new Vector3(x, fixedY, 0f);
        }

        public void ResetPosition()
        {
            transform.position = new Vector3(0f, fixedY, 0f);
        }
    }
}