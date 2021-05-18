using NewVersion.Weapons.Projectiles;
using UnityEngine;

namespace NewVersion.Weapons
{
    public class LinealProjectile : Projectile
    {
        protected override void DoStart()
        {
            rb.velocity = Vector2.up * (speed * Time.deltaTime);
        }

        protected override void DoDestroy()
        {
        }

        protected override void DoMove()
        {
        }
    }
}