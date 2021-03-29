using UnityEngine;

public class PlayerPrefsStrategy: MonoBehaviour
{
    public IPlayerPrefsAdapter GetPlayerPrefsAdapter()
    {
        return new PlayerPrefsAdapter();
    }
}