using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ControadorDeVidasDeUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player, spriteDePlayer;
    public TextMeshProUGUI stage, ranking;
    private void Start()
    {

        ranking.text = "";
        StartCoroutine(GetRequest("https://juegos.peryloth.com/api/" + "score/best/galaga"));
    }

    public void ActualizarVidas()
    {
        //buscar los hijos y eliminarlos
        for(int i = 0; i < GameObject.FindGameObjectsWithTag("Vidas").Length; i++)
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
            vida.tag = "Vidas";
            vida.name = "hijo";
        }
    }

    public void ActualizarStage(int stageActual)
    {
        stage.text = stageActual.ToString();
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {

                ranking.text = "Error en el servidor";
            }
            else
            {
                ranking.text = "";
                Debug.Log(")))))" + webRequest.downloadHandler.text);
                Score[] s = JsonHelper.FromJson<Score>(webRequest.downloadHandler.text);
                ranking.text = s[0].score.ToString();
                Debug.Log("1 - " + s[0].score + " - " + s[0].nombre);
            }
        }
    }

}
