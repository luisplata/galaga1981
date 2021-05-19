using System;
using System.Collections;
using System.Collections.Generic;
using NewVersion.Ship;
using NewVersion.Weapons;
using UnityEngine;

public class ShipInstaller : MonoBehaviour
{
    [SerializeField] private ShipControllerMediator ship;
    [SerializeField] private TypeOfInput type;
    [SerializeField] private ProjectileId projectileId;

    private void Awake()
    {
        ship.Configure(type,projectileId);
    }
}
