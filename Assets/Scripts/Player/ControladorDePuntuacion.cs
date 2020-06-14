using UnityEngine;
using System.Collections;
using TMPro;

public class ControladorDePuntuacion : MonoBehaviour
{
    public TextMeshProUGUI puntuacionUI;
    private int puntuacion;
    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.HasKey("score"))
        {
            puntuacion = PlayerPrefs.GetInt("score");
        }
        else
        {
            puntuacion = 0;
        }
        puntuacionUI.text = this.puntuacion.ToString();
    }

    public void AumentarPuntuacion(int puntuacion)
    {
        //ademas de aumentar la puntuacion actualizamos la puntuacion de la UI
        this.puntuacion += puntuacion;
        puntuacionUI.text = this.puntuacion.ToString();
        PlayerPrefs.SetInt("score", GetPuntuacion());
    }

    public int GetPuntuacion()
    {
        return this.puntuacion;
    }
}
