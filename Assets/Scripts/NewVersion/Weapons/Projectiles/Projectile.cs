using System.Collections;
using UnityEngine;

namespace NewVersion.Weapons.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] protected ProyectileId projectile;
        [SerializeField] protected float speed;
        public string Id => projectile.Id;
        protected Rigidbody2D rb;

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
            DoMove();
        }

        protected abstract void DoMove();
    }
}