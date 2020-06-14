using UnityEngine;
using System.Collections;

public class ControladorDeVidasPlayer : MonoBehaviour
{
    public bool estaVivo = true;
    public int vidas = 2;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("BalaEnemigo"))
        {
            GetComponent<Animator>().SetBool("estaMuerto", true);
        }
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
