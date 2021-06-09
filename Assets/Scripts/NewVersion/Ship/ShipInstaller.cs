using System;
using System.Collections;
using System.Collections.Generic;
using NewVersion.Ship;
using NewVersion.Ship.Factory;
using NewVersion.Weapons;
using UnityEngine;

public class ShipInstaller : MonoBehaviour
{
    [SerializeField] private ShipToSpawnConfiguration shipPlayerConfiguration;
    [SerializeField] private ShipConfiguration shipConfiguration;
    private ShipControllerMediator ship;

    private void Awake()
    {
        var shipFactory = new ShipFactory(Instantiate(shipConfiguration));

        var shipBuilder = shipFactory.Create(shipPlayerConfiguration.ShipId.Id);
        shipBuilder.WithShipConfiguration(shipPlayerConfiguration)
            .WithTypeOfInput(TypeOfInput.TouchInput)
            .WithPrefabProjectile(shipPlayerConfiguration.ProjectileId);

        ship = shipBuilder.Build();
    }
}
