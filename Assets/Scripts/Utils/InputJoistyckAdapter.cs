using UnityEngine;

public class InputJoistyckAdapter : IInputAdapter
{
    public Vector2 GetDirection()
    {
        var direction = new Vector2(SimpleInput.GetAxis("Horizontal"), 0);
        return direction;
    }

    public bool GetButton(string name)
    {
        return SimpleInput.GetButtonDown(name);
    }
}