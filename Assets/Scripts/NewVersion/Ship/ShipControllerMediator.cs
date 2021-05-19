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
        [SerializeField] private TypeOfInput typeInput;
        private GameObject pointToFollow;
        private IInputAdapter _inputAdapter;
        private bool isConfigure;

        public void Configure(TypeOfInput typeOfInput, ProjectileId defaultProjectile)
        {
            pointToFollow = new GameObject("pointToFollow");
            pointToFollow.transform.position = transform.position;
            typeInput = typeOfInput;
            movementController.Configure(this);
            weaponController.Configure(this, defaultProjectile);
            _inputAdapter = GetInput();
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

        public IInputAdapter GetInput()
        {
            switch (typeInput)
            {
                case TypeOfInput.TouchInput:
                    return new InputTouchAdapter(Camera.main, pointToFollow, gameObject);
                case TypeOfInput.MachineInput:
                    return new InputMachine();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void IsMovement()
        {
        
        }

        public void IsShoot(Projectile projectile)
        {
        
        }
    }
}