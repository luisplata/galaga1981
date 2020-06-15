using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorDeGrupos : MonoBehaviour
{
    int generales, capitanes, obreros, ordenLayer;
    List<Vector2> posiciones;
    private void Start()
    {
        posiciones = new List<Vector2>();
        //llenamos la lista
        ordenLayer = 2;
    }
    //el objetivo de este controlador es que cuando inicie ejecute una serie de corutinas llenando de enemigos
    public void Comenzar(int stage)
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
        StartCoroutine(LlenandoDinamicamente(stage));
    }
    public void MandarAtacarAlgunosEnemigos(int stage)
    {
        int cantidadDeEnemigos = GameObject.FindGameObjectsWithTag("Enemigo").Length;
        //va a tomar una cantidad
        for(int i = 0; i< (stage * 2); i++)
        {
            //los enemigos
            int enemigoNum = Random.Range(0, cantidadDeEnemigos);
            GameObject.FindGameObjectsWithTag("Enemigo")[enemigoNum].GetComponent<ControladorDeVidaGeneralizado>().enemigo.cansadoDeEsperar = true;
        }
    }
    IEnumerator LlenandoDinamicamente(int stage)
    {
        //Queremos cambiar la ubicacion del controlador para que salgan desde varias partes del mapa

        while (capitanes < 16)
        {
            yield return new WaitForSeconds(0.2f);
            GameObject oo = Instantiate(Resources.Load("Prefabs/Capitan"), transform) as GameObject;
            capitanes++;
            oo.GetComponent<Renderer>().sortingOrder = ordenLayer;
            ordenLayer++;
            oo.GetComponent<Enemigo>().stage = stage;
            oo.AddComponent(typeof(EstadoPasarela));
        }
        
        while(generales < 4)
        {
            yield return new WaitForSeconds(0.2f);
            //comenzamos a instanciar enemigos y los mandamos
            GameObject o = Instantiate(Resources.Load("Prefabs/General"), transform) as GameObject;
            generales++;
            o.GetComponent<Renderer>().sortingOrder = ordenLayer;
            ordenLayer++;
            o.GetComponent<Enemigo>().stage = stage;
            o.AddComponent(typeof(EstadoPasarela));

        }

        while (obreros < 20)
        {
            yield return new WaitForSeconds(0.2f);
            //comenzamos a instanciar enemigos y los mandamos
            GameObject o = Instantiate(Resources.Load("Prefabs/Obrero"), transform) as GameObject;
            obreros++;
            o.GetComponent<Renderer>().sortingOrder = ordenLayer;
            ordenLayer++;
            o.GetComponent<Enemigo>().stage = stage;
            o.AddComponent(typeof(EstadoPasarela));

        }
        
    }
}
