using UnityEngine;
using UnityEngine.AI;

namespace MarketTycoon.Gameplay.AI
{
    public class CustomerView : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = 0.4f;
            _agent.radius = 0.25f; // Compact radius allows closer queue positioning
            _agent.acceleration = 14f;
            _agent.autoBraking = true;
        }

        public void MoveTo(Vector3 targetPosition)
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.SetDestination(targetPosition);
            }
        }

        public bool HasReachedDestination()
        {
            if (_agent == null || !_agent.isOnNavMesh) return true;

            if (!_agent.pathPending)
            {
                // Standard arrival check with slight tolerance
                if (_agent.remainingDistance <= _agent.stoppingDistance + 0.25f)
                {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }

                // Queue check: customer is close enough to target and stopped behind another customer
                if (_agent.remainingDistance <= 0.85f && _agent.velocity.sqrMagnitude < 0.05f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}