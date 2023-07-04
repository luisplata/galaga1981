using System;
using NewVersion.Ship.Factory;
using UnityEngine;

namespace NewVersion.Ship.Enemies
{
    public class EnemiesSpawner : MonoBehaviour, IEnemiesSpawner
    {
        [SerializeField] private LevelConfiguration levelConfiguration;
        [SerializeField] private ShipConfiguration shipConfiguration;
        [SerializeField] private Transform spawnLocation;
        private ShipFactory factory;
        private float currentTimeInSeconds;
        private int configurationIndex;
        private bool _isPaused = true;

        private void Awake()
        {
            factory = new ShipFactory(Instantiate(shipConfiguration));
        }

        private void Update()
        {
            if (_isPaused) return;
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
                var shipBuilder = factory.Create(shipConfigurationLocal.ShipId.Id)
                    .WithPosition(spawnLocation.position)
                    .WithRotation(spawnLocation.rotation)
                    .WithTypeOfInput(TypeOfInput.MachineInput)
                    .WithShipConfiguration(shipConfigurationLocal)
                    .WithPrefabProjectile(shipConfigurationLocal.ProjectileId)
                    .WithCurve(shipConfigurationLocal.Curve)
                    .WithMediator(this);
                var enemiShip = shipBuilder.Build();
            }
        }

        public void StartSpawning()
        {
            _isPaused = false;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Play()
        {
            _isPaused = false;
        }

        public bool IsPause()
        {
            return _isPaused;
        }
    }
}