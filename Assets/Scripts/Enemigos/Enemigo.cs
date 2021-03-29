using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemigo : MonoBehaviour
{
    public int valor;
    public string tipo;
    public float speed;
    public GameObject estacionamiento;
    public bool estaMuerto;
    public bool cansadoDeEsperar;
    public int stage;
}
