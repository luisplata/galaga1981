using UnityEngine;

public class ComportamientoEscenario : MonoBehaviour
{
    public Escenario escenario;
    public GameObject panel_player;

    private void Awake()
    {
        escenario = new Escenario();
    }
}
