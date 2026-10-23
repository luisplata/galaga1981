using System.Collections;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Haz tractor del Boss (M2): telegraph 0.8 s → activo 2 s → off.
    /// Cono por matemática en world space (rect): el Player no tiene Rigidbody2D y dos
    /// colliders estáticos no emiten triggers, así que no hay física de por medio.
    /// El dueño (Enemy carrier) decide la captura; este componente sólo detecta y avisa.
    /// </summary>
    public class TractorBeam : MonoBehaviour
    {
        [SerializeField] private float telegraphDuration = 0.8f;
        [SerializeField] private float activeDuration = 2f;
        [SerializeField] private float coneWidth = 1.8f;
        [SerializeField] private float extraReach = 1.5f;

        private const float TelegraphAlphaMin = 0.05f;
        private const float TelegraphAlphaMax = 0.45f;
        private const float ActiveAlpha = 0.8f;

        private Enemy owner;
        private SpriteRenderer spriteRenderer;
        private Coroutine cycle;
        private bool beaming;
        private float currentHeight;

        /// <summary>Ciclo en curso (telegraph + activo).</summary>
        public bool IsActive { get; private set; }

        private void Awake()
        {
            owner = GetComponentInParent<Enemy>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetRenderer(false);
        }

        /// <summary>Arranca un ciclo completo: telegraph visible y luego captura durante 2 s.</summary>
        public void BeginCycle()
        {
            if (cycle != null) StopCoroutine(cycle);
            cycle = StartCoroutine(Cycle());
        }

        /// <summary>Corta el ciclo (captura, muerte del carrier, reset o disable).</summary>
        public void Cut()
        {
            if (cycle != null)
            {
                StopCoroutine(cycle);
                cycle = null;
            }
            EndCycle();
        }

        private IEnumerator Cycle()
        {
            IsActive = true;
            beaming = false;
            SetRenderer(true);

            // Telegraph: alpha pulsante, sin captura.
            float t = 0f;
            while (t < telegraphDuration)
            {
                float pulse = Mathf.Lerp(TelegraphAlphaMin, TelegraphAlphaMax,
                    Mathf.PingPong(Time.time * 6f, 1f));
                SetAlpha(pulse);
                t += Time.deltaTime;
                yield return null;
            }

            // Activo: geometría calculada contra el jugador y captura si entra al cono.
            beaming = true;
            SetAlpha(ActiveAlpha);
            t = 0f;
            while (t < activeDuration)
            {
                UpdateGeometry();
                if (TryCapture())
                {
                    EndCycle();
                    cycle = null;
                    yield break;
                }
                t += Time.deltaTime;
                yield return null;
            }

            EndCycle();
            cycle = null;
        }

        /// <summary>
        /// h = ownerY - playerY + extraReach; el rect cuelga del centro del Boss (pivote centro).
        /// El alcance se calcula porque 4.5 u fijas nunca llegan al jugador con cámara orto 9.6.
        /// </summary>
        private void UpdateGeometry()
        {
            float ownerY = owner != null ? owner.transform.position.y : transform.parent.position.y;
            Transform player = owner != null ? owner.PlayerTransform : null;
            float playerY = player != null ? player.position.y : ownerY - 10f;
            currentHeight = Mathf.Max(1f, ownerY - playerY + extraReach);

            transform.localScale = new Vector3(coneWidth, currentHeight, 1f);
            transform.localPosition = new Vector3(0f, -currentHeight * 0.5f, 0f);
        }

        private bool TryCapture()
        {
            if (!beaming || owner == null) return false;
            Transform player = owner.PlayerTransform;
            if (player == null) return false;

            Vector3 o = owner.transform.position;
            Vector3 p = player.position;
            bool inside = Mathf.Abs(p.x - o.x) <= coneWidth * 0.5f
                          && p.y <= o.y
                          && p.y >= o.y - currentHeight;
            if (!inside) return false;

            owner.OnBeamHitPlayer();
            return true;
        }

        private void SetRenderer(bool on)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = on;
        }

        private void SetAlpha(float alpha)
        {
            if (spriteRenderer == null) return;
            Color c = Color.white;
            c.a = alpha;
            spriteRenderer.color = c;
        }

        private void EndCycle()
        {
            beaming = false;
            IsActive = false;
            SetRenderer(false);
        }

        private void OnDisable()
        {
            Cut();
        }
    }
}
