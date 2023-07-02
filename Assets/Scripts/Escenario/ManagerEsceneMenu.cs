using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ManagerEsceneMenu : MonoBehaviour
{
    [SerializeField] private string urlToMessage;
    public TextMeshProUGUI ranking, message;
    private float parpadeo = 1;
    private float detalTime = 0;
    private bool parpadeobol = false;
    private void Start()
    {
        ranking.text = "";
        StartCoroutine(GetRequest("https://juegos.peryloth.com/api/" + "score/best/galaga"));
        StartCoroutine(GetRequestMessage(urlToMessage));
    }

    public void InicioGame()
    {
        PlayerPrefs.SetInt("score", 0);
        SceneManager.LoadScene("02_Game");
    }
    public void InicioNewGame()
    {
        PlayerPrefs.SetInt("newScore", 0);
        SceneManager.LoadScene("022_Game");
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
                ranking.text += "1 - "+ s[0].score+ " - "+ s[0].nombre;
                Debug.Log("1 - " + s[0].score + " - " + s[0].nombre);
            }
        }
    }
    IEnumerator GetRequestMessage(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {

                message.text = "Error en el servidor";
            }
            else
            {
                message.text = "";
                Debug.Log(")))))" + webRequest.downloadHandler.text);
                message.text = webRequest.downloadHandler.text;
            }
        }
    }
}
