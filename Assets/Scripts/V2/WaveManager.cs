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
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform player; // x del jugador para la fase 1 de la picada
        [SerializeField] private Vector2 spawnPoint = new(0f, 6f); // arriba del viewport (cámara 4.8)

        // Timing de entrada (patrón curvo del pack)
        [SerializeField] private float spawnInterval = 0.35f;
        [SerializeField] private float waveGap = 1.5f;

        // Cooldown picadas — nivel 1 fijo (tunable)
        [SerializeField] private float diveCooldownMin = 5f;
        [SerializeField] private float diveCooldownMax = 8f;
        [SerializeField] private int maxDiversAtOnce = 2;

        private Coroutine waveLoop;
        private Coroutine diveScheduler;
        private bool wavesActive;

        /// <summary>
        /// Lanza el ciclo de olas infinitas y el scheduler de picadas (desde GameFlow.StartGame).
        /// </summary>
        public void StartWaves()
        {
            StopWaves();
            wavesActive = true;
            waveLoop = StartCoroutine(WaveLoop());
            diveScheduler = StartCoroutine(DiveScheduler());
        }

        public void StopWaves()
        {
            wavesActive = false;
            if (waveLoop != null) { StopCoroutine(waveLoop); waveLoop = null; }
            if (diveScheduler != null) { StopCoroutine(diveScheduler); diveScheduler = null; }
        }

        /// <summary>
        /// Un enemigo murió (vía GameFlow.OnEnemyKilled): baja el contador de la grilla.
        /// Si queda vacía, FormationManager emite OnFormationCleared → WaveLoop sigue.
        /// </summary>
        public void OnEnemyKilled(Enemy e)
        {
            if (formation != null) formation.NotifyKilled(e);
        }

        /// <summary>
        /// Ola actual → esperar limpieza de grilla → gap → siguiente ola (loop ∞).
        /// </summary>
        private IEnumerator WaveLoop()
        {
            while (wavesActive)
            {
                yield return StartCoroutine(SpawnRoutine());

                // Espera la limpieza de la grilla (AliveCount llega a 0 vía NotifyKilled).
                while (wavesActive && formation != null && formation.AliveCount > 0)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(waveGap);
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
        /// Si la grilla está en entrada/vacía, PickDiver devuelve null y el ciclo sigue.
        /// </summary>
        private IEnumerator DiveScheduler()
        {
            while (wavesActive)
            {
                yield return new WaitForSeconds(Random.Range(diveCooldownMin, diveCooldownMax));
                if (!wavesActive) yield break;

                for (int i = 0; i < maxDiversAtOnce; i++)
                {
                    Enemy diver = formation != null ? formation.PickDiver() : null;
                    if (diver == null) break;
                    diver.StartDive(player != null ? player.position.x : 0f);
                }
            }
        }
    }
}