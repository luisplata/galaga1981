using UnityEngine;

namespace V2
{
    public class FramePacingBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            if (!Application.isMobilePlatform) return;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}