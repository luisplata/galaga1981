using System;
using NewVersion.Ship.Enemies;
using NewVersion.Weapons;
using NewVersion.Weapons.Projectiles;
using UnityEngine;
using UnityEngine.InputSystem;
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
        private Vector2 _mousePosition;
        private bool _isTapped;
        private bool _isPause = true;
        private IEnemiesSpawner _enemiesSpawner;

        private void Awake()
        {
            pointToFollow = new GameObject("pointToFollow");
            pointToFollow.transform.position = transform.position;
        }

        public void Configure(IInputAdapter typeOfInput, ProjectileId defaultProjectile, Vector2 speed, float fireRatio, IEnemiesSpawner enemiesSpawner)
        {
            movementController.Configure(this,speed);
            weaponController.Configure(this, defaultProjectile,fireRatio, enemiesSpawner);
            _inputAdapter = typeOfInput;
            isConfigure = true;
            _enemiesSpawner = enemiesSpawner;
        }

        private void Update()
        {
            if (!isConfigure) return;
            var worldPosition = _inputAdapter.GetDirection();
            if (!_inputAdapter.CanMove() || _enemiesSpawner.IsPause())
            {
                movementController.MovePointTo(Vector2.zero);
            }
            else
            {
                 movementController.MovePointTo(worldPosition);
            }
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

        public bool IsFiring()
        {
            return _inputAdapter.GetButton("Fire");
        }
        
        public void OnPoint(InputAction.CallbackContext context)
        {
            _mousePosition = context.ReadValue<Vector2>();
        }
        
        public void OnClick(InputAction.CallbackContext context)
        {
            if(context.performed)
                _isTapped = true;
            else if(context.canceled)
                _isTapped = false;
        }

        public Vector3 GetMousePosition()
        {
            return _mousePosition;
        }

        public void Pause()
        {
            _isPause = true;
        }

        public void Play()
        {
            _isPause = false;
        }
    }
}