using UnityEngine;

namespace NewVersion.Weapons.Projectiles
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
            var transformLocal = transform;
            rb.velocity = transformLocal.up * (speed * Time.deltaTime) + transformLocal.right * (curve.Evaluate(delta) * forceAnimation);
        }
    }
}