using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ControladorDeCreditosScore : MonoBehaviour
{
    public TextMeshProUGUI score, nombre, ranking;
    public TMP_InputField nombreDeJugador;
    public GameObject button, jugar;
    // Start is called before the first frame update
    void Start()
    {
        CrearScore();
        button.SetActive(false);
    }


    public void CrearScore()
    {
        score.text = "Cargando";
        nombre.text = "...";
        ranking.text = "";
        StartCoroutine(GetRequest("https://juegos.peryloth.com/api/" + "score/best/galaga"));
    }

    public void RegistrarNombre()
    {
        StartCoroutine(Upload());
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

                ranking.text = "Error";
                score.text = "En El";
                nombre.text = "Servidor";
            }
            else
            {
                score.text = "";
                nombre.text = "";
                ranking.text = "";
                Debug.Log(")))))" + webRequest.downloadHandler.text);
                Score[] s = JsonHelper.FromJson<Score>(webRequest.downloadHandler.text);
                int count = 1;
                foreach (Score sco in s)
                {
                    ranking.text += count + "\n";
                    score.text += sco.score + "\n";
                    nombre.text += sco.nombre + "\n";
                    Debug.Log(sco.nombre + " => " + sco.score);
                    count++;
                }
                button.SetActive(true);
            }
        }
    }
    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("nombre", nombreDeJugador.text);
        int score;
        //lo sacamos de las preferencias
        if (!PlayerPrefs.HasKey("score"))
        {
            score = 0;
        }
        else
        {
            score = PlayerPrefs.GetInt("score");
        }
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post("https://juegos.peryloth.com/api/" + "guardarData/galaga", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                CrearScore();
                PlayerPrefs.SetInt("score", 0);
                button.SetActive(false);
                jugar.SetActive(true);
            }
        }
    }

    public void JugarDeNuevo()
    {
        SceneManager.LoadScene("01_Menu");
    }
}
