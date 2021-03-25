using UnityEngine;

public class InputJoistyckAdapter : IInputAdapter
{
    public Vector2 GetDirection()
    {
        return new Vector2(SimpleInput.GetAxis("Horizontal"), SimpleInput.GetAxis("Vertical"));
    }

    public bool GetButton(string name)
    {
        return SimpleInput.GetButtonDown(name);
    }
}