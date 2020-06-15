using UnityEngine;
using System;

public class EstadoAtaque : EstadosFinitosEnemigo
{
    private bool tocoFondo = false;
    private bool disparara = false;
    private bool evaluarRetorno = false;
    Vector2 diff, anterior;
    public override void Start()
    {
        base.Start();
    }
    public override void Salir()
    {
        //throw new NotImplementedException();
        controladorVidas.enemigo.cansadoDeEsperar = false;
    }

    private void FixedUpdate()
    {
        anterior = transform.position;
    }
    public override void Update()
    {
        //Aqui lo que tiene que hacer es ir hacia abajo 
        if (!tocoFondo)
        {
            Vector2 velocidadDeBajda = Vector2.down * (controladorVidas.enemigo.speed*(controladorVidas.enemigo.stage)) * Time.deltaTime;
            GetComponent<Rigidbody2D>().velocity = velocidadDeBajda;
            int direccionador = diff.x < 0 ? -1 : 1;
            float angulo = Vector2.Angle(Vector2.down, transform.position) * direccionador;
            //transform.eulerAngles = new Vector3(0, 0, Vector3.forward.z * angulo);
        }
        else
        {
            //cuando toque fondo regresar al posicion original
            GetComponent<Rigidbody2D>().velocity = (controladorVidas.enemigo.estacionamiento.transform.position - transform.position) * controladorVidas.enemigo.speed * Time.deltaTime;
        }
        
        if (UnityEngine.Random.Range(1, 100) > 0 && UnityEngine.Random.Range(1, 100) <= 50 && disparara && GameObject.Find("Player").GetComponent<ControladorDeVidasPlayer>().estaVivo)
        {
            //dispara
            //disparamos
            GameObject disparoInstanciado = Instantiate(Resources.Load("Prefabs/Bala"), transform) as GameObject;
            disparoInstanciado.tag = "BalaEnemigo";
            disparoInstanciado.transform.position = new Vector2(disparoInstanciado.transform.position.x - 0.058f, disparoInstanciado.transform.position.y);
            disparoInstanciado.transform.rotation = new Quaternion(0, 0, 180, 0);
            diff = GameObject.Find("Abajo").transform.position - transform.position;
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
