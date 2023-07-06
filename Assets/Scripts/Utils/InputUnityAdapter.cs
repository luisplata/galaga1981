using UnityEngine;
using Utils;

public class InputUnityAdapter : IInputAdapter
{
    private readonly InputStragety _inputStragety;

    public InputUnityAdapter(InputStragety inputStragety)
    {
        _inputStragety = inputStragety;
    }

    public Vector2 GetDirection()
    {
        return _inputStragety.direcctionLocal;
    }

    public bool GetButton(string name)
    {
        return SimpleInput.GetButtonDown(name);
    }

    public bool CanMove()
    {
        return true;
    }
}