using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace MarketTycoon.Gameplay.Checkout
{
    public class CheckoutView : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private CheckoutConfigSO _config;

        [Header("Points")]
        [SerializeField] private Transform _paymentStandPoint;
        [SerializeField] private Transform[] _queueSlots;

        [Header("Cashier Animation Clips")]
        [SerializeField] private Animator _cashierAnimator;
        [SerializeField] private AnimationClip _cashierIdleClip;
        [SerializeField] private AnimationClip _cashierEmoteYesClip;

        private PlayableGraph _playableGraph;

        public CheckoutConfigSO Config => _config;
        public Transform PaymentStandPoint => _paymentStandPoint;

        private void Start()
        {
            if (_cashierAnimator != null && _cashierIdleClip != null)
            {
                PlayClip(_cashierIdleClip);
            }
        }

        public Vector3 GetQueueSlotPosition(int index)
        {
            if (_queueSlots != null && index >= 0 && index < _queueSlots.Length)
            {
                return _queueSlots[index].position;
            }

            return _paymentStandPoint != null ? _paymentStandPoint.position : transform.position;
        }

        public void PlayPaymentSuccessEmote()
        {
            if (_cashierAnimator != null && _cashierEmoteYesClip != null)
            {
                PlayClip(_cashierEmoteYesClip);

                // Return to idle after emote finishes
                CancelInvoke(nameof(ResetCashierToIdle));
                Invoke(nameof(ResetCashierToIdle), _cashierEmoteYesClip.length);
            }
        }

        public void ResetCashierToIdle()
        {
            if (_cashierAnimator != null && _cashierIdleClip != null)
            {
                PlayClip(_cashierIdleClip);
            }
        }

        private void PlayClip(AnimationClip clip)
        {
            if (clip == null || _cashierAnimator == null) return;

            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }

            AnimationPlayableUtilities.PlayClip(_cashierAnimator, clip, out _playableGraph);
        }

        private void OnDestroy()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }
        }
    }
}