using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EstadoPresentacion : EstadosFinitos
{
    private TextMeshProUGUI palabraCentro;
    private bool presentacionCompleta = false;
    private bool comienzaPresentacion = false;
    private float contador = 0;

    public override void Start()
    {
        base.Start();
        comportamiento.panel_player.SetActive(true);
        palabraCentro = GameObject.Find("TextoAlCentro").GetComponent<TextMeshProUGUI>();
        palabraCentro.text = "Ready?";

        //validamos si llego por muerte o stage finalizado
        if(GameObject.FindGameObjectsWithTag("Enemigo").Length <= 0)
        {
            controladorDeGrupos.Comenzar(escenario.stage);
        }
        //seteamos al estado inicial del player
        GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().Init();
        vidasUI.ActualizarVidas();
        vidasUI.ActualizarStage(escenario.stage);
    }
    public override void Salir()
    {
        GetComponent<ComportamientoEscenario>().panel_player.SetActive(false);
    }

    public override void Update()
    {
        if (input.GetButton("Fire") && !comienzaPresentacion)
        {
            //cambiamos el texto al stage y esperamos 2 seg para cambiar de estado
            comienzaPresentacion = true;
            palabraCentro.text = "Stage "+escenario.stage;
        }
        if (comienzaPresentacion)
        {
            contador += Time.deltaTime;
            if(contador >= 2)
            {
                presentacionCompleta = true;
            }
        }
        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        //throw new NotImplementedException();
        if (presentacionCompleta)
        {
            //Cambiamos el estado al de Play
            //Debug.Log("Mandamos al estado Play que aun no esta contruido");
            return typeof(EstadoPlay);
        }
        return GetType();
    }
}
