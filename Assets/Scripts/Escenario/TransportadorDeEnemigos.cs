using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportadorDeEnemigos : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemigo"))
        {
            collision.transform.position = new Vector2(collision.transform.position.x, GameObject.Find("Arriba").transform.position.y);
        }
    }
}
