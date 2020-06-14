using UnityEngine;
using System.Collections;
using System;

public class EstadoPasarela : EstadosFinitosEnemigo
{
    public GameObject objetivo;
    public override void Start()
    {
        base.Start();
        //Vamos a buscar por tag segun el tipo del enemigo
        GameObject[] formaciones = GameObject.FindGameObjectsWithTag(GetComponent<Enemigo>().tipo);
        //Despues iteramos y encontramos el que este disponible
        foreach (GameObject target in formaciones)
        {
            if (target.GetComponent<MovimientoDeFormacionDeEnemigos>().estaDisponible)
            {
                objetivo = target;
                //no lo dejamos disponible
                objetivo.GetComponent<MovimientoDeFormacionDeEnemigos>().estaDisponible = false;
                //Cuando cambie de estado desocupa la cuadricula
                break;
            }
        }
    }
    public override void Salir()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        Vector2 diff = objetivo.transform.position - transform.position;
        //lo mandamos con velocidad fija

        //lo mandamos haca alla
        GetComponent<Rigidbody2D>().velocity = diff;
        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        if(transform.position != objetivo.transform.position)
        {
            //lo manda a Formacion
        }
        return GetType();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemigo") || collision.transform.CompareTag("Paredes"))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
}
