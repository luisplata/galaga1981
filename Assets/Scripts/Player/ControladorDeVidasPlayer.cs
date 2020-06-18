using UnityEngine;
using System.Collections;

public class ControladorDeVidasPlayer : MonoBehaviour
{
    public bool estaVivo = true;
    public int vidas = 2;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Murio(collision.gameObject);
    }

    private void Murio(GameObject collision)
    {
        if (collision.CompareTag("Enemigo") || collision.CompareTag("BalaEnemigo"))
        {
            GetComponent<Animator>().SetBool("estaMuerto", true);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<ControladorDeMovimiento>().enabled = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Murio(collision.gameObject);
    }

    public void MatarPlayer()
    {
        estaVivo = false;
    }

    public void Init()
    {
        estaVivo = true;
        GetComponent<Animator>().SetBool("estaMuerto", false);
    }
}
