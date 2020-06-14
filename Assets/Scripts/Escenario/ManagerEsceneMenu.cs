using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerEsceneMenu : MonoBehaviour
{
    public TextMeshProUGUI clickToContinue;
    private float parpadeo = 1;
    private float detalTime = 0;
    private bool parpadeobol = false;
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
    }

    public void InicioGame()
    {
        SceneManager.LoadScene("Game");
    }
}
