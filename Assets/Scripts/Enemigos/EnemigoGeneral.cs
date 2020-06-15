using UnityEngine;
using System.Collections;

public class EnemigoGeneral : Enemigo
{

    // Use this for initialization
    void Start()
    {
        base.valor = 10;
        base.tipo = "General";
        base.speed = 30*4;
    }
}
