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
    List<FuncionDeMovimiento> listaDeMovimientos;
    FuncionDeMovimiento movimiento;
    Vector2 diff;
    private Rigidbody2D rigidbody2D1;
    private GameObject player;

    public override void Start()
    {
        player = GameObject.Find("Player");
        rigidbody2D1 = GetComponent<Rigidbody2D>();
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
        var speedEnemi = (controladorVidas.enemigo.speed * (controladorVidas.enemigo.stage <= 1 ? 0.5f : Mathf.Log(controladorVidas.enemigo.stage)) * Time.deltaTime);
        //var speedEnemi = controladorVidas.enemigo.speed * Mathf.Max(1, controladorVidas.enemigo.stage) * Time.deltaTime;
        if (!tocoFondo)
        {
            Vector2 velocidadDeBajda = Vector2.down * (speedEnemi);
            var variableInY = movimiento.EjecutarFuncion(deltaTimeLocal);
            //Debug.Log(variableInY);
            //transform.Rotate(new Vector3(0,0,variableInY));
            velocidadDeBajda.x = variableInY;
            rigidbody2D1.velocity = velocidadDeBajda;
        }
        else
        {
            //cuando toque fondo regresar al posicion original
            rigidbody2D1.velocity = (controladorVidas.enemigo.estacionamiento.transform.position - transform.position) * (controladorVidas.enemigo.speed * (controladorVidas.enemigo.stage == 0 ? 1 : controladorVidas.enemigo.stage) * Time.deltaTime);
        }


        if (UnityEngine.Random.Range(1, 100) > 0 && UnityEngine.Random.Range(1, 100) <= 50 && disparara && player.GetComponent<ControladorDeVidasPlayer>().EstaVivo)
        {
            //dispara
            //disparamos
            if (Instantiate(Resources.Load("Prefabs/Bala"), transform) is GameObject disparoInstanciado)
            {
                disparoInstanciado.tag = "BalaEnemigo";
                var position = disparoInstanciado.transform.position;
                position = new Vector2(position.x - 0.058f, position.y);
                disparoInstanciado.transform.position = position;
                disparoInstanciado.transform.rotation = new Quaternion(0, 0, 180, 0);
                var velocidad = (player.transform.position - transform.position).normalized * (speedEnemi * 2);
                disparoInstanciado.GetComponent<Rigidbody2D>().velocity = velocidad;
                disparoInstanciado.transform.parent = null;
            }

            disparara = false;
            controladorVidas.salidaDeSonido.GetComponent<AudioSource>().PlayOneShot(controladorVidas.disparo);
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
        if (!collision.transform.CompareTag("Respawn") || disparara) return;
        disparara = true;
        evaluarRetorno = true;
    }

}
