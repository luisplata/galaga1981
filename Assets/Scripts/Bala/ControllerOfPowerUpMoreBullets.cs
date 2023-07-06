using System;
using UnityEngine;

public class ControllerOfPowerUpMoreBullets : ControllerOfPowerUp
{
    [SerializeField] private int countOfBulletsMore = 1;
    protected override void OnPlayerCollision(GameObject player)
    {
        var controllerShooting = player.GetComponent<ControllerShooting>();
        //add bullets
        controllerShooting.AddBullets(countOfBulletsMore);
    }
}