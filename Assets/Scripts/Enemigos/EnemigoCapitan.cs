using UnityEngine;
using System.Collections;

public class EnemigoCapitan : Enemigo
{

    // Use this for initialization
    void Start()
    {
        base.valor = 5;
        base.tipo = "Capitan";
        base.speed = 30*4;
    }
}
