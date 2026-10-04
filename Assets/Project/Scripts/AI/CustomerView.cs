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
            _agent.stoppingDistance = 0.25f;
            _agent.acceleration = 12f;
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
            if (!_agent.pathPending)
            {
                if(_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}
