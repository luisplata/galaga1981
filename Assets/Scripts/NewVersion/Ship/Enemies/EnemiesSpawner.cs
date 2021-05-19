using System;
using NewVersion.Ship.Factory;
using UnityEngine;

namespace NewVersion.Ship.Enemies
{
    public class EnemiesSpawner : MonoBehaviour
    {
        [SerializeField] private LevelConfiguration levelConfiguration;
        [SerializeField] private ShipConfiguration shipConfiguration;
        [SerializeField] private Transform spawnLocation;
        private ShipFactory factory;

        private float currentTimeInSeconds;
        private int configurationIndex;

        private void Awake()
        {
            factory = new ShipFactory(Instantiate(shipConfiguration));
        }

        private void Update()
        {
            if (configurationIndex >= levelConfiguration.spawnConfigurations.Length)
            {
                return;
            }
            currentTimeInSeconds += Time.deltaTime;
            var configuration = levelConfiguration.spawnConfigurations[configurationIndex];
            if (configuration.TimeToSpawn >= currentTimeInSeconds)
            {
                return;
            }

            SpawnShip(configuration);
            configurationIndex++;
        }

        private void SpawnShip(SpawnConfiguration configuration)
        {
            foreach (var shipConfigurationLocal in configuration.ShipToSpawnConfigurations)
            {
                var shipInstantiate = factory.Create(shipConfigurationLocal.ShipId.Id, spawnLocation.position,
                    spawnLocation.rotation);
                shipInstantiate.Configure(TypeOfInput.MachineInput, shipConfigurationLocal.ProjectileId);
            }
        }
    }
}