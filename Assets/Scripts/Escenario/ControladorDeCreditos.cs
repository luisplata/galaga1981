using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControladorDeCreditos : MonoBehaviour
{
    public List<TextMeshProUGUI> listaDeCreditos;
    [SerializeField]
    private int index, max;
    //cada x seg cambia
    [SerializeField]
    private float deltaTimeLocal, xSegundos;
    // Start is called before the first frame update
    void Start()
    {
        xSegundos = 5;
        max = listaDeCreditos.Count;
        Debug.Log(">>>>>>" + max);
        listaDeCreditos[0].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        deltaTimeLocal += Time.deltaTime;
        if(deltaTimeLocal >= xSegundos)
        {
            deltaTimeLocal = 0;
            listaDeCreditos[index].gameObject.SetActive(false);
            index++;
            if(index > max-1)
            {
                index = 0;
            }
            listaDeCreditos[index].gameObject.SetActive(true);
        }

    }
}
