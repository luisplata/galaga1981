using UnityEngine;
using System.Collections;
using System;

public class EstadoReinicio : EstadosFinitos
{
    public override void Start()
    {
        base.Start();
        //verificamos porque llego aqui
        if(GameObject.FindGameObjectsWithTag("Enemigo").Length <= 0)
        {
            //es por stage finalizado
            escenario.stage++;
        }
        else
        {
            //por muerte
        }
    }
    public override void Salir()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        if (GameObject.FindGameObjectsWithTag("Enemigo").Length <= 0)
        {
            return typeof(EstadoPresentacion);
        }
        else if (!GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().estaVivo)
        {
            GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().vidas--;
            //verificamos si lo mandamos a presentacion o para game over
            if (GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().vidas <= 0)
            {
                return typeof(EstadoGameOver);
            }
            else
            {
                return typeof(EstadoPresentacion);
            }
        }
        return GetType();
    }
}
