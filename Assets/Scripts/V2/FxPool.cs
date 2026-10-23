using System.Collections.Generic;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Pool de flashes de muerte (juice M3): 12 SpriteRenderer hijos reutilizados.
    /// Sin ParticleSystem y sin Instantiate por kill. Si los 12 están ocupados se recicla
    /// el más antiguo. El flash dura 0.11 s con punch de escala 1.3 -> 1 y fade de alpha.
    /// </summary>
    public class FxPool : MonoBehaviour
    {
        public const int MaxFlashes = 12;

        [SerializeField] private float duration = 0.11f;
        [SerializeField] private float punchScale = 1.3f;
        [SerializeField] private Color killColor = Color.white;
        [SerializeField] private Color playerColor = new Color(1f, 0.55f, 0.15f, 1f);
        [SerializeField] private Sprite bossSprite;
        [SerializeField] private Sprite guardSprite;
        [SerializeField] private Sprite gruntSprite;
        [SerializeField] private Sprite playerSprite;

        private sealed class Entry
        {
            public SpriteRenderer renderer;
            public Color color;
            public float startedAt;
            public bool active;
        }

        private readonly List<Entry> entries = new List<Entry>(MaxFlashes);

        /// <summary>Cantidad de flashes activos en este frame (verificación del presupuesto).</summary>
        public int ActiveCount
        {
            get
            {
                EnsureInitialized();
                int count = 0;
                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].active) count++;
                }
                return count;
            }
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void OnEnable()
        {
            EnsureInitialized();
        }

        /// <summary>
        /// Los 12 hijos viven en la escena (inactivos): 0 Instantiate en gameplay.
        /// Idempotente y diferido a propósito: si el orden de carga/recarga de scripts deja
        /// el pool sin hijos en Awake, el primer Play lo completa.
        /// </summary>
        private void EnsureInitialized()
        {
            if (entries.Count > 0) return;

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < renderers.Length && entries.Count < MaxFlashes; i++)
            {
                SpriteRenderer sr = renderers[i];
                if (sr == null) continue;
                sr.gameObject.SetActive(false);
                entries.Add(new Entry { renderer = sr });
            }
        }

        /// <summary>Dispara un flash reutilizando una entrada libre o la más antigua.</summary>
        public void Play(Sprite sprite, Vector3 position, Color color)
        {
            EnsureInitialized();
            if (entries.Count == 0) return;

            Entry target = null;
            for (int i = 0; i < entries.Count; i++)
            {
                if (!entries[i].active)
                {
                    target = entries[i];
                    break;
                }
            }

            if (target == null)
            {
                target = entries[0];
                for (int i = 1; i < entries.Count; i++)
                {
                    if (entries[i].startedAt < target.startedAt) target = entries[i];
                }
            }

            SpriteRenderer sr = target.renderer;
            if (sr == null) return;

            target.active = true;
            target.startedAt = Time.time;
            target.color = color;

            if (sprite != null) sr.sprite = sprite;
            sr.color = color;
            sr.transform.position = position;
            sr.transform.localScale = Vector3.one * punchScale;
            sr.gameObject.SetActive(true);
        }

        /// <summary>Flash de un enemigo muerto, con el sprite de su tipo.</summary>
        public void PlayKill(EnemyType type, Vector3 position)
        {
            Play(SpriteFor(type), position, killColor);
        }

        /// <summary>Flash naranja de la nave del jugador (impacto o captura).</summary>
        public void PlayPlayerDeath(Vector3 position)
        {
            Play(playerSprite, position, playerColor);
        }

        private void Update()
        {
            float now = Time.time;
            for (int i = 0; i < entries.Count; i++)
            {
                Entry entry = entries[i];
                if (!entry.active || entry.renderer == null) continue;

                float t = (now - entry.startedAt) / Mathf.Max(0.0001f, duration);
                if (t >= 1f)
                {
                    entry.active = false;
                    entry.renderer.gameObject.SetActive(false);
                    continue;
                }

                Color color = entry.color;
                color.a = entry.color.a * (1f - t);
                entry.renderer.color = color;
                entry.renderer.transform.localScale = Vector3.one * Mathf.Lerp(punchScale, 1f, t);
            }
        }

        private Sprite SpriteFor(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Boss: return bossSprite;
                case EnemyType.Guard: return guardSprite;
                default: return gruntSprite;
            }
        }
    }
}
