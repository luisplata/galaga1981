using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Olas infinitas: entrada escalonada (1 enemigo cada spawnInterval, fila por fila),
    /// cooldown de picadas nivel 1 fijo (5-8s, tunable) con 1-2 buzos por ciclo, y ola
    /// siguiente automática al quedar la grilla vacía (gap waveGap). Nivel 1 fijo: sin escalado.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private FormationManager formation;
        [SerializeField] private ChallengingStage challenging;
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform player; // x del jugador para la fase 1 de la picada
        [SerializeField] private Vector2 spawnPoint = new(0f, 6f); // arriba del viewport (cámara ortho 9.6)

        // Timing de entrada (patrón curvo del pack)
        [SerializeField] private float spawnInterval = 0.35f;
        [SerializeField] private float waveGap = 1.5f;

        // Cooldown picadas — nivel 1 fijo (tunable)
        [SerializeField] private float diveCooldownMin = 5f;
        [SerializeField] private float diveCooldownMax = 8f;
        [SerializeField] private int maxDiversAtOnce = 2;

        // Tractor (M2): desde nivel 3, 1 Boss por ciclo cada 10-14 s.
        [SerializeField] private float tractorCooldownMin = 10f;
        [SerializeField] private float tractorCooldownMax = 14f;
        [SerializeField] private int tractorMinLevel = 3;

        private Coroutine waveLoop;
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

        public void StopWaves()
        {
            wavesActive = false;
            challenging?.Cancel();
            if (waveLoop != null) { StopCoroutine(waveLoop); waveLoop = null; }
            if (diveScheduler != null) { StopCoroutine(diveScheduler); diveScheduler = null; }
            if (tractorScheduler != null) { StopCoroutine(tractorScheduler); tractorScheduler = null; }
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
                    yield return StartCoroutine(SpawnRoutine());

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
        /// Prepara la ola (Slots asignados) y activa 1 enemigo cada spawnInterval, fila por fila.
        /// </summary>
        private IEnumerator SpawnRoutine()
        {
            if (formation == null) yield break;

            List<Enemy> wave = formation.SpawnFormation(spawnPoint);
            foreach (Enemy e in wave)
            {
                yield return new WaitForSeconds(spawnInterval);
                if (e == null) continue;
                e.gameObject.SetActive(true);
                e.BeginEntry(); // re-setea el timer de la curva (el GO estuvo inactivo)
            }
        }

        /// <summary>
        /// Timer aleatorio 5-8s → PickDiver × maxDiversAtOnce (1-2 enemigos en Diving).
        /// En challenging no hay picadas. Si la grilla está en entrada/vacía, PickDiver
        /// devuelve null y el ciclo sigue.
        /// </summary>
        private IEnumerator DiveScheduler()
        {
            while (wavesActive)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(diveCooldownMin, diveCooldownMax));
                if (!wavesActive) yield break;
                if (IsChallenging) continue;

                for (int i = 0; i < maxDiversAtOnce; i++)
                {
                    Enemy diver = formation != null ? formation.PickDiver() : null;
                    if (diver == null) break;
                    diver.StartDive(player != null ? player.position.x : 0f);
                }
            }
        }

        /// <summary>
        /// Timer 10-14 s → un Boss en Formation arranca el tractor. Sólo nivel ≥3 y nunca
        /// durante el challenging ni con la grilla vacía (PickTractorBoss devuelve null).
        /// </summary>
        private IEnumerator TractorScheduler()
        {
            while (wavesActive)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(tractorCooldownMin, tractorCooldownMax));
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