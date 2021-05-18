using System;
using System.Collections.Generic;
using System.Linq;
using NewVersion.Weapons.Projectiles;
using UnityEngine;

namespace NewVersion.Weapons
{
    [CreateAssetMenu(menuName = "Factory/Create FactoryProjectiles", fileName = "FactoryProjectilesConfiguration", order = 0)]
    public class FactoryProjectilesConfiguration : ScriptableObject
    {
        [SerializeField] private List<Projectile> projectiles;
        private Dictionary<string, Projectile> _mapProyectiles;

        private void Awake()
        {
            _mapProyectiles = new Dictionary<string, Projectile>();
            foreach (var projectile in projectiles)
            {
                _mapProyectiles.Add(projectile.Id, projectile);
            }
        }

        public Projectile FindProjectilById(string id)
        {
            return _mapProyectiles.First(pair => pair.Key == id).Value;
        }
    }
}