using NewVersion.Ship.Enemies;
using NewVersion.Weapons.Projectiles;
using UnityEngine;

namespace NewVersion.Weapons
{
    public class FactoryProjectiles
    {
        private readonly FactoryProjectilesConfiguration _configuration;

        public FactoryProjectiles(FactoryProjectilesConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Projectile Create(string id, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(_configuration.FindProjectilById(id),position,rotation);
        }
    }
}