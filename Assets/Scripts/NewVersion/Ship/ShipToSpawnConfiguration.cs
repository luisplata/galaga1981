using NewVersion.Weapons;
using UnityEngine;

namespace NewVersion.Ship
{
    [CreateAssetMenu(menuName = "Ship/Create ShipToSpawnConfiguration", fileName = "ShipToSpawnConfiguration", order = 0)]
    public class ShipToSpawnConfiguration : ScriptableObject
    {
        [SerializeField] private Vector2 speed;
        [SerializeField] private ShipId shipId;
        [SerializeField] private ProjectileId projectileId;
        [SerializeField] private float fireRatio;
        [SerializeField] private AnimationCurve curve;

        public AnimationCurve Curve => curve;
        public Vector2 Speed => speed;
        public ShipId ShipId => shipId;
        public ProjectileId ProjectileId => projectileId;
        public float FireRatio => fireRatio;

    }
}