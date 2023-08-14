using Cinemachine;
using UnityEngine;

namespace Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private CinemachineVirtualCamera _aimCam;
        private CinemachineTransposer _transposer;
        
        [Header("Movements")]
        public float speed = 2;
        public float speedUpCoefficient = 2;
        private CameraControles _controls;

        [Header("Zooming")] 
        public float zoomSpeed = 0.1f;
        public AnimationCurve yOffsetCurve;
        public AnimationCurve zOffsetCurve;
        private float zoomState = 0.5f;
        
        
        private void OnValidate()
        {
            if (!root) TryGetComponent(out root);
        }

        private void Awake()
        {
            _controls = new CameraControles();
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void Start()
        {
            _transposer = _aimCam.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void Update()
        {
            MoveCamera(_controls.Camera.Move.ReadValue<Vector2>());
            Zoom(_controls.Camera.Zooming.ReadValue<float>());
        }

        public void MoveCamera(Vector2 direction)
        {
            float speedMult = _controls.Camera.SpeedUp.IsPressed() ? speedUpCoefficient : 1;
            Vector3 newPos = new Vector3(direction.x, 0, direction.y) * (speed * Time.deltaTime * speedMult);
            newPos += root.position;
            root.position = newPos;
        }

        public void Zoom(float deltaZoom)
        {
            zoomState += deltaZoom * zoomSpeed * Time.deltaTime;
            zoomState = Mathf.Clamp01(zoomState);
            Vector3 newOffset = new Vector3
            {
                y = yOffsetCurve.Evaluate(zoomState),
                z = -zOffsetCurve.Evaluate(zoomState)
            };
            _transposer.m_FollowOffset = newOffset;
        }
    }
}