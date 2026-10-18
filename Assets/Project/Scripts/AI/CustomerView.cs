using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace MarketTycoon.Gameplay.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CustomerView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;

        [Header("Direct Animation Clips")]
        [SerializeField] private AnimationClip _idleClip;
        [SerializeField] private AnimationClip _walkClip;
        [SerializeField] private AnimationClip _interactClip;

        private PlayableGraph _playableGraph;
        private AnimationClip _currentPlayingClip;
        private bool _isActionPlaying = false;

        private void Awake()
        {
            if (_agent == null)
            {
                _agent = GetComponent<NavMeshAgent>();
            }

            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }

            // CRITICAL: Prevent animation from overriding NavMeshAgent movement
            if (_animator != null)
            {
                _animator.applyRootMotion = false;
            }
        }

        private void Start()
        {
            PlayAnimationDirect(_idleClip);
        }

        private void Update()
        {
            if (_animator == null || _agent == null || _isActionPlaying) return;

            // Check if agent is actually navigating along a path
            bool isMoving = _agent.hasPath && _agent.remainingDistance > _agent.stoppingDistance;

            if (isMoving)
            {
                if (_currentPlayingClip != _walkClip)
                {
                    PlayAnimationDirect(_walkClip);
                }
            }
            else
            {
                if (_currentPlayingClip != _idleClip)
                {
                    PlayAnimationDirect(_idleClip);
                }
            }
        }

        public void MoveTo(Vector3 destination)
        {
            _isActionPlaying = false;

            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
                _agent.ResetPath();
                _agent.SetDestination(destination);
            }
        }

        public bool HasReachedDestination()
        {
            if (_agent == null || !_agent.isOnNavMesh) return true;

            // 1. Still computing the path - definitely not reached yet
            if (_agent.pathPending) return false;

            // 2. If agent has a valid path, check distance
            if (_agent.hasPath)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    // Reached if almost stopped or path ended
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude < 0.05f)
                    {
                        return true;
                    }
                }
                return false;
            }

            // 3. Fallback: if no path exists, agent is idling
            return true;
        }

        public void PlayInteract()
        {
            _isActionPlaying = true;

            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
            }

            PlayAnimationDirect(_interactClip);
        }

        public void ResetLocomotion()
        {
            _isActionPlaying = false;

            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
            }

            PlayAnimationDirect(_idleClip);
        }

        private void PlayAnimationDirect(AnimationClip clip)
        {
            if (clip == null || _animator == null) return;

            _currentPlayingClip = clip;

            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }

            AnimationPlayableUtilities.PlayClip(_animator, clip, out _playableGraph);

            // Re-enforce disabling root motion on each graph init
            _animator.applyRootMotion = false;
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