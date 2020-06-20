using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EstadoGameOver : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        SceneManager.LoadScene("03_Creditos");
    }
}
