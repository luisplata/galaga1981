using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControadorDeVidasDeUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player, spriteDePlayer;
    public TextMeshProUGUI stage;

    public void ActualizarVidas()
    {
        //buscar los hijos y eliminarlos
        for(int i = 1; i <= player.GetComponent<ControladorDeVidasPlayer>().vidas; i++)
        {
            if(transform.Find("hijo") != null)
            {
                Destroy(transform.Find("hijo").gameObject);
            }
        }
        //ahora creamos los hijos necesarios
        for (int i = 1; i <= player.GetComponent<ControladorDeVidasPlayer>().vidas; i++)
        {
            GameObject vida = Instantiate(spriteDePlayer, transform);
            vida.transform.position = new Vector2(vida.transform.position.x + i, vida.transform.position.y);
            vida.name = "hijo";
        }
    }

    public void ActualizarStage(int stageActual)
    {
        stage.text = stageActual.ToString();
    }

}
