using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoPlay : EstadosFinitos
{
    public GameObject playerLocal;
    public override void Start()
    {
        base.Start();
        //Tenemos que instanciar o player o activarlo
        playerLocal = GameObject.Find("Player");
        playerLocal.GetComponent<InitPlayer>().InitializarPlayer();
    }
    public override void Salir()
    {
        playerLocal.GetComponent<InitPlayer>().DetenerPlayer();
    }

    public override void Update()
    {

        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        //Aqui preguntamos cualquiera de las dos posibles variaciones de cambio de estado: Muerte o PasoDeNivel
        if(GameObject.FindGameObjectsWithTag("Enemigo").Length <= 0 || !playerLocal.GetComponent<ControladorDeVidasPlayer>().estaVivo)
        {
            return typeof(EstadoReinicio);
        }
        return GetType();
    }
}
