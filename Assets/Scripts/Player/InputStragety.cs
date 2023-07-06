using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class InputStragety : MonoBehaviour
{
    [SerializeField] private GameObject Joistick, Buttons;
    public Vector2 direcctionLocal;
    public bool fire;
    public IInputAdapter GetInput()
    {
#if UNITY_EDITOR
        return new InputUnityAdapter(this);
#else
        return new InputJoistyckAdapter();
#endif
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        direcctionLocal = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        fire = context.performed;
    }
}