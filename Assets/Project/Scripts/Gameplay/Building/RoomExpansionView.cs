using System.Collections;
using MarketTycoon.Gameplay.Navigation;
using UnityEngine;

namespace MarketTycoon.Gameplay.Building
{
    public class RoomExpansionView : MonoBehaviour
    {
        [Header("Door Animator")]
        [Tooltip("Animator component on the door object. The GameObject itself is never deactivated.")]
        [SerializeField] private Animator _doorAnimator;
        [SerializeField] private string _openTriggerName = "Open";

        [Header("Darkness Overlay")]
        [Tooltip("Dark translucent cover placed over the locked room.")]
        [SerializeField] private GameObject _darknessOverlay;

        [Header("Obstacles & Content")]
        [Tooltip("Invisible collider or NavMeshObstacle blocking the passage before opening.")]
        [SerializeField] private GameObject _doorColliderObstacle;

        [Tooltip("Container holding shelves and items inside the new room.")]
        [SerializeField] private GameObject _roomContent;

        [Header("Room Wander Zone")]
        [Tooltip("Wander zone collider for customers inside the new room.")]
        [SerializeField] private Collider _roomWanderZone;

        private bool _isUnlocked = false;

        private void Awake()
        {
            if (!_isUnlocked)
            {
                SetLockedState(true);
            }
        }

        public void UnlockRoom()
        {
            _isUnlocked = true;
            StopAllCoroutines();
            StartCoroutine(UnlockSequenceRoutine());
        }

        private IEnumerator UnlockSequenceRoutine()
        {
            // 1. Trigger door opening animation (object stays active permanently)
            if (_doorAnimator != null)
            {
                _doorAnimator.SetTrigger(_openTriggerName);
            }

            // Brief delay to let the door open visually
            yield return new WaitForSeconds(0.35f);

            // 2. Remove darkness overlay
            if (_darknessOverlay != null)
            {
                _darknessOverlay.SetActive(false);
            }

            // 3. Remove passage blocker so agents and players can pass
            if (_doorColliderObstacle != null)
            {
                _doorColliderObstacle.SetActive(false);
            }

            // 4. Reveal inner room content (shelves, decor)
            if (_roomContent != null)
            {
                _roomContent.SetActive(true);
            }

            // 5. Activate wander zone for customer AI
            if (_roomWanderZone != null)
            {
                _roomWanderZone.gameObject.SetActive(true);
                _roomWanderZone.enabled = true;
            }

            // 6. Update NavMesh
            if (NavMeshRuntimeUpdater.Instance != null)
            {
                NavMeshRuntimeUpdater.Instance.RebuildNavMesh();
            }
        }

        public void SetLockedState(bool isLocked)
        {
            _isUnlocked = !isLocked;

            if (_darknessOverlay != null)
            {
                _darknessOverlay.SetActive(isLocked);
            }

            if (_doorColliderObstacle != null)
            {
                _doorColliderObstacle.SetActive(isLocked);
            }

            if (_roomContent != null)
            {
                _roomContent.SetActive(!isLocked);
            }

            if (_roomWanderZone != null)
            {
                _roomWanderZone.gameObject.SetActive(!isLocked);
                _roomWanderZone.enabled = !isLocked;
            }
        }
    }
}