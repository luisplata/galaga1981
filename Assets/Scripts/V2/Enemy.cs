using UnityEngine;

namespace V2
{
    /// <summary>
    /// Tipos de enemigo de la formación (pack 02-datos / 04-contenido).
    /// </summary>
    public enum EnemyType { Boss, Guard, Grunt }

    /// <summary>
    /// Ciclo de estados M1: entra con curva (seno), queda en su slot,
    /// baja en picada hacia la zona del jugador y retorna. Al morir libera el slot.
    /// </summary>
    public enum EnemyState { Entering, Formation, Diving, Dead }

    /// <summary>
    /// Enemigo de formación M1. enum + switch (sin FSM por componente).
    /// Reemplaza conceptualmente a EnemySimple. Muerte: desactiva el GO (slot = hueco) y
    /// notifica a GameFlow → WaveManager para el conteo de la grilla.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        // Config por tipo — defaults del pack (02-datos): Boss 2HP 150/100, Guard/Grunt 1HP 100/50
        [SerializeField] private EnemyType type = EnemyType.Grunt;
        [SerializeField] private int hp = 1;
        [SerializeField] private int pointsMoving = 100;    // Boss: 150
        [SerializeField] private int pointsFormation = 50;  // Boss: 100

        // Entrada (patrón curvo seno del pack)
        [SerializeField] private float enterSpeed = 4f;
        [SerializeField] private float enterSwayAmplitude = 1.5f;
        [SerializeField] private float enterSwayFrequency = 2f;
        [SerializeField] private float arrivalDistance = 0.1f;

        // Picada — 2 fases rectas (baja a (playerX, diveTargetY) → retorna al slot)
        [SerializeField] private float diveSpeed = 7f;
        [SerializeField] private float diveTargetY = -6f;   // zona del jugador (sin colisión en M1)
        [SerializeField] private float returnSpeed = 6f;

        // Refs serializadas
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameFlow gameFlow; // wiring del kill: Enemy → GameFlow → WaveManager
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip explosionClip;
        [SerializeField] private SpriteRenderer spriteRenderer; // flash en hit sobreviviente (Boss)
        [SerializeField] private GameObject heldShip;           // nave retenida (solo Boss)
        [SerializeField] private Vector2 carrierHold = new(4.2f, 5f); // parking del carrier

        private static readonly Color HitFlashColor = new(1f, 0.3f, 0.3f);
        private const float HitFlashDuration = 0.08f;

        private int initialHp;
        private float enterTime;
        private float enterStartY;
        private float divePlayerX;
        private bool divingDown;
        private float hitFlashTimer;

        public EnemyState State { get; private set; }
        public EnemyType Type => type;
        public Vector2 SlotPosition { get; private set; }
        public bool IsAlive => State != EnemyState.Dead;

        /// <summary>Boss que retiene una nave capturada: no pica ni re-dispara tractor.</summary>
        public bool IsCarrier { get; private set; }

        /// <summary>Haz tractor del prefab (Boss); null en Guard/Grunt.</summary>
        public TractorBeam Beam { get; private set; }

        /// <summary>El player llega por GameFlow (no se toca InitRefs ni el prefab de escena).</summary>
        public Transform PlayerTransform => gameFlow != null ? gameFlow.PlayerTransform : null;

        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            Beam = GetComponentInChildren<TractorBeam>(true);
            initialHp = hp;
        }

        /// <summary>
        /// Asigna slot y arranca la entrada curva desde spawnPoint (Estado Entering).
        /// El GO puede estar inactivo: WaveManager lo activa escalonado y llama BeginEntry().
        /// </summary>
        public void AssignSlot(Vector2 slot, Vector2 spawnPoint)
        {
            SlotPosition = slot;
            enterStartY = spawnPoint.y;
            enterTime = Time.time;
            transform.position = spawnPoint;
            State = EnemyState.Entering;
        }

        /// <summary>
        /// Reinicia el timer de la curva al activar el GO (entrada escalonada de la ola).
        /// </summary>
        public void BeginEntry()
        {
            enterTime = Time.time;
        }

        /// <summary>
        /// Inicia la picada de 2 fases rectas hacia (playerX, diveTargetY) y retorno al slot.
        /// Solo desde Formation.
        /// </summary>
        public void StartDive(float playerX)
        {
            if (State != EnemyState.Formation || IsCarrier) return;
            divePlayerX = playerX;
            divingDown = true;
            State = EnemyState.Diving;
        }

        /// <summary>
        /// Re-uso para la próxima ola: restaura HP/flash y limpia el estado de carrier
        /// (el pool reutiliza instancias entre olas y niveles).
        /// </summary>
        public void ResetEnemy()
        {
            hp = initialHp;
            hitFlashTimer = 0f;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            Beam?.Cut();
            ReleaseHeldShip();
        }

        /// <summary>
        /// Inyecta las refs de escena (scoreManager + gameFlow). Aditivo al design: los prefabs
        /// no pueden referenciar objetos de escena, así que FormationManager las asigna al spawn.
        /// </summary>
        public void InitRefs(ScoreManager sm, GameFlow gf)
        {
            scoreManager = sm;
            gameFlow = gf;
        }

        /// <summary>
        /// El haz alcanzó al jugador: este Boss pasa a carrier, se estaciona y retiene la nave.
        /// Idempotente: sólo desde Formation y si todavía no es carrier.
        /// </summary>
        public void OnBeamHitPlayer()
        {
            if (State != EnemyState.Formation || IsCarrier) return;

            IsCarrier = true;
            transform.position = new Vector3(carrierHold.x, carrierHold.y, 0f);
            if (heldShip != null) heldShip.SetActive(true);
            Beam?.Cut();
            gameFlow?.OnPlayerCaptured(this);
        }

        /// <summary>Apaga la nave retenida y libera el flag de carrier (sin rescate).</summary>
        public void ReleaseHeldShip()
        {
            if (heldShip != null) heldShip.SetActive(false);
            IsCarrier = false;
        }

        private void Update()
        {
            switch (State)
            {
                case EnemyState.Entering: UpdateEntering(); break;
                case EnemyState.Diving: UpdateDiving(); break;
                // Formation: quieto en slot. Dead: el GO está desactivado, no llega acá.
            }

            UpdateHitFlash();
        }

        private void UpdateEntering()
        {
            float y = Mathf.MoveTowards(transform.position.y, SlotPosition.y, enterSpeed * Time.deltaTime);
            float progress = Mathf.Clamp01(1f - (y - SlotPosition.y) / (enterStartY - SlotPosition.y));
            float sway = Mathf.Sin((Time.time - enterTime) * enterSwayFrequency) * enterSwayAmplitude * (1f - progress);
            float x = SlotPosition.x + sway;

            transform.position = new Vector3(x, y, 0f);

            if (Vector2.Distance(transform.position, SlotPosition) < arrivalDistance)
            {
                transform.position = SlotPosition;
                State = EnemyState.Formation;
            }
        }

        private void UpdateDiving()
        {
            if (divingDown)
            {
                // Fase 1: recta hacia la zona del jugador (sin interacción en M1)
                Vector2 target = new Vector2(divePlayerX, diveTargetY);
                transform.position = Vector2.MoveTowards(transform.position, target, diveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, target) <= arrivalDistance)
                {
                    transform.position = target;
                    divingDown = false;
                }
            }
            else
            {
                // Fase 2: retorno recto al slot
                transform.position = Vector2.MoveTowards(transform.position, SlotPosition, returnSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, SlotPosition) <= arrivalDistance)
                {
                    transform.position = SlotPosition;
                    State = EnemyState.Formation;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Bullet>(out Bullet bullet)) return;
            Die(bullet);
        }

        private void Die(Bullet bullet)
        {
            hp--;
            // La bala se consume en CADA impacto (fix sobre el design: sin esto el Boss
            // recibiría 2+ hits en el mismo frame porque la bala seguiría overlapando).
            Destroy(bullet.gameObject);

            if (hp > 0)
            {
                // Hit sobreviviente: flash (Boss 2HP) y sigue en su estado.
                if (spriteRenderer != null) spriteRenderer.color = HitFlashColor;
                hitFlashTimer = HitFlashDuration;
                return;
            }

            // Puntaje por estado (02-datos): picada paga pointsMoving, formación pointsFormation.
            int pts = State == EnemyState.Diving ? pointsMoving : pointsFormation;
            scoreManager?.Add(pts);
            if (audioSource != null && explosionClip != null)
            {
                audioSource.PlayOneShot(explosionClip);
            }

            State = EnemyState.Dead;
            bool wasCarrier = IsCarrier;
            if (wasCarrier)
            {
                // Rescate: cortar el haz, soltar la nave y avisar UNA vez antes del kill.
                Beam?.Cut();
                ReleaseHeldShip();
                gameFlow?.OnCarrierDestroyed(this);
            }
            gameFlow?.OnEnemyKilled(this); // libera el slot (hueco) y avisa al conteo de grilla
            gameObject.SetActive(false); // slot queda como hueco (no se rellena en la misma ola)
        }

        private void UpdateHitFlash()
        {
            if (hitFlashTimer <= 0f) return;
            hitFlashTimer -= Time.deltaTime;
            if (hitFlashTimer <= 0f && spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}