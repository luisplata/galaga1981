using UnityEngine;
using System.Collections;
using System;

public class EstadoEsperar : EstadosFinitosEnemigo
{
    public override void Start()
    {
        base.Start();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
    public override void Salir()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        Vector2 diff = controladorVidas.enemigo.estacionamiento.transform.position - transform.position;
        //lo mandamos hacia alla
        Vector2 velocity = diff * (controladorVidas.enemigo.speed * 15 * Time.deltaTime);
        GetComponent<Rigidbody2D>().velocity = velocity;
        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        if (controladorVidas.enemigo.cansadoDeEsperar)
        {
            return typeof(EstadoAtaque);
        }
        if (controladorVidas.enemigo.estaMuerto)
        {
            return typeof(EstadoMuriendo);
        }
        return GetType();
    }
}
