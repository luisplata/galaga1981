using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorDeVidaGeneralizado : MonoBehaviour, IControllerLifeOfEnemyView
{
    public Enemigo enemigo;
    public GameObject salidaDeSonido;
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
        if (CollisionWithEverythinOfPlayer(collision.CompareTag("BalaPlayer")))
        {
            Destroy(collision.gameObject);
        }
    }

    private bool CollisionWithEverythinOfPlayer(bool isConllision)
    {
        if (isConllision)
        {
            PlayDestroyEnemy();
        }

        return isConllision;
    }

    private void PlayDestroyEnemy()
    {
        salidaDeSonido.GetComponent<AudioSource>().PlayOneShot(explosion);
        enemigo.estaMuerto = true;
    }

    public void TerminoDeMorir()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IsOtherEnemy(collision.transform.CompareTag("Enemigo"), collision.transform.name == "Arriba", collision.gameObject);
        CollisionWithEverythinOfPlayer(collision.transform.CompareTag("Player"));
    }


    private void IsOtherEnemy(bool isEnemy, bool isTop, GameObject go)
    {
        if (isEnemy || isTop)
        {
            Physics2D.IgnoreCollision(go.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
}