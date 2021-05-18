using NewVersion.Weapons;
using NewVersion.Weapons.Projectiles;

public interface IPlayer
{
    IInputAdapter GetInput();
    void IsMovement();
    void IsShoot(Projectile projectile);
}