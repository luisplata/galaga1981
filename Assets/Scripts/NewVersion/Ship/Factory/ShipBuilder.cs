﻿using NewVersion.Ship.Enemies;
using NewVersion.Weapons;
using UnityEngine;
using UnityEngine.Assertions;

namespace NewVersion.Ship.Factory
{
    public class ShipBuilder
    {
        private ShipControllerMediator _ship;
        private Vector3 _position = Vector3.zero;
        private Quaternion _rotation = Quaternion.identity;
        private TypeOfInput _typeOfInput = TypeOfInput.MachineInput;
        private ProjectileId _projectileId;
        private ShipToSpawnConfiguration _shipPlayerConfiguration;
        private AnimationCurve curve = AnimationCurve.Linear(1, 1, 1, 1);
        private IEnemiesSpawner _mediator;

        public ShipBuilder FromPrefab(ShipControllerMediator ship)
        {
            _ship = ship;
            return this;
        }

        public ShipBuilder WithCurve(AnimationCurve curve)
        {
            this.curve = curve;
            return this;
        }

        public ShipBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public ShipBuilder WithRotation(Quaternion rotation)
        {
            _rotation = rotation;
            return this;
        }

        public ShipBuilder WithTypeOfInput(TypeOfInput input)
        {
            _typeOfInput = input;
            return this;
        }

        public ShipBuilder WithPrefabProjectile(ProjectileId projectileId)
        {
            _projectileId = projectileId;
            return this;
        }

        public ShipBuilder WithShipConfiguration(ShipToSpawnConfiguration shipPlayerConfiguration)
        {
            _shipPlayerConfiguration = shipPlayerConfiguration;
            return this;
        }
        
        public ShipBuilder WithMediator(IEnemiesSpawner mediator)
        {
            _mediator = mediator;
            return this;
        }
    
        public ShipControllerMediator Build()
        {
            var shipControllerMediator = Object.Instantiate(_ship, _position, _rotation);
            var typeOfInput = new StrategyForInput(_typeOfInput, shipControllerMediator, curve);
            Assert.IsNotNull(_projectileId);
            shipControllerMediator.Configure(typeOfInput.GetInput(),_projectileId,_shipPlayerConfiguration.Speed,_shipPlayerConfiguration.FireRatio, _mediator);

            return shipControllerMediator;
        }
    }
}