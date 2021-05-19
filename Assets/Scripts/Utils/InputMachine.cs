using UnityEngine;

namespace Utils
{
    public class InputMachine : IInputAdapter
    {
        public Vector2 GetDirection()
        {
            return Vector2.down;
        }

        public bool GetButton(string name)
        {
            return Random.Range(0, 100) < 20;
        }
    }
}