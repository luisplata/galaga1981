public class LogicShooting
{
    private IControllerShoot controllerMov;

    public LogicShooting(IControllerShoot controllerMov)
    {
        this.controllerMov = controllerMov;
    }
    public void IsPlayerFire(bool isButtonFire, int countBullet)
    {
        if (isButtonFire && countBullet < 2)
        {
            controllerMov.PlayerShootingBullet();
        }
    }
}