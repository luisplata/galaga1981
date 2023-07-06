using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        //randomizamos el power up que va a salir
        var powerUp = UnityEngine.Random.Range(0, 50);
        if (powerUp == 1)
        //if (true)
        {
            List<string> powerUps = new List<string>();
            powerUps.Add("MoreBulltes");
            powerUps.Add("MoreSpeed");
            //instanciamos el power up from resources
            var powerUpInstanciado = Instantiate(Resources.Load<GameObject>("Prefabs/PowerUps/"+powerUps[UnityEngine.Random.Range(0, powerUps.Count)]));
            powerUpInstanciado.transform.position = transform.position;
            powerUpInstanciado.transform.SetParent(null);
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
        
        return GetType();
    }
}
