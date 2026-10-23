using System.Collections.Generic;
using UnityEngine;

namespace V2
{
    /// <summary>
    /// Autofire: 1 proyectil hacia +Y cada fireInterval (0.25s) con máximo maxShots (2)
    /// activos en pantalla. Solo dispara cuando el flujo lo habilita.
    /// </summary>
    public class Autofire : MonoBehaviour
    {
        [SerializeField] private float fireInterval = 0.25f;
        [SerializeField] private int maxShots = 2;
        [SerializeField] private int dualMaxShots = 4;
        [SerializeField] private float dualOffsetX = 0.15f;
        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private float spawnOffsetY = 0.6f;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private AudioClip laserClip;

        private readonly List<Bullet> activeBullets = new List<Bullet>();
        private AudioSource audioSource;
        private float timer;
        private bool fireEnabled = true;

        /// <summary>Dual Fighter: 2 balas por volley (±0.15) y tope 4 activas.</summary>
        public bool IsDual { get; private set; }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void SetFireEnabled(bool enabled)
        {
            fireEnabled = enabled;
            if (!enabled) timer = 0f;
        }

        public void SetDual(bool value)
        {
            IsDual = value;
        }

        private void Update()
        {
            if (!fireEnabled) return;

            timer += Time.deltaTime;
            activeBullets.RemoveAll(bullet => bullet == null);

            int need = IsDual ? 2 : 1;
            int limit = IsDual ? dualMaxShots : maxShots;
            if (timer < fireInterval || activeBullets.Count + need > limit) return;

            timer = 0f;
            Vector3 spawn = transform.position + Vector3.up * spawnOffsetY;
            if (IsDual)
            {
                // Volley doble: una bala a cada lado del eje de la nave.
                SpawnBullet(spawn + Vector3.right * -dualOffsetX);
                SpawnBullet(spawn + Vector3.right * dualOffsetX);
            }
            else
            {
                SpawnBullet(spawn);
            }

            if (audioSource != null && laserClip != null)
            {
                audioSource.PlayOneShot(laserClip);
            }
        }

        private void SpawnBullet(Vector3 spawn)
        {
            Bullet bullet = Instantiate(bulletPrefab, spawn, Quaternion.identity);
            bullet.Init(bulletSpeed);
            activeBullets.Add(bullet);
        }
    }
}