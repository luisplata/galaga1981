using UnityEngine;
using System.Collections;
using System;

public class EstadoMuriendo : EstadosFinitosEnemigo
{
    public override void Start()
    {
        base.Start();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Animator>().SetBool("EstaMuerto", true);
        //Aumentamos la puntuacion del player
        GameObject.Find("Player").GetComponent<ControladorDePuntuacion>().AumentarPuntuacion(GetComponent<Enemigo>().valor);
        
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
        return GetType();
    }
}
