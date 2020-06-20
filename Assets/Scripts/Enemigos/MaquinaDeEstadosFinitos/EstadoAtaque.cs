using UnityEngine;
using System;
using System.Collections.Generic;

public class EstadoAtaque : EstadosFinitosEnemigo
{
    private bool tocoFondo = false;
    private bool disparara = false;
    private bool evaluarRetorno = false;
    private float deltaTimeLocal;
    public bool ordencreciente = true;
    public bool imprimir = false;
    List<FuncionDeMovimiento> listaDeMovimientos;
    FuncionDeMovimiento movimiento;
    Vector2 diff;
    public override void Start()
    {
        base.Start();
        listaDeMovimientos = new List<FuncionDeMovimiento>();
        listaDeMovimientos.Add(new FuncionDeSeno());
        listaDeMovimientos.Add(new FuncionTangenteCuadradoMenosDos());
        //buscamos uno a azar para darle movimiento al enemigo
        movimiento = listaDeMovimientos[UnityEngine.Random.Range(0, listaDeMovimientos.Count)];
        deltaTimeLocal = 0;
    }
    public override void Salir()
    {
        //throw new NotImplementedException();
        controladorVidas.enemigo.cansadoDeEsperar = false;
    }

    public override void Update()
    {

        if (deltaTimeLocal >= movimiento.limiteSuperior)
        {
            ordencreciente = false;
        }
        else if (deltaTimeLocal <= movimiento.limiteInferior)
        {
            ordencreciente = true;
        }

        if (ordencreciente)
        {
            deltaTimeLocal += Time.deltaTime;
        }
        else
        {
            deltaTimeLocal -= Time.deltaTime;
        }
        //Aqui lo que tiene que hacer es ir hacia abajo 
        if (!tocoFondo)
        {
            Vector2 velocidadDeBajda = Vector2.down * (controladorVidas.enemigo.speed*(controladorVidas.enemigo.stage == 0? 1: controladorVidas.enemigo.stage)) * Time.deltaTime;
            velocidadDeBajda.x = movimiento.EjecutarFuncion(deltaTimeLocal);
            GetComponent<Rigidbody2D>().velocity = velocidadDeBajda;
            if (imprimir)
            {
                Debug.Log("movimiento " + movimiento.GetType().ToString()+" deltaTimeLocal " + deltaTimeLocal + " velocidadDeBajda.x " + velocidadDeBajda.x);
            }
        }
        else
        {
            //cuando toque fondo regresar al posicion original
            GetComponent<Rigidbody2D>().velocity = (controladorVidas.enemigo.estacionamiento.transform.position - transform.position) * (controladorVidas.enemigo.speed * (controladorVidas.enemigo.stage == 0 ? 1 : controladorVidas.enemigo.stage)) * Time.deltaTime;
        }


        if (UnityEngine.Random.Range(1, 100) > 0 && UnityEngine.Random.Range(1, 100) <= 50 && disparara && GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().estaVivo)
        {
            //dispara
            //disparamos
            GameObject disparoInstanciado = Instantiate(Resources.Load("Prefabs/Bala"), transform) as GameObject;
            disparoInstanciado.tag = "BalaEnemigo";
            disparoInstanciado.transform.position = new Vector2(disparoInstanciado.transform.position.x - 0.058f, disparoInstanciado.transform.position.y);
            disparoInstanciado.transform.rotation = new Quaternion(0, 0, 180, 0);
            diff = GameObject.Find("Player").transform.position - transform.position;
            //buscamos el objetivo
            //le damos velocidad
            Vector2 velocidad = diff * (controladorVidas.enemigo.speed * 0.6f) * Time.deltaTime;
            disparoInstanciado.GetComponent<Rigidbody2D>().velocity = velocidad;
            disparoInstanciado.transform.parent = null;
            disparara = false;
        }
        else
        {
            disparara = false;
        }
        
        VerificarCambios();
    }

    public override Type VerficarTransiciones()
    {
        if (controladorVidas.enemigo.estaMuerto)
        {
            return typeof(EstadoMuriendo);
        }
        if((transform.position - controladorVidas.enemigo.estacionamiento.gameObject.transform.position).magnitude < 0.02f && evaluarRetorno)
        {
            return typeof(EstadoEsperar);
        }
        return GetType();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        tocoFondo = collision.gameObject.CompareTag("Paredes");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Respawn") && !disparara)
        {
            disparara = true;
            evaluarRetorno = true;
        }
    }

}
