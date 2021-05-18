using System;
using NewVersion.Weapons;
using UnityEngine;

public class WeaponController:MonoBehaviour
{
    [SerializeField] private GameObject pointToSpawnProyectil;
    [SerializeField] private float _fireRateInSeconds;
    [SerializeField] private ProyectileId concurrentProjectil, defauldProyectile;
    [SerializeField] private FactoryProjectilesConfiguration factoryProjectilesConfiguration;
    private float _fireRateDeltaTime;
    private IPlayer _player;
    private bool isConfigurated = false;
    private FactoryProjectiles _factoryProjectiles;

    public void Configure(IPlayer player)
    {
        concurrentProjectil = defauldProyectile;
        _player = player;
        _fireRateDeltaTime = _fireRateInSeconds;
        isConfigurated = true;
        _factoryProjectiles = new FactoryProjectiles(Instantiate(factoryProjectilesConfiguration));
    }

    private void Update()
    {
        if (!isConfigurated) return;
        TryShoot();
        _fireRateDeltaTime -= Time.deltaTime;
    }

    public void TryShoot()
    {
        if (!(_fireRateDeltaTime < 0)) return;
        Shoot();
        _fireRateDeltaTime = _fireRateInSeconds;
    }

    private void Shoot()
    {
        var transformToPointShoot = pointToSpawnProyectil.transform;
        var projectile = _factoryProjectiles.Create(concurrentProjectil.Id, 
                                                    transformToPointShoot.position,
                                                    transformToPointShoot.rotation);
        _player.IsShoot(projectile);
    }
}