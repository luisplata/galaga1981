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
        [SerializeField] private float bulletSpeed = 15f;
        [SerializeField] private float spawnOffsetY = 0.6f;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private AudioClip laserClip;

        private readonly List<Bullet> activeBullets = new List<Bullet>();
        private AudioSource audioSource;
        private float timer;
        private bool fireEnabled = true;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void SetFireEnabled(bool enabled)
        {
            fireEnabled = enabled;
            if (!enabled) timer = 0f;
        }

        private void Update()
        {
            if (!fireEnabled) return;

            timer += Time.deltaTime;
            activeBullets.RemoveAll(bullet => bullet == null);

            if (timer >= fireInterval && activeBullets.Count < maxShots)
            {
                timer = 0f;
                Vector3 spawn = transform.position + Vector3.up * spawnOffsetY;
                Bullet bullet = Instantiate(bulletPrefab, spawn, Quaternion.identity);
                bullet.Init(bulletSpeed);
                activeBullets.Add(bullet);

                if (audioSource != null && laserClip != null)
                {
                    audioSource.PlayOneShot(laserClip);
                }
            }
        }
    }
}