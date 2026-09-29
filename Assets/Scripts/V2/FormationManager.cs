using System;
using System.Collections.Generic;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Config de una fila de la grilla: tipo de enemigo y cantidad (04-contenido; tunable).
    /// </summary>
    [Serializable]
    public class RowConfig
    {
        public EnemyType type;
        public int count;
    }

    /// <summary>
    /// Grilla de slots CALCULADA (sin GO-slot): posición O(1) por fila/columna, centrada por fila.
    /// Pool interno: los enemigos muertos se desactivan (hueco visual) y se reutilizan en la
    /// siguiente ola (sin alloc). Emite OnFormationCleared cuando la grilla queda vacía.
    /// </summary>
    public class FormationManager : MonoBehaviour
    {
        // Grilla 5 filas / 24 enemigos = 4 boss / 8 guard / 12 grunt (pack 04, números a ajustar)
        [SerializeField] private RowConfig[] rows = {
            new() { type = EnemyType.Boss,  count = 4 },
            new() { type = EnemyType.Guard, count = 8 },
            new() { type = EnemyType.Grunt, count = 4 },
            new() { type = EnemyType.Grunt, count = 4 },
            new() { type = EnemyType.Grunt, count = 4 },
        };
        [SerializeField] private float spacingX = 0.55f;
        [SerializeField] private float spacingY = 0.55f;
        [SerializeField] private Vector2 gridCenter = new(0f, 3f); // y fila 0; fila 4 = 3 - 4*0.55 = 0.8
        [SerializeField] private Enemy[] enemyPrefabs = new Enemy[3]; // [0]=Boss [1]=Guard [2]=Grunt
        [SerializeField] private Transform enemyParent;

        // Aditivo al design: refs de escena que se inyectan a CADA instancia (los prefabs no
        // pueden referenciar objetos de escena). Sin esto, los kills no suman puntos ni limpian
        // la grilla.
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameFlow gameFlow;

        public event Action OnFormationCleared;
        public int AliveCount { get; private set; }

        private readonly List<Enemy> pool = new();
        private readonly List<Enemy> active = new();

        /// <summary>
        /// Prepara la ola completa: asigna slot único a cada enemigo (fila por fila, en orden de
        /// entrada). Los GOs quedan inactivos en spawnPoint; WaveManager los activa escalonado.
        /// </summary>
        public List<Enemy> SpawnFormation(Vector2 spawnPoint)
        {
            active.Clear();
            var result = new List<Enemy>();

            for (int row = 0; row < rows.Length; row++)
            {
                RowConfig config = rows[row];
                for (int col = 0; col < config.count; col++)
                {
                    Vector2 slot = SlotWorld(row, col);
                    Enemy e = GetFromPool(config.type);
                    e.ResetEnemy();
                    e.AssignSlot(slot, spawnPoint);
                    result.Add(e);
                }
            }

            active.AddRange(result);
            AliveCount = active.Count;
            return result;
        }

        /// <summary>
        /// Un enemigo murió: baja el contador; si la grilla queda vacía → OnFormationCleared
        /// (WaveManager lanza la siguiente ola). El slot muerto queda como hueco.
        /// </summary>
        public void NotifyKilled(Enemy e)
        {
            AliveCount = Mathf.Max(0, AliveCount - 1);
            if (AliveCount == 0)
            {
                OnFormationCleared?.Invoke();
            }
        }

        /// <summary>
        /// Elige un enemigo vivo en Formation al azar (reservoir sin alloc). null si no hay.
        /// </summary>
        public Enemy PickDiver()
        {
            int count = 0;
            for (int i = 0; i < active.Count; i++)
            {
                Enemy e = active[i];
                if (e != null && e.State == EnemyState.Formation) count++;
            }

            if (count == 0) return null;

            int pick = UnityEngine.Random.Range(0, count);
            for (int i = 0; i < active.Count; i++)
            {
                Enemy e = active[i];
                if (e != null && e.State == EnemyState.Formation)
                {
                    if (pick == 0) return e;
                    pick--;
                }
            }
            return null;
        }

        /// <summary>
        /// Slot de la grilla: x centrada por fila (col - (count-1)/2), y = gridCenter.y - row*spacingY.
        /// </summary>
        private Vector2 SlotWorld(int row, int col)
        {
            RowConfig config = rows[row];
            float x = gridCenter.x + (col - (config.count - 1) / 2f) * spacingX;
            float y = gridCenter.y - row * spacingY;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Reutiliza un enemigo muerto del tipo pedido (ola anterior) o instancia uno nuevo.
        /// En ambos casos inyecta las refs de escena (scoreManager/gameFlow).
        /// </summary>
        private Enemy GetFromPool(EnemyType type)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                Enemy e = pool[i];
                if (e != null && e.Type == type && e.State == EnemyState.Dead)
                {
                    e.InitRefs(scoreManager, gameFlow);
                    return e;
                }
            }

            Enemy prefab = enemyPrefabs[(int)type];
            Enemy instance = Instantiate(prefab, enemyParent);
            instance.gameObject.SetActive(false);
            instance.InitRefs(scoreManager, gameFlow);
            pool.Add(instance);
            return instance;
        }
    }
}