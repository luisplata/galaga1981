using UnityEngine;
using System.Collections;
using System;

public class EstadoReinicio : EstadosFinitos
{
    //todos los enemigos estan en estado de stay?
    public bool estanTodosLosEnemigosEsperando = false;
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
        estanTodosLosEnemigosEsperando = true;
    }
    public override void Salir()
    {
        //throw new NotImplementedException();
    }

    public override void Update()
    {
        foreach(GameObject enemigo in GameObject.FindGameObjectsWithTag("Enemigo"))
        {
            if (!enemigo.GetComponent<EstadoEsperar>())
            {
                estanTodosLosEnemigosEsperando = false;
                break;
            }
            else
            {
                estanTodosLosEnemigosEsperando = true;
            }
        }
        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        if (GameObject.FindGameObjectsWithTag("Enemigo").Length <= 0)
        {
            return typeof(EstadoPresentacion);
        }
        else if (!GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().EstaVivo)
        {
            
            //verificamos si lo mandamos a presentacion o para game over
            if (GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().Vidas <= 0)
            {
                return typeof(EstadoGameOver);
            }
            else
            {
                if (estanTodosLosEnemigosEsperando)
                {
                    GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().Vidas--;
                    return typeof(EstadoPresentacion);
                }
            }
        }
        return GetType();
    }
}
