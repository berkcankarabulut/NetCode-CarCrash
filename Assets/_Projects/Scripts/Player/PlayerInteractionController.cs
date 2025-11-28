using _Project.Collect;
using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    public class PlayerInteractionController : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if(!IsOwner) return;
            ICollectible collectible = other.gameObject.GetComponent<ICollectible>();
            if (collectible == null) return;
            collectible.Collect();
        }
    }
}