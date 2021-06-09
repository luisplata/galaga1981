using NewVersion.Ship.Enemies;
using UnityEngine;

namespace NewVersion.Ship.Factory
{
    public class ShipFactory
    {
        private readonly ShipConfiguration configuration;

        public ShipFactory(ShipConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ShipBuilder Create(string id)
        {
            return new ShipBuilder().FromPrefab(configuration.FindShipById(id));
        }
    }
}