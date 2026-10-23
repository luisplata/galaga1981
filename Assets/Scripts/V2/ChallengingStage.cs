using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Stage bonus M2 (cada 4 niveles): 24 enemigos en 5 tandas cruzan en arco bezier de
    /// borde a borde alternando el lado. Sin formación, picadas, disparo enemigo ni tractor.
    /// Pool propio: no toca el AliveCount de FormationManager.
    /// </summary>
    public class ChallengingStage : MonoBehaviour
    {
        [SerializeField] private Enemy[] enemyPrefabs = new Enemy[3]; // [0]=Boss [1]=Guard [2]=Grunt
        [SerializeField] private Transform enemyParent;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameFlow gameFlow;
        [SerializeField] private Transform player; // mantiene el arco por encima de la zona del jugador

        [SerializeField] private int totalEnemies = 24;
        [SerializeField] private int waveCount = 5;
        [SerializeField] private float spawnInterval = 0.25f;
        [SerializeField] private float flyDuration = 3f;
        [SerializeField] private int fullClearBonus = 1000;
        [SerializeField] private float timeout = 15f;
        [SerializeField] private float waveGap = 0.6f;

        // Composición (04-contenido, tunable): Boss, Guard, Grunt.
        private static readonly int[] Composition = { 4, 8, 12 };
        // 24 enemigos en 5 tandas.
        private static readonly int[] WaveSizes = { 5, 5, 5, 5, 4 };

        private const float SideOffsetX = 6.8f; // fuera del viewport (cámara orto 9.6, ancho 5.4)
        private const float ApexHeight = 5.5f;

        private readonly List<Enemy> pool = new();
        private readonly List<Enemy> active = new();
        private readonly List<EnemyType> queue = new();

        /// <summary>Stage en curso (false al terminar, cancelar o agotar el timeout).</summary>
        public bool IsRunning { get; private set; }

        /// <summary>Enemigos destruidos por el jugador (observabilidad / verificación).</summary>
        public int Kills { get; private set; }

        /// <summary>Enemigos resueltos (kills + escapes). Llega a totalEnemies al cerrar.</summary>
        public int Resolved { get; private set; }

        /// <summary>Tandas efectivamente lanzadas (5 en el stage completo).</summary>
        public int WavesStarted { get; private set; }

        /// <summary>
        /// Corre el stage completo: 5 tandas escalonadas y espera a que los 24 se resuelvan
        /// (o al timeout). Full clear = 24 kills → bonus; escapes no penalizan.
        /// </summary>
        public IEnumerator Run()
        {
            if (enemyPrefabs == null || enemyPrefabs.Length < 3 || enemyPrefabs[0] == null)
            {
                Debug.LogWarning("ChallengingStage: faltan prefabs; se saltea el stage bonus.");
                yield break;
            }

            IsRunning = true;
            Kills = 0;
            Resolved = 0;
            WavesStarted = 0;
            active.Clear();
            BuildQueue();

            float stageStart = Time.time;
            float playerY = player != null ? player.position.y : -7.5f;
            // El arco se mantiene arriba de la zona del jugador (no hay ataques en el stage).
            float baseY = Mathf.Max(1.5f, playerY + 9f);
            int spawned = 0;

            for (int wave = 0; wave < waveCount && wave < WaveSizes.Length; wave++)
            {
                if (!IsRunning) { Cleanup(); yield break; }

                bool fromLeft = wave % 2 == 0;
                WavesStarted++;

                for (int i = 0; i < WaveSizes[wave]; i++)
                {
                    if (!IsRunning) { Cleanup(); yield break; }
                    if (spawned >= totalEnemies || spawned >= queue.Count) break;

                    SpawnFlyer(queue[spawned], baseY, fromLeft);
                    spawned++;
                    yield return new WaitForSeconds(spawnInterval);
                }

                // Separa visualmente las tandas (el lado alterna entre una y otra).
                yield return new WaitForSeconds(waveGap);
            }

            while (IsRunning && Resolved < totalEnemies && Time.time - stageStart < timeout)
            {
                yield return null;
            }

            // Full clear: murieron los 24. Con escapes no hay bonus (y nunca resta).
            if (IsRunning && Kills >= totalEnemies)
            {
                scoreManager?.Add(fullClearBonus);
            }

            IsRunning = false;
            Cleanup();
        }

        /// <summary>Kill dentro del stage: suma puntos vía Enemy.Die y cuenta para el cierre.</summary>
        public void OnEnemyKilled(Enemy e)
        {
            if (!IsRunning) return;
            if (!active.Remove(e)) return;
            Kills++;
            Resolved++;
        }

        /// <summary>Enemigo que cruzó el viewport: cuenta como resuelto, sin puntos ni castigo.</summary>
        public void OnEnemyEscaped(Enemy e)
        {
            if (!IsRunning) return;
            if (!active.Remove(e)) return;
            Resolved++;
        }

        /// <summary>Corta el stage (StopWaves): apaga los que siguen en vuelo y libera el estado.</summary>
        public void Cancel()
        {
            if (!IsRunning) return;
            IsRunning = false;
            Cleanup();
        }

        private void SpawnFlyer(EnemyType type, float baseY, bool fromLeft)
        {
            Enemy e = GetFromPool(type);
            if (e == null) return;

            e.InitRefs(scoreManager, gameFlow);
            e.ResetEnemy();

            float side = fromLeft ? -1f : 1f;
            Vector3 start = new(side * SideOffsetX, baseY + Random.Range(0f, 2f), 0f);
            Vector3 end = new(-side * SideOffsetX, baseY + Random.Range(0f, 2f), 0f);
            Vector3 control = new(0f, baseY + ApexHeight + Random.Range(-0.8f, 1.2f), 0f);

            e.gameObject.SetActive(true);
            // El GO estuvo inactivo: BeginFlyThrough setea posición inicial y estado.
            e.BeginFlyThrough(start, control, end, flyDuration);
            active.Add(e);
        }

        private void BuildQueue()
        {
            queue.Clear();
            for (int i = 0; i < Composition.Length; i++)
            {
                for (int n = 0; n < Composition[i]; n++) queue.Add((EnemyType)i);
            }
        }

        private Enemy GetFromPool(EnemyType type)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                Enemy e = pool[i];
                if (e != null && e.Type == type && e.State == EnemyState.Dead)
                {
                    return e;
                }
            }

            Enemy prefab = enemyPrefabs[(int)type];
            if (prefab == null) return null;
            Enemy instance = Instantiate(prefab, enemyParent);
            instance.gameObject.SetActive(false);
            pool.Add(instance);
            return instance;
        }

        private void Cleanup()
        {
            for (int i = 0; i < active.Count; i++)
            {
                Enemy e = active[i];
                if (e == null) continue;
                // Dead (no FlyThrough) para que GetFromPool pueda reciclarlo: si queda en
                // vuelo pero inactivo, el stage pierde hasta 24 instancias por cancel/restart.
                e.Discard();
            }
            active.Clear();
        }
    }
}
