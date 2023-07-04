using UnityEngine;
using Utils;

public class InputUnityAdapter : IInputAdapter
{
    public InputUnityAdapter(GameObject gameObject)
    {
    }

    public Vector2 GetDirection()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public bool GetButton(string name)
    {
        return Input.GetButtonDown(name);
    }

    public bool CanMove()
    {
        return true;
    }
}