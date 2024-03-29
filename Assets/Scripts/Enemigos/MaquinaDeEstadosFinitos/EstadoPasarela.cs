﻿using UnityEngine;
using System.Collections;
using System;

public class EstadoPasarela : EstadosFinitosEnemigo
{
    public MovimientoDeFormacionDeEnemigos objetivo;
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
                objetivo = target.GetComponent<MovimientoDeFormacionDeEnemigos>();
                //no lo dejamos disponible
                objetivo.estaDisponible = false;
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
        Vector2 diff = (objetivo.transform.position - transform.position).normalized;
        Vector2 velocity = diff * (controladorVidas.enemigo.speed * 10 * Time.deltaTime);
        GetComponent<Rigidbody2D>().velocity = velocity;
        int direccionador = diff.x < 0 ? -1 : 1;
        float angulo = Vector2.Angle(objetivo.transform.position, transform.position) * direccionador;
        //transform.eulerAngles = new Vector3(0, 0, Vector3.forward.z * angulo);
        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        //Debug.Log($"{(transform.position - objetivo.transform.position).magnitude}");
        if ((transform.position - objetivo.transform.position).magnitude < 0.2f)
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
