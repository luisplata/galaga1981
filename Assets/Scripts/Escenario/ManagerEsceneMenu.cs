using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ManagerEsceneMenu : MonoBehaviour
{
    public TextMeshProUGUI clickToContinue, ranking;
    private float parpadeo = 1;
    private float detalTime = 0;
    private bool parpadeobol = false;
    private void Start()
    {
        ranking.text = "";
        StartCoroutine(GetRequest("URL" + "score/best/galaga"));
    }
    // Update is called once per frame
    void Update()
    {
        detalTime += Time.deltaTime;
        if(detalTime >= parpadeo || (detalTime >= parpadeo/2 && parpadeobol))
        {
            detalTime = 0;
            clickToContinue.gameObject.SetActive(parpadeobol);
            parpadeobol = !parpadeobol;
        }
        if (Input.GetKey(KeyCode.Joystick1Button1))
        {
            InicioGame();
        }
    }

    public void InicioGame()
    {
        PlayerPrefs.SetInt("score", 0);
        SceneManager.LoadScene("02_Game");
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
}
