using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstadoPlay : EstadosFinitos
{
    public GameObject playerLocal;
    public float delta, limite;
    public override void Start()
    {
        base.Start();
        limite = 5;
        //Tenemos que instanciar o player o activarlo
        playerLocal = GameObject.Find("Player");
        playerLocal.GetComponent<InitPlayer>().InitializarPlayer();
        //movemos al player al medio
        playerLocal.transform.position = new Vector2(-2.2f, -8f);
    }
    public override void Salir()
    {
        playerLocal.GetComponent<InitPlayer>().DetenerPlayer();
    }

    public override void Update()
    {
        delta += Time.deltaTime;
        //vamos a decirle que cada cierto tiempo tome unos enemigos y los ponga a atacar
        if(delta >= limite)
        {
            delta = 0;
            controladorDeGrupos.MandarAtacarAlgunosEnemigos(escenario.stage);
        }

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
