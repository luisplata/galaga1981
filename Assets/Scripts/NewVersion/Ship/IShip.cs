using NewVersion.Weapons;
using NewVersion.Weapons.Projectiles;
using Utils;

public interface IShip
{
    IInputAdapter GetInput();
    void IsMovement();
    void IsShoot(Projectile projectile);
}