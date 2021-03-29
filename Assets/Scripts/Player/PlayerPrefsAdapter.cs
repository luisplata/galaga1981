using UnityEngine;

public class PlayerPrefsAdapter : IPlayerPrefsAdapter
{
    public bool HasKey(string name)
    {
        return PlayerPrefs.HasKey(name);
    }

    public int GetInt(string name)
    {
        return PlayerPrefs.GetInt(name);
    }

    public void SetInt(string name, int input)
    {
        PlayerPrefs.SetInt(name, input);
    }
}