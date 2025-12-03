using _Project.Networking.Client;
using _Project.Networking.Host;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private HostSingleton _hostSingletonPrefab;
        [SerializeField] private ClientSingleton _clientSingletonPrefab;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }

        private async UniTask LaunchInMode(bool isDedicatedServer)
        {
            if (isDedicatedServer)
            {
                // DEDICATED SERVER
            }
            else
            {
                // HOST CLIENT
                HostSingleton hostSingletonInstance = Instantiate(_hostSingletonPrefab);
                hostSingletonInstance.CreateHost();

                ClientSingleton clientSingletonInstance = Instantiate(_clientSingletonPrefab);
                bool isAuthenticated = await clientSingletonInstance.CreateClient();

                if (isAuthenticated)
                {
                    clientSingletonInstance.ClientManager.GoToMainMenu();
                }
            }
        }
    }
}