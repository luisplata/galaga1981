using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Olas infinitas: entrada escalonada (1 enemigo cada SpawnInterval de Difficulty, fila por
    /// fila), picadas dirigidas al player y ola siguiente automática al quedar la grilla vacía
    /// (gap waveGap). La amenaza (cooldowns, divers, velocidad, spawn) sale de
    /// Difficulty.ForLevel(CurrentLevel) en cada ciclo; el challenging no escala.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private FormationManager formation;
        [SerializeField] private ChallengingStage challenging;
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform player; // x/y del jugador al iniciar la picada
        [SerializeField] private Vector2 spawnPoint = new(0f, 6f); // arriba del viewport (cámara ortho 9.6)

        // Timing de entrada
        [SerializeField] private float waveGap = 1.5f;

        // Tractor (M2): desde nivel 3, 1 Boss por ciclo; cooldown escalado por Difficulty.
        [SerializeField] private int tractorMinLevel = 3;

        private Coroutine waveLoop;
        private Coroutine spawnRoutine;
        private Coroutine diveScheduler;
        private Coroutine tractorScheduler;
        private bool wavesActive;

        /// <summary>Nivel actual (arranca en 1 y lo resetea StartWaves).</summary>
        public int CurrentLevel { get; private set; } = 1;

        /// <summary>Se emite después de cada CurrentLevel++ (HUD de nivel).</summary>
        public event Action<int> LevelChanged;

        /// <summary>Stage bonus en curso: no hay grilla, picadas ni tractor.</summary>
        public bool IsChallenging => challenging != null && challenging.IsRunning;

        /// <summary>
        /// Lanza el ciclo de olas infinitas, las picadas y el tractor (desde GameFlow.StartGame).
        /// </summary>
        public void StartWaves()
        {
            StopWaves();
            CurrentLevel = 1;
            wavesActive = true;
            waveLoop = StartCoroutine(WaveLoop());
            diveScheduler = StartCoroutine(DiveScheduler());
            tractorScheduler = StartCoroutine(TractorScheduler());
        }

        /// <summary>
        /// Detiene TODO y deja el mundo limpio (GameOver / retry). Orden estricto:
        /// 1) cierra el gate, 2) cancela las coroutines (incluida SpawnRoutine, que corre
        /// aparte de WaveLoop y podría reactivar enemigos ya descartados), 3) challenging
        /// (pool propio), 4) formación (pool completo, sin evento: no debe disparar una ola).
        /// </summary>
        public void StopWaves()
        {
            wavesActive = false;
            if (spawnRoutine != null) { StopCoroutine(spawnRoutine); spawnRoutine = null; }
            if (waveLoop != null) { StopCoroutine(waveLoop); waveLoop = null; }
            if (diveScheduler != null) { StopCoroutine(diveScheduler); diveScheduler = null; }
            if (tractorScheduler != null) { StopCoroutine(tractorScheduler); tractorScheduler = null; }
            challenging?.Cancel();
            formation?.ClearAll();
        }

        /// <summary>
        /// Un enemigo murió (vía GameFlow.OnEnemyKilled): baja el contador de la grilla.
        /// Si queda vacía, FormationManager emite OnFormationCleared → WaveLoop sigue.
        /// </summary>
        /// <summary>
        /// Un enemigo murió (vía GameFlow.OnEnemyKilled). En challenging cuenta para el stage;
        /// en grilla baja AliveCount de la formación. Nunca ambos.
        /// </summary>
        public void OnEnemyKilled(Enemy e)
        {
            if (IsChallenging)
            {
                challenging.OnEnemyKilled(e);
                return;
            }
            if (formation != null) formation.NotifyKilled(e);
        }

        /// <summary>Un enemigo de challenging salió del viewport: no puntúa ni penaliza.</summary>
        public void OnEnemyEscaped(Enemy e)
        {
            if (IsChallenging) challenging.OnEnemyEscaped(e);
        }

        /// <summary>
        /// El Boss capturó al jugador: sale del conteo de la grilla (sin puntaje) para que la
        /// ola pueda cerrar y el carrier quede estacionado entre niveles.
        /// </summary>
        public void OnPlayerCaptured(Enemy carrier)
        {
            if (formation != null) formation.NotifyCarrierLeave(carrier);
        }

        /// <summary>
        /// Stage actual → limpieza → gap → nivel+1 (LevelChanged). Múltiplo de 4 corre el
        /// challenging en vez de la grilla; nunca ambos.
        /// </summary>
        private IEnumerator WaveLoop()
        {
            while (wavesActive)
            {
                if (CurrentLevel % 4 == 0 && challenging != null)
                {
                    yield return challenging.Run();
                }
                else
                {
                    spawnRoutine = StartCoroutine(SpawnRoutine());
                    yield return spawnRoutine;
                    spawnRoutine = null;

                    // Espera la limpieza de la grilla (AliveCount llega a 0 vía NotifyKilled).
                    while (wavesActive && formation != null && formation.AliveCount > 0)
                    {
                        yield return null;
                    }
                }

                if (!wavesActive) yield break;
                yield return new WaitForSeconds(waveGap);
                if (!wavesActive) yield break;

                CurrentLevel++;
                LevelChanged?.Invoke(CurrentLevel);
            }
        }

        /// <summary>
        /// Prepara la ola (Slots asignados) y activa 1 enemigo cada SpawnInterval, fila por fila.
        /// El multiplicador de picada se aplica en la activación, después de SpawnFormation
        /// (que ya llamó ResetEnemy y restauró la velocidad base).
        /// </summary>
        private IEnumerator SpawnRoutine()
        {
            if (formation == null) yield break;

            DifficultySettings d = Difficulty.ForLevel(CurrentLevel);
            List<Enemy> wave = formation.SpawnFormation(spawnPoint);
            foreach (Enemy e in wave)
            {
                yield return new WaitForSeconds(d.SpawnInterval);
                if (e == null) continue;
                e.SetDiveSpeedMul(d.DiveSpeedMul);
                e.gameObject.SetActive(true);
                e.BeginEntry(); // re-setea el timer de la curva (el GO estuvo inactivo)
            }
        }

        /// <summary>
        /// Timer aleatorio de Difficulty (nivel 1: 5-8 s) → PickDiver × MaxDivers.
        /// En challenging no hay picadas. Si la grilla está en entrada/vacía, PickDiver
        /// devuelve null y el ciclo sigue.
        /// </summary>
        private IEnumerator DiveScheduler()
        {
            while (wavesActive)
            {
                DifficultySettings d = Difficulty.ForLevel(CurrentLevel);
                yield return new WaitForSeconds(UnityEngine.Random.Range(d.DiveCooldownMin, d.DiveCooldownMax));
                if (!wavesActive) yield break;
                if (IsChallenging) continue;

                for (int i = 0; i < d.MaxDivers; i++)
                {
                    Enemy diver = formation != null ? formation.PickDiver() : null;
                    if (diver == null) break;
                    // X e Y del player congeladas al iniciar (telegraph): con X-only el dive
                    // paraba en y=-6 y nunca entraba al radio letal de 0.5 con el player en -7.5.
                    float targetX = player != null ? player.position.x : 0f;
                    float targetY = player != null ? player.position.y : -6f;
                    diver.StartDive(targetX, targetY);
                }
            }
        }

        /// <summary>
        /// Timer de Difficulty (nivel 3: 10-14 s) → un Boss en Formation arranca el tractor.
        /// Sólo nivel ≥3 y nunca durante el challenging ni con la grilla vacía.
        /// </summary>
        private IEnumerator TractorScheduler()
        {
            while (wavesActive)
            {
                DifficultySettings d = Difficulty.ForLevel(CurrentLevel);
                yield return new WaitForSeconds(UnityEngine.Random.Range(d.TractorCooldownMin, d.TractorCooldownMax));
                if (!wavesActive) yield break;
                if (IsChallenging) continue;
                if (CurrentLevel < tractorMinLevel) continue;
                if (formation == null) continue;

                Enemy boss = formation.PickTractorBoss();
                if (boss == null || boss.Beam == null) continue;
                boss.Beam.BeginCycle();
            }
        }
    }
}