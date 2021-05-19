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

        public ShipControllerMediator Create(string id, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(configuration.FindShipById(id), position, rotation);
        }
    }
}