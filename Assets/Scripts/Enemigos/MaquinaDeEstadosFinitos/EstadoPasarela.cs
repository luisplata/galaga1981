using UnityEngine;
using System.Collections;
using System;

public class EstadoPasarela : EstadosFinitosEnemigo
{
    public GameObject objetivo;
    private bool estaMuerto;
    Vector2 anterior;
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
        controladorVidas.enemigo.estacionamiento = objetivo;
    }
    public override void Salir()
    {
        //throw new NotImplementedException();
    }
    private void FixedUpdate()
    {
        anterior = transform.position;
    }
    public override void Update()
    {
        Vector2 diff = objetivo.transform.position - transform.position;
        //lo mandamos hacia alla
        GetComponent<Rigidbody2D>().velocity = diff * (controladorVidas.enemigo.speed*(controladorVidas.enemigo.stage == 0 ? 1 : controladorVidas.enemigo.stage)) * Time.deltaTime;
        int direccionador = diff.x < 0 ? -1 : 1;
        float angulo = Vector2.Angle(objetivo.transform.position, transform.position) * direccionador;
        //transform.eulerAngles = new Vector3(0, 0, Vector3.forward.z * angulo);
        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        if ((transform.position - objetivo.transform.position).magnitude < 0.02f)
        {
            //lo manda a Formacion
            return typeof(EstadoEsperar);
        }
        if (controladorVidas.enemigo.estaMuerto)
        {
            return typeof(EstadoMuriendo);
        }
        return GetType();
    }

}
