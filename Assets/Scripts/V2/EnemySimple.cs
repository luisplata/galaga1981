using UnityEngine;

namespace V2
{
    /// <summary>
    /// Enemigo simple de formación: estático, muere en 1 hit y suma 50 puntos.
    /// Se desactiva al morir (respawn estable en restart) en vez de destruirse.
    /// </summary>
    public class EnemySimple : MonoBehaviour
    {
        [SerializeField] private int hp = 1;
        [SerializeField] private int points = 50;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameFlow gameFlow;
        [SerializeField] private AudioClip explosionClip;

        private AudioSource audioSource;
        private int initialHp;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            initialHp = hp;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Bullet>(out Bullet bullet)) return;
            Die(bullet);
        }

        private void Die(Bullet bullet)
        {
            hp--;
            if (hp > 0) return;

            scoreManager?.Add(points);
            if (audioSource != null && explosionClip != null)
            {
                audioSource.PlayOneShot(explosionClip);
            }

            Destroy(bullet.gameObject);
            gameFlow?.OnEnemyKilled(this);
            gameObject.SetActive(false);
        }

        public void Respawn()
        {
            hp = initialHp;
            gameObject.SetActive(true);
        }
    }
}