using UnityEngine;
using System.Collections;

public class EnemigoObrero : Enemigo
{

    // Use this for initialization
    void Start()
    {
        base.valor = 5;
        base.tipo = "Obrero";
        base.speed = 30*4;
    }
}
