using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorDeOrdenamiento : MonoBehaviour
{
    public bool inicial = false;
    public string tipo;
    // Start is called before the first frame update
    void Start()
    {
        //Vamos a buscar por tag segun el tipo del enemigo
        GameObject[] formaciones = GameObject.FindGameObjectsWithTag(tipo);
        //Despues iteramos y encontramos el que este disponible
        foreach (GameObject target in formaciones)
        {
            if (target.GetComponent<ControladorDeCuadricula>().estaDisponible)
            {
                objetivo = target;
                //no lo dejamos disponible
                objetivo.GetComponent<ControladorDeCuadricula>().estaDisponible = false;
                //Cuando cambie de estado desocupa la cuadricula
                break;
            }
        }
    }
    public GameObject objetivo = null;
    // Update is called once per frame
    void Update()
    {
        if (inicial)
        {
            //Despues buscamos el vector de posicional
            Vector2 diff = objetivo.transform.position - transform.position;
            //lo mandamos haca alla
            GetComponent<Rigidbody2D>().velocity = diff;
            //mientras no este en la posicion del objetivo estará en estado formacion
            if(transform.position != objetivo.transform.position)
            {
                Debug.Log("Esta en estado Formacion");
            }
            else
            {
                Debug.Log("Esta en estado stay");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemigo"))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
}
