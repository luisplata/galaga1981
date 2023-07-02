using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button buttonConfiguration, buttonBackToMainMenu;
    [SerializeField] private GameObject panelConfiguration;

    private void Start()
    {
        buttonConfiguration.onClick.AddListener(OpenConfiguration);
        buttonBackToMainMenu.onClick.AddListener(BackToMainMenu);
        panelConfiguration.SetActive(false);
    }

    private void BackToMainMenu()
    {
        panelConfiguration.SetActive(false);
    }

    private void OpenConfiguration()
    {
        panelConfiguration.SetActive(true);
    }
}