using UnityEngine;
using Unity.AI.Navigation;

namespace MarketTycoon.Gameplay.Navigation 
{
    public class NavMeshRuntimeUpdater : MonoBehaviour
    {
        [Header("NavMesh Surface")]
        [SerializeField] private NavMeshSurface _navMeshSurface;

        public static NavMeshRuntimeUpdater Instance { get; private set; }

        private void Awake()
        {
           if(Instance !=null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
           Instance = this;
           
           if(_navMeshSurface == null)
            {
                _navMeshSurface = FindFirstObjectByType<NavMeshSurface>();
            }
        }

        [ContextMenu("Rebuild NavMesh")]
        public void RebuildNavMesh()
        {
            if(_navMeshSurface != null)
            {
                _navMeshSurface.BuildNavMesh();
                Debug.Log("[NavMeshRuntimeUpdater] NavMesh rebuilt successfully.");
            }
            else
            {
                Debug.LogWarning("[NavMeshRuntimeUpdater] Cannot rebuild: NavMeshSurface is missing!");
            }
        }
    }
}



