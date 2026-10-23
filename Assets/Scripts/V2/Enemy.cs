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
    public enum EnemyState { Entering, Formation, Diving, FlyThrough, Dead }

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

        // Picada — 2 fases rectas (baja a (playerX, playerY) → retorna al slot)
        [SerializeField] private float diveSpeed = 7f;
        [SerializeField] private float returnSpeed = 6f;

        // FlyThrough (M2 challenging): cruce bezier de borde a borde, sin slot ni picada.
        [SerializeField] private float flyDuration = 3f;

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
        private float initialDiveSpeed;
        private float enterTime;
        private float enterStartY;
        private float divePlayerX;
        private float currentDiveTargetY;
        private bool divingDown;
        private bool hasHitPlayerThisDive;
        private float hitFlashTimer;
        private float flyTime;
        private float flyTotal;
        private Vector3 flyStart;
        private Vector3 flyControl;
        private Vector3 flyEnd;

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
            initialDiveSpeed = diveSpeed;
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
        /// Inicia la picada de 2 fases rectas hacia (playerX, targetY) y retorno al slot.
        /// X e Y se congelan al iniciar (telegraph). Sólo desde Formation.
        /// </summary>
        public void StartDive(float playerX, float targetY)
        {
            if (State != EnemyState.Formation || IsCarrier) return;
            divePlayerX = playerX;
            currentDiveTargetY = targetY;
            divingDown = true;
            hasHitPlayerThisDive = false;
            State = EnemyState.Diving;
        }

        /// <summary>
        /// Re-uso para la próxima ola: restaura HP/flash y limpia el estado de carrier
        /// (el pool reutiliza instancias entre olas y niveles).
        /// </summary>
        public void ResetEnemy()
        {
            hp = initialHp;
            diveSpeed = initialDiveSpeed;
            hitFlashTimer = 0f;
            hasHitPlayerThisDive = false;
            divingDown = false;
            if (spriteRenderer != null) spriteRenderer.color = Color.white;
            Beam?.Cut();
            ReleaseHeldShip();
        }

        /// <summary>
        /// Escalado M3: multiplica la velocidad base cacheada en Awake (1.0 = base).
        /// ResetEnemy restaura el valor original al reutilizar la instancia.
        /// </summary>
        public void SetDiveSpeedMul(float mul)
        {
            diveSpeed = initialDiveSpeed * mul;
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

            // i-frames: consultar ANTES de estacionar el carrier. Si no, quedaría un Boss
            // parked con la nave retenida sin captura consumida.
            if (gameFlow == null || !gameFlow.CanTakeDamage) return;

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

        /// <summary>
        /// Saca la instancia de juego sin puntaje ni notificaciones: Dead + GO inactivo.
        /// Se usa para carriers descartados (segunda captura) y flyers cancelados, así el
        /// pool (que exige Dead) puede reutilizarlos en la próxima ola/stage.
        /// </summary>
        public void Discard()
        {
            if (State == EnemyState.Dead) return;
            Beam?.Cut();
            ReleaseHeldShip();
            State = EnemyState.Dead;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Inicia el cruce del challenging: bezier cuadratica start -> control -> end durante
        /// flyDuration. Al salir del viewport muere como escapado (sin puntaje).
        /// </summary>
        public void BeginFlyThrough(Vector3 start, Vector3 control, Vector3 end)
        {
            BeginFlyThrough(start, control, end, flyDuration);
        }

        /// <summary>Variante con duracion explicita (ChallengingStage tiene su propio tunable).</summary>
        public void BeginFlyThrough(Vector3 start, Vector3 control, Vector3 end, float duration)
        {
            flyStart = start;
            flyControl = control;
            flyEnd = end;
            flyTotal = Mathf.Max(0.1f, duration);
            flyTime = Time.time;
            transform.position = start;
            State = EnemyState.FlyThrough;
        }

        private void Update()
        {
            switch (State)
            {
                case EnemyState.Entering: UpdateEntering(); break;
                case EnemyState.Diving: UpdateDiving(); break;
                case EnemyState.FlyThrough: UpdateFlyThrough(); break;
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
                // Fase 1: recta hacia la posición congelada del jugador al iniciar la picada.
                Vector2 target = new Vector2(divePlayerX, currentDiveTargetY);
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

            // Ida y vuelta dañan. Una picada pega como máximo una vez.
            TryHitPlayer();
        }

        /// <summary>
        /// Impacto matemático contra el player (sin física): radio 0.5 inclusivo.
        /// El enemigo NO muere ni corta la picada; completa el retorno al slot.
        /// </summary>
        private void TryHitPlayer()
        {
            if (hasHitPlayerThisDive) return;
            if (gameFlow == null || !gameFlow.CanTakeDamage) return;

            Transform target = PlayerTransform;
            if (target == null) return;

            if (!HitMath.WithinSquared(transform.position, target.position, HitMath.PlayerHitRadius)) return;

            if (gameFlow.OnPlayerHit(this)) hasHitPlayerThisDive = true;
        }

        private void UpdateFlyThrough()
        {
            float t = Mathf.Clamp01((Time.time - flyTime) / flyTotal);
            float u = 1f - t;
            transform.position = u * u * flyStart + 2f * u * t * flyControl + t * t * flyEnd;

            if (t < 1f) return;

            State = EnemyState.Dead;
            gameFlow?.OnEnemyEscaped(this);
            gameObject.SetActive(false);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Bullet>(out Bullet bullet)) return;
            Die(bullet);
        }

        private void Die(Bullet bullet)
        {
            // Dos balas del volley dual pueden impactar el mismo enemigo en el MISMO physics
            // step: sólo el primer impacto resuelve la muerte (puntaje/audio/rescate). El
            // segundo consume su bala y sale, para no duplicar Add ni los efectos.
            if (State == EnemyState.Dead)
            {
                if (bullet != null) Destroy(bullet.gameObject);
                return;
            }

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
            // Bonus x1.5 por cercanía SOLO en la ruta de kill (ScoreManager.Add queda intacto,
            // así el +1000 del challenging no se multiplica).
            int pts = State == EnemyState.Diving || State == EnemyState.FlyThrough ? pointsMoving : pointsFormation;
            pts = ScoreRules.ApplyProximity(pts, DistanceToPlayer(), ScoreRules.NearRadius);
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

        /// <summary>
        /// Distancia centro-a-centro al player en el instante de la muerte; +∞ si no hay player
        /// (así el bonus de cercanía nunca se regala sin jugador).
        /// </summary>
        private float DistanceToPlayer()
        {
            Transform target = PlayerTransform;
            return target != null ? Vector2.Distance(transform.position, target.position) : float.PositiveInfinity;
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