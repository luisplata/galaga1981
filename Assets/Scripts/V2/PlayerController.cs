using UnityEngine;

namespace V2
{
    /// <summary>
    /// Movimiento relativo 1:1: x = shipStartWorld.x + dragDeltaWorld.x.
    /// El offset vertical queda fijo (el dedo nunca tapa la nave) y sin inercia al soltar.
    /// M3: blink de i-frames por contador (sin coroutine) sobre la visibilidad del sprite.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float minX = -4.8f;
        [SerializeField] private float maxX = 4.8f;
        [SerializeField] private float fixedY = -7.5f;
        [SerializeField] private float resetX = 0f;
        [SerializeField] private float blinkInterval = 0.06f; // ~8.3 Hz (tunable)
        [SerializeField] private InputDrag inputDrag;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D bodyCollider;

        private Vector3 shipStartWorld;
        private bool dragging;
        private bool captured;

        private bool visible = true;
        private bool blinkOn = true;
        private float blinkUntil;
        private float nextBlink;

        private void Awake()
        {
            // Fallback defensivo: el prefab los wirea, pero la instancia puede quedar sin refs.
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();
        }

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
            if (captured) return; // capturado: InputDrag puede reactivar el arrastre
            dragging = true;
            shipStartWorld = transform.position;
        }

        private void OnDragEnded()
        {
            dragging = false;
        }

        private void Update()
        {
            // El blink corre aun sin arrastre; sólo toca el renderer/collider.
            TickBlink();

            if (captured || !dragging || inputDrag == null) return;

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
            transform.position = new Vector3(resetX, fixedY, 0f);
        }

        /// <summary>Arranca el parpadeo de i-frames (se llama al respawnear).</summary>
        public void BeginInvulnerable(float seconds)
        {
            blinkUntil = Time.time + Mathf.Max(0f, seconds);
            nextBlink = Time.time + blinkInterval;
            blinkOn = true;
            Apply();
        }

        /// <summary>Captura: la nave desaparece y el arrastre queda bloqueado hasta el respawn.</summary>
        public void Capture()
        {
            CancelBlink();
            captured = true;
            dragging = false;
            SetVisible(false);
        }

        /// <summary>Respawn tras la captura/impacto: vuelve al centro visible y habilitado.</summary>
        public void Respawn()
        {
            ResetPosition();
            captured = false;
            SetVisible(true);
        }

        /// <summary>Reset de run (StartGame): posición inicial, nave visible y blink cancelado.</summary>
        public void ResetForRun()
        {
            CancelBlink();
            ResetPosition();
            captured = false;
            dragging = false;
            SetVisible(true);
        }

        /// <summary>GameOver: nave oculta con estado definido (sin blink colgado).</summary>
        public void HideForGameOver()
        {
            CancelBlink();
            captured = true;
            dragging = false;
            SetVisible(false);
        }

        public void SetVisible(bool value)
        {
            visible = value;
            Apply();
        }

        private void TickBlink()
        {
            if (blinkUntil <= 0f) return;

            if (Time.time >= blinkUntil)
            {
                blinkUntil = 0f;
                blinkOn = true;
                Apply();
                return;
            }

            if (Time.time >= nextBlink)
            {
                blinkOn = !blinkOn;
                nextBlink = Time.time + blinkInterval;
                Apply();
            }
        }

        private void CancelBlink()
        {
            blinkUntil = 0f;
            blinkOn = true;
        }

        private void Apply()
        {
            if (spriteRenderer != null) spriteRenderer.enabled = visible && blinkOn;
            if (bodyCollider != null) bodyCollider.enabled = visible;
        }
    }
}
