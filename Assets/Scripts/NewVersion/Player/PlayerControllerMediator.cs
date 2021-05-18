using NewVersion.Weapons;
using NewVersion.Weapons.Projectiles;
using UnityEngine;

namespace NewVersion.Player
{
    public class PlayerControllerMediator : MonoBehaviour, IPlayer
    {
        [SerializeField] private MovementController movementController;
        [SerializeField] private WeaponController weaponController;
        private IInputAdapter _inputAdapter;

        private void Start()
        {
            movementController.Configure(this);
            weaponController.Configure(this);
            _inputAdapter = GetInput();
        }

        private void Update()
        {
            var worldPosition = _inputAdapter.GetDirection();
            movementController.MovePointTo(worldPosition);
            if (_inputAdapter.GetButton("Fire"))
            {
                weaponController.TryShoot();
            }
        }

        public IInputAdapter GetInput()
        {
            return new InputTouchAdapter(Camera.main);
        }

        public void IsMovement()
        {
        
        }

        public void IsShoot(Projectile projectile)
        {
        
        }
    }
}