using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxController : MonoBehaviour
{
    public RawImage capaFondo, capaSuperior;
    float velocidadSup, velocidadFond;
    // Update is called once per frame
    void Update()
    {

        velocidadSup = capaSuperior.uvRect.y + Time.deltaTime;
        velocidadFond = (capaSuperior.uvRect.y + Time.deltaTime)/2;
        capaSuperior.uvRect = new Rect(capaSuperior.uvRect.x, velocidadSup, capaSuperior.uvRect.width,capaSuperior.uvRect.height);
        capaFondo.uvRect = new Rect(capaFondo.uvRect.x, velocidadFond, capaFondo.uvRect.width, capaFondo.uvRect.height);
    }
}
