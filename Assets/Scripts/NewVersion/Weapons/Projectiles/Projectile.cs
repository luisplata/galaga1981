using System.Collections;
using NewVersion.Ship;
using NewVersion.Ship.Enemies;
using UnityEngine;

namespace NewVersion.Weapons.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] protected ProjectileId projectile;
        [SerializeField] protected float speed;
        public string Id => projectile.Id;
        protected Rigidbody2D rb;
        protected IEnemiesSpawner _enemiesSpawner;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            DoStart();
            StartCoroutine(DestroyCoroutine(10));
        }

        protected abstract void DoStart();

        private IEnumerator DestroyCoroutine(int timeForDestroy)
        {
            yield return new WaitForSeconds(timeForDestroy);
            DoDestroy();
            Destroy(gameObject);
        }

        protected abstract void DoDestroy();

        protected void FixedUpdate()
        {
            if (_enemiesSpawner.IsPause())
            {
                rb.velocity = Vector2.zero;
                return;
            }
            DoMove();
        }

        protected abstract void DoMove();

        public void AddingMediator(IEnemiesSpawner enemiesSpawner)
        {
            _enemiesSpawner = enemiesSpawner;
        }
    }
}