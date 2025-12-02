using System.Threading.Tasks;
using _Project.Networking.Client;
using UnityEngine;

namespace _Project.Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton _clientSingletonPrefab;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isDedicatedServer)
        {

            if (isDedicatedServer)
            {
                // DEDICATED SERVER
            }
            else
            {
                // HOST CLIENT
                ClientSingleton clientSingleton = Instantiate(_clientSingletonPrefab);
                await clientSingleton.CreateClient();
            }
        }
    }
}