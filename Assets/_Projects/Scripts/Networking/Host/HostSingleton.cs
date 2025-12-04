using UnityEngine;

namespace _Projects.Networking.Host
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton instance;

        public HostManager HostManager { get; private set; }

        public static HostSingleton Instance
        {
            get
            {
                if (instance != null) { return instance; }

                instance = FindAnyObjectByType<HostSingleton>();

                if (instance == null)
                {
                    Debug.LogError("No HostSingleton in the scene!");
                    return null;
                }

                return instance;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void CreateHost()
        {
            HostManager = new HostManager();
        }

        private void OnDestroy()
        {
            HostManager?.Dispose();
        }
    }
}