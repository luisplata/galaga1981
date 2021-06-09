using System;
using NewVersion.Weapons;
using NewVersion.Weapons.Projectiles;
using UnityEngine;
using Utils;

namespace NewVersion.Ship
{
    public class ShipControllerMediator : MonoBehaviour, IShip
    {
        [SerializeField] private ShipId id;
        [SerializeField] private MovementController movementController;
        [SerializeField] private WeaponController weaponController;
        public GameObject PointToFollow => pointToFollow;
        private GameObject pointToFollow;
        private IInputAdapter _inputAdapter;
        private bool isConfigure;

        private void Awake()
        {
            pointToFollow = new GameObject("pointToFollow");
            pointToFollow.transform.position = transform.position;
        }

        public void Configure(IInputAdapter typeOfInput, ProjectileId defaultProjectile, Vector2 speed, float fireRatio)
        {
            movementController.Configure(this,speed);
            weaponController.Configure(this, defaultProjectile,fireRatio);
            _inputAdapter = typeOfInput;
            isConfigure = true;
        }

        private void Update()
        {
            if (!isConfigure) return;
            
            var worldPosition = _inputAdapter.GetDirection();
            movementController.MovePointTo(worldPosition);
            if (_inputAdapter.GetButton("Fire"))
            {
                weaponController.TryShoot();
            }
        }

        public string Id => id.Id;

        public void IsMovement()
        {
        
        }

        public void IsShoot(Projectile projectile)
        {
        
        }
    }
}