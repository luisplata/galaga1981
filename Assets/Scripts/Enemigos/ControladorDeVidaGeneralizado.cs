using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorDeVidaGeneralizado : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BalaPlayer"))
        {
            Destroy(collision.gameObject);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Animator>().SetBool("EstaMuerto", true);
            //Aumentamos la puntuacion del player
            GameObject.Find("Player").GetComponent<ControladorDePuntuacion>().AumentarPuntuacion(GetComponent<Enemigo>().valor);
        }
    }

    public void TerminoDeMorir()
    {
        Destroy(gameObject);
    }
}
