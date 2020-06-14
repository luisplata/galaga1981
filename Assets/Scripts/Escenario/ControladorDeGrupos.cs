using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorDeGrupos : MonoBehaviour
{
    int generales, capitanes, obreros;
    //el objetivo de este controlador es que cuando inicie ejecute una serie de corutinas llenando de enemigos
    public void Comenzar()
    {
        generales = 0;
        capitanes = 0;
        obreros = 0;
        //Vamos a buscar por tag segun el tipo del enemigo
        GameObject[] Capitanes = GameObject.FindGameObjectsWithTag("Capitan");
        //Despues iteramos y encontramos el que este disponible
        foreach (GameObject target in Capitanes)
        {
            target.GetComponent<MovimientoDeFormacionDeEnemigos>().estaDisponible = true;
        }
        GameObject[] Generales = GameObject.FindGameObjectsWithTag("General");
        //Despues iteramos y encontramos el que este disponible
        foreach (GameObject target in Generales)
        {
            target.GetComponent<MovimientoDeFormacionDeEnemigos>().estaDisponible = true;
        }
        GameObject[] Obreros = GameObject.FindGameObjectsWithTag("Obrero");
        //Despues iteramos y encontramos el que este disponible
        foreach (GameObject target in Obreros)
        {
            target.GetComponent<MovimientoDeFormacionDeEnemigos>().estaDisponible = true;
        }
        StartCoroutine(LlenandoDinamicamente());
    }

    IEnumerator LlenandoDinamicamente()
    {
        while (capitanes < 16)
        {
            yield return new WaitForSeconds(0.2f);
            GameObject oo = Instantiate(Resources.Load("Prefabs/Capitan"), transform) as GameObject;
            capitanes++;
            oo.AddComponent(typeof(EstadoPasarela));
        }

        while(generales < 4)
        {
            yield return new WaitForSeconds(0.2f);
            //comenzamos a instanciar enemigos y los mandamos
            GameObject o = Instantiate(Resources.Load("Prefabs/General"), transform) as GameObject;
            generales++;
            o.AddComponent(typeof(EstadoPasarela));

        }

        while (obreros < 20)
        {
            yield return new WaitForSeconds(0.2f);
            //comenzamos a instanciar enemigos y los mandamos
            GameObject o = Instantiate(Resources.Load("Prefabs/Obrero"), transform) as GameObject;
            obreros++;
            o.AddComponent(typeof(EstadoPasarela));

        }
    }
}
