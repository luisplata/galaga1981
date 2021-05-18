using NewVersion.Weapons.Projectiles;
using UnityEngine;

namespace NewVersion.Weapons
{
    public class CurveProjectile : Projectile
    {
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float forceAnimation;
        private float delta;

        protected override void DoStart()
        {
            
        }

        protected override void DoDestroy()
        {
            
        }

        protected override void DoMove()
        {
            delta += Time.deltaTime;
            rb.velocity = Vector2.up * (speed * Time.deltaTime) + Vector2.right * (curve.Evaluate(delta) * forceAnimation);
        }
    }
}