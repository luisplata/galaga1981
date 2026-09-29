using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace V2
{
    /// <summary>
    /// Drag relativo 1:1 a partir del Pointer (unifica touch primario y mouse).
    /// Zona = toda la pantalla. El `canceled` del press (dedo que sale de pantalla) => DragEnded.
    /// </summary>
    public class InputDrag : MonoBehaviour
    {
        public event Action DragStarted;
        public event Action DragEnded;

        public bool IsDragging { get; private set; }
        public Vector2 DragScreenDelta { get; private set; }

        [SerializeField] private InputActionAsset inputAsset;

        private InputAction dragPosition;
        private InputAction dragPress;
        private Vector2 touchStartScreen;

        private void Awake()
        {
            dragPosition = inputAsset.FindAction("Game/DragPosition");
            dragPress = inputAsset.FindAction("Game/DragPress");
        }

        private void OnEnable()
        {
            dragPosition.Enable();
            dragPress.Enable();
            dragPress.performed += OnPressPerformed;
            dragPress.canceled += OnPressCanceled;
            dragPosition.performed += OnPositionPerformed;
        }

        private void OnDisable()
        {
            dragPress.performed -= OnPressPerformed;
            dragPress.canceled -= OnPressCanceled;
            dragPosition.performed -= OnPositionPerformed;
            dragPosition.Disable();
            dragPress.Disable();
            if (IsDragging)
            {
                IsDragging = false;
                DragEnded?.Invoke();
            }
        }

        private void OnPressPerformed(InputAction.CallbackContext context)
        {
            touchStartScreen = dragPosition.ReadValue<Vector2>();
            DragScreenDelta = Vector2.zero;
            IsDragging = true;
            DragStarted?.Invoke();
        }

        private void OnPressCanceled(InputAction.CallbackContext context)
        {
            IsDragging = false;
            DragEnded?.Invoke();
        }

        private void OnPositionPerformed(InputAction.CallbackContext context)
        {
            if (!IsDragging) return;
            DragScreenDelta = context.ReadValue<Vector2>() - touchStartScreen;
        }
    }
}