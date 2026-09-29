using UnityEngine;

namespace V2
{
    public class FramePacingBootstrap : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private int sleepTimeoutSeconds = -1; // -1 == SleepTimeout.NeverSleep

        private void Awake()
        {
            if (!Application.isMobilePlatform) return;
            Application.targetFrameRate = targetFrameRate;
            Screen.sleepTimeout = sleepTimeoutSeconds;
        }
    }
}