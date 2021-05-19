using UnityEngine;

namespace Utils
{
    public interface IInputAdapter
    {
        Vector2 GetDirection();
        bool GetButton(string name);
    }
}