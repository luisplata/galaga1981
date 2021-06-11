using UnityEngine;

namespace Utils
{
    public class InputMachine : IInputAdapter
    {
        private readonly AnimationCurve curve;

        public InputMachine(AnimationCurve curve)
        {
            this.curve = curve;
        }

        public Vector2 GetDirection()
        {
            var result = Vector2.down;
            return result;
        }

        public bool GetButton(string name)
        {
            return Random.Range(0, 100) < 20;
        }
    }
}