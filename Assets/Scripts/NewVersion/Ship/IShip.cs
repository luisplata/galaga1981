using NewVersion.Ship;
using NewVersion.Weapons;
using NewVersion.Weapons.Projectiles;
using Utils;

public interface IShip
{
    void IsMovement();
    void IsShoot(Projectile projectile);
}