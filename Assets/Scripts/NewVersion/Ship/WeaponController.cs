using NewVersion.Ship.Enemies;
using NewVersion.Weapons;
using UnityEngine;

namespace NewVersion.Ship
{
    public class WeaponController:MonoBehaviour
    {
        [SerializeField] private GameObject pointToSpawnProyectil;
        [SerializeField] private ProjectileId concurrentProjectil, defauldProjectile;
        [SerializeField] private FactoryProjectilesConfiguration factoryProjectilesConfiguration;
        private float fireRateDeltaTime;
        private IShip ship;
        private bool isConfigurated;
        private FactoryProjectiles factoryProjectiles;
        private float fireRateInSeconds;
        private IEnemiesSpawner _enemiesSpawner;

        public void Configure(IShip ship, ProjectileId defaultProjectile, float fireRatio, IEnemiesSpawner enemiesSpawner)
        {
            defauldProjectile = defaultProjectile;
            concurrentProjectil = defauldProjectile;
            this.ship = ship;
            fireRateInSeconds = fireRatio;
            fireRateDeltaTime = fireRateInSeconds;
            isConfigurated = true;
            factoryProjectiles = new FactoryProjectiles(Instantiate(factoryProjectilesConfiguration));
            _enemiesSpawner = enemiesSpawner;
        }

        private void Update()
        {
            if (!isConfigurated) return;
            if(_enemiesSpawner.IsPause()) return;
            fireRateDeltaTime -= Time.deltaTime;
        }

        public void TryShoot()
        {
            if(_enemiesSpawner.IsPause()) return;
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
            projectile.AddingMediator(_enemiesSpawner);
            ship.IsShoot(projectile);
        }
    }
}