using UnityEngine;

public class InputStragety : MonoBehaviour
{
    [SerializeField] private GameObject Joistick, Buttons;
    public IInputAdapter GetInput()
    {
#if UNITY_ANDROID
        return new InputJoistyckAdapter();
#else
        Joistick.SetActive(false);
        Buttons.SetActive(false);
        return new InputUnityAdapter();
#endif
    }
}