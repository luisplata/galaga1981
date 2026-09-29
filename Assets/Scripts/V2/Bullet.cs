using UnityEngine;

namespace V2
{
    /// <summary>
    /// Proyectil simple: avanza hacia +Y y se autodestruye al salir del viewport.
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 15f;
        [SerializeField] private float killY = 10f;

        public void Init(float newSpeed)
        {
            speed = newSpeed;
        }

        private void Update()
        {
            transform.position += Vector3.up * (speed * Time.deltaTime);
            if (transform.position.y > killY)
            {
                Destroy(gameObject);
            }
        }
    }
}