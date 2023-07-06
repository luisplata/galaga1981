using UnityEngine;

public class ControllerOfPowerUpMoreSpeed : ControllerOfPowerUp
{
    [SerializeField] private float speedMore = 1;
    protected override void OnPlayerCollision(GameObject player)
    {
        player.GetComponent<ControladorDeMovimiento>().AddSpeed(speedMore);
    }
}