using UnityEngine;

namespace MarketTycoon.Gameplay.CameraControl
{
    [RequireComponent(typeof(Camera))]
    public class RTSCameraController : MonoBehaviour
    {
        [Header("Keys & Target")]
        [SerializeField] private KeyCode _resetKey = KeyCode.Space;
        [SerializeField] private Transform _focusTarget;

        [Header("Drag Movement (RMB / MMB)")]
        [SerializeField] private bool _enableDrag = true;
        [SerializeField] private float _dragSensitivity = 0.025f;

        [Header("Edge Scrolling (Screen Borders)")]
        [SerializeField] private bool _enableEdgeScrolling = true;
        [SerializeField] private float _edgeScrollSpeed = 14f;
        [SerializeField] private float _edgeBorderThickness = 20f;

        [Header("Zoom Settings")]
        [SerializeField] private float _zoomSensitivity = 4f;
        [SerializeField] private float _minHeight = 4f;
        [SerializeField] private float _maxHeight = 30f;

        [Header("Smooth Damping")]
        [Range(0.01f, 0.3f)]
        [SerializeField] private float _smoothTime = 0.08f;

        private Vector3 _targetPosition;
        private Vector3 _defaultPosition;
        private Vector3 _lastMousePos;
        private Vector3 _currentVelocity;
        private bool _isDragging;

        private void Awake()
        {
            QualitySettings.vSyncCount = 1;

            _defaultPosition = transform.position;
            _targetPosition = transform.position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_resetKey))
            {
                ResetCamera();
            }

            if (_enableDrag)
            {
                if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    _isDragging = true;
                    _lastMousePos = Input.mousePosition;
                }

                if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
                {
                    _isDragging = false;
                }
            }

            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                Vector3 zoomStep = transform.forward * (scroll * _zoomSensitivity * 10f);
                Vector3 prospective = _targetPosition + zoomStep;

                if (prospective.y >= _minHeight && prospective.y <= _maxHeight)
                {
                    _targetPosition = prospective;
                }
            }

            Vector3 camRight = transform.right;
            Vector3 camFloorForward = Vector3.Cross(camRight, Vector3.up).normalized;

            if (_isDragging)
            {
                Vector3 currentMousePos = Input.mousePosition;
                Vector3 mouseDelta = currentMousePos - _lastMousePos;
                _lastMousePos = currentMousePos;

                if (mouseDelta.sqrMagnitude > 0.001f)
                {
                    Vector3 moveOffset = (-camRight * mouseDelta.x + -camFloorForward * mouseDelta.y) * _dragSensitivity;
                    _targetPosition += moveOffset;
                }
            }
            else if (_enableEdgeScrolling)
            {
                HandleEdgeScrolling(camRight, camFloorForward);
            }
        }

        private void LateUpdate()
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                _targetPosition,
                ref _currentVelocity,
                _smoothTime
            );
        }

        private void HandleEdgeScrolling(Vector3 camRight, Vector3 camFloorForward)
        {
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height)
            {
                return;
            }

            Vector3 edgeDirection = Vector3.zero;

            if (mousePos.x >= Screen.width - _edgeBorderThickness) edgeDirection += camRight;
            if (mousePos.x <= _edgeBorderThickness) edgeDirection -= camRight;
            if (mousePos.y >= Screen.height - _edgeBorderThickness) edgeDirection += camFloorForward;
            if (mousePos.y <= _edgeBorderThickness) edgeDirection -= camFloorForward;

            if (edgeDirection.sqrMagnitude > 0.001f)
            {
                edgeDirection.Normalize();
                _targetPosition += edgeDirection * (_edgeScrollSpeed * Time.deltaTime);
            }
        }

        public void ResetCamera()
        {
            _isDragging = false;
            _currentVelocity = Vector3.zero;

            if (_focusTarget != null)
            {
                Vector3 floorCenter = new Vector3(_focusTarget.position.x, 0f, _focusTarget.position.z);
                Vector3 cameraOffset = _defaultPosition - Vector3.zero;
                _targetPosition = floorCenter + cameraOffset;
            }
            else
            {
                _targetPosition = _defaultPosition;
            }
        }
    }
}