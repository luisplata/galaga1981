using UnityEngine;
using System.Collections;

public class ControladorDeMusicaDeMenu : MonoBehaviour
{
    public AudioClip intro;
    public AudioSource salida, salida2;
    private float deltaTimeLocal;
    // Update is called once per frame
    void Update()
    {   
        deltaTimeLocal += Time.deltaTime;
        if(salida.time + Time.deltaTime >= salida.clip.length)
        {
            deltaTimeLocal = 0;
            salida.enabled = false;
            salida2.gameObject.SetActive(true);
        }
    }
}
