using System.Collections.Generic;
using UnityEngine;

namespace NewVersion.Ship.Factory
{
    [CreateAssetMenu(menuName = "Factory/Create ShipConfiguration", fileName = "ShipConfiguration", order = 0)]
    public class ShipConfiguration : ScriptableObject
    {
        [SerializeField] private List<ShipControllerMediator> ships;
        private Dictionary<string, ShipControllerMediator> mapShips;

        private void Awake()
        {
            mapShips = new Dictionary<string, ShipControllerMediator>();
            foreach (var ship in ships)
            {
                mapShips.Add(ship.Id, ship);
            }
        }

        public ShipControllerMediator FindShipById(string id)
        {
            return mapShips[id];
        }
    }
}