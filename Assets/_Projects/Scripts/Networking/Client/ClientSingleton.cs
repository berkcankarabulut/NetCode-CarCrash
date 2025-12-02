using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Networking.Client
{
    public class ClientSingleton : MonoBehaviour
    {
        private static ClientSingleton _instance;
        private ClientGameManager _gameManager;

        public static ClientSingleton Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<ClientSingleton>();
                if (_instance == null)
                {
                    Debug.LogError("No ClientSingleton found");
                    return null;
                }

                return _instance;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async UniTask CreateClient()
        { 
            _gameManager = new ClientGameManager();
            await _gameManager.InitAsync();
        }
    }
}