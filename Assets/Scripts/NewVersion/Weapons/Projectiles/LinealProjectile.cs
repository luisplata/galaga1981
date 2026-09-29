using UnityEngine;

namespace NewVersion.Weapons.Projectiles
{
    public class LinealProjectile : Projectile
    {
        protected override void DoStart()
        {
            rb.linearVelocity = transform.up * (speed * Time.deltaTime);
        }

        protected override void DoDestroy()
        {
        }

        protected override void DoMove()
        {
            rb.linearVelocity = transform.up * (speed * Time.deltaTime);
        }
    }
}