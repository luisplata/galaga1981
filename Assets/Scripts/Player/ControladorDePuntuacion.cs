using UnityEngine;
using System.Collections;
using TMPro;

public class ControladorDePuntuacion : MonoBehaviour, IControllerPointsView
{
    [SerializeField] private TextMeshProUGUI puntuacionUI;
    private IPlayerPrefsAdapter playerPrefsAdapter;

    private LogicPointsPlayer logicPointsPlayer;
    // Use this for initialization
    void Start()
    {
        playerPrefsAdapter = GetComponent<PlayerPrefsStrategy>().GetPlayerPrefsAdapter();
        logicPointsPlayer = new LogicPointsPlayer(this, playerPrefsAdapter);
    }

    public void ShowPuntuaction(string puntuaction)
    {
        puntuacionUI.text = puntuaction;
    }

    public void AumentarPuntuacion(int puntuacion)
    {
        logicPointsPlayer.PointsUp(puntuacion);
    }

    public int GetPuntuacion()
    {
        return logicPointsPlayer.GetPoints();
    }
}