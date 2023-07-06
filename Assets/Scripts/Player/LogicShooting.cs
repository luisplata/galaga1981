public class LogicShooting
{
    private IControllerShoot controllerMov;
    private int _maxOfBullets = 2;

    public LogicShooting(IControllerShoot controllerMov)
    {
        this.controllerMov = controllerMov;
    }
    
    public void AddMaxOfBullets(int count)
    {
        _maxOfBullets += count;
    }
    
    public void IsPlayerFire(bool isButtonFire, int countBullet)
    {
        if (isButtonFire && countBullet < _maxOfBullets)
        {
            controllerMov.PlayerShootingBullet();
        }
    }
}