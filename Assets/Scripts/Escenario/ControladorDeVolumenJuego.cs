using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControladorDeVolumenJuego : MonoBehaviour
{

    public AudioSource audio;
    public Slider slider;
    private void Start()
    {
        if (PlayerPrefs.HasKey("volumenGeneral"))
        {
            audio.volume = PlayerPrefs.GetFloat("volumenGeneral");
        }
    }
}
