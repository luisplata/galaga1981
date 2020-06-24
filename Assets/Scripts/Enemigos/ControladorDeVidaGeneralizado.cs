using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorDeVidaGeneralizado : MonoBehaviour
{
    public Enemigo enemigo;
    public GameObject quienTeElimino, salidaDeSonido;
    public AudioClip explosion, disparo;
    private void Start()
    {
        enemigo = GetComponent<Enemigo>();
        salidaDeSonido = GameObject.Find("ControladorDeJuego");
        //ignoramos a todos los enemigos
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemigo"))
        {
            Physics2D.IgnoreCollision(e.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BalaPlayer"))
        {
            salidaDeSonido.GetComponent<AudioSource>().PlayOneShot(explosion);
            enemigo.estaMuerto = collision.gameObject.CompareTag("BalaPlayer");
            quienTeElimino = collision.gameObject;
        }
    }

    public void TerminoDeMorir()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemigo") || collision.transform.name == "Arriba")
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
        if (collision.transform.CompareTag("Player"))
        {
            salidaDeSonido.GetComponent<AudioSource>().PlayOneShot(explosion);
            enemigo.estaMuerto = true;
        }
    }
}
