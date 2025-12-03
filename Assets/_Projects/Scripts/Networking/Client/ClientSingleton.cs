using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Networking.Client
{
    public class ClientSingleton : MonoBehaviour
    {
        private static ClientSingleton instance;

        public ClientManager ClientManager { get; private set; }

        public static ClientSingleton Instance
        {
            get
            {
                if (instance != null) { return instance; }

                instance = FindAnyObjectByType<ClientSingleton>();

                if (instance == null)
                {
                    Debug.LogError("No ClientSingleton in the scene!");
                    return null;
                }

                return instance;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async UniTask<bool> CreateClient()
        {
            ClientManager = new ClientManager();
            return await ClientManager.InitAsync();
        }

        private void OnDestroy()
        {
            ClientManager?.Dispose();
        }
    }
}