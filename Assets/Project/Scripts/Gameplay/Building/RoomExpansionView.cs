using System.Collections;
using MarketTycoon.Gameplay.Navigation;
using UnityEngine;

namespace MarketTycoon.Gameplay.Building
{
    /// <summary>
    /// Handles visual unlocking sequence for a new room: door animator, darkness fade, and NavMesh rebuild.
    /// </summary>
    public class RoomExpansionView : MonoBehaviour
    {
        [Header("Door Animator")]
        [SerializeField] private Animator _doorAnimator;
        [SerializeField] private string _openTriggerName = "Open";

        [Header("Darkness Overlay")]
        [Tooltip("Dark translucent cube or plane placed above the locked room.")]
        [SerializeField] private GameObject _darknessOverlay;

        [Header("Obstacles & Content")]
        [Tooltip("Door obstacle collider that blocks NavMesh before opening (if used).")]
        [SerializeField] private GameObject _doorColliderObstacle;

        [Tooltip("Container holding building spots or decorations inside the room.")]
        [SerializeField] private GameObject _roomContent;

        private void Awake()
        {
            // Initial state: room is locked
            SetLockedState(true);
        }

        public void UnlockRoom()
        {
            StartCoroutine(UnlockSequenceRoutine());
        }

        private IEnumerator UnlockSequenceRoutine()
        {
            // 1. Trigger door open animation
            if (_doorAnimator != null)
            {
                _doorAnimator.SetTrigger(_openTriggerName);
            }

            // Small delay for door animation to progress
            yield return new WaitForSeconds(0.25f);

            // 2. Disable darkness overlay and open door obstacle
            if (_darknessOverlay != null)
            {
                _darknessOverlay.SetActive(false);
            }

            if (_doorColliderObstacle != null)
            {
                _doorColliderObstacle.SetActive(false);
            }

            // 3. Enable interactive content inside the room
            if (_roomContent != null)
            {
                _roomContent.SetActive(true);
            }

            // 4. Update NavMesh so agents can navigate through the door
            if (NavMeshRuntimeUpdater.Instance != null)
            {
                NavMeshRuntimeUpdater.Instance.RebuildNavMesh();
            }
        }

        public void SetLockedState(bool isLocked)
        {
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
        }
    }
}