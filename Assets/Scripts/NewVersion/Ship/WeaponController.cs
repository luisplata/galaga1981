using NewVersion.Weapons;
using UnityEngine;

namespace NewVersion.Ship
{
    public class WeaponController:MonoBehaviour
    {
        [SerializeField] private GameObject pointToSpawnProyectil;
        [SerializeField] private float fireRateInSeconds;
        [SerializeField] private ProjectileId concurrentProjectil, defauldProjectile;
        [SerializeField] private FactoryProjectilesConfiguration factoryProjectilesConfiguration;
        private float fireRateDeltaTime;
        private IShip ship;
        private bool isConfigurated;
        private FactoryProjectiles factoryProjectiles;

        public void Configure(IShip ship, ProjectileId defaultProjectile)
        {
            defauldProjectile = defaultProjectile;
            concurrentProjectil = defauldProjectile;
            this.ship = ship;
            fireRateDeltaTime = fireRateInSeconds;
            isConfigurated = true;
            factoryProjectiles = new FactoryProjectiles(Instantiate(factoryProjectilesConfiguration));
        }

        private void Update()
        {
            if (!isConfigurated) return;
            TryShoot();
            fireRateDeltaTime -= Time.deltaTime;
        }

        public void TryShoot()
        {
            if (!(fireRateDeltaTime < 0)) return;
            Shoot();
            fireRateDeltaTime = fireRateInSeconds;
        }

        private void Shoot()
        {
            var transformToPointShoot = pointToSpawnProyectil.transform;
            var projectile = factoryProjectiles.Create(concurrentProjectil.Id, 
                transformToPointShoot.position,
                transformToPointShoot.rotation);
            ship.IsShoot(projectile);
        }
    }
}