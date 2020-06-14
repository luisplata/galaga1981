using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportamientoEscenario : MonoBehaviour
{
    public Escenario escenario = new Escenario();
    public GameObject panel_player;
    private void Start()
    {
        escenario.stage = 1;
    }
}
