using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace _Projects.SkillSystem
{
    public class FakeBoxSkillController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas _fakeBoxCanvas;
        [SerializeField] private Collider _collider;
        [SerializeField] private RectTransform _animationTarget;
        [SerializeField] private float _animationDuration;
        
        private Tween _animationTween;
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                SetOwnerVisualsRPC();
            }    
        }
        
        [Rpc(SendTo.Owner)]
        private void SetOwnerVisualsRPC()
        {
            _fakeBoxCanvas.gameObject.SetActive(true);
            _fakeBoxCanvas.worldCamera = Camera.main;
            _collider.enabled = false;
            _animationTween = _animationTarget.DOAnchorPosY(-.5f,_animationDuration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        public override void OnNetworkDespawn()
        {
            _animationTween.Kill();
        }
    }
}