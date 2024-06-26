﻿using Cinemachine;
using UnityEngine;
using Utils;

namespace Cameras
{
    public class CameraController : Singleton<CameraController>
    {
        [SerializeField] private Transform root;
        [SerializeField] private CinemachineVirtualCamera _aimCam;
        private CinemachineTransposer _transposer;
        
        [Header("Movements")]
        public float speed = 2;
        public float speedUpCoefficient = 2;

        [Header("Zooming")] 
        public bool canZoom = true;
        public bool canMove = true;
        public float zoomSpeed = 0.1f;
        public AnimationCurve yOffsetCurve;
        public AnimationCurve zOffsetCurve;
        private float zoomState = 0.5f;
        
        
        private void OnValidate()
        {
            if (!root) TryGetComponent(out root);
        }

        private void Start()
        {
            _transposer = _aimCam.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void Update()
        {
            
        }

        public void MoveCamera(Vector2 direction, bool speedUp)
        {
            if (!canMove) return;
            
            float speedMult =  speedUp ? speedUpCoefficient : 1;
            Vector3 newPos = new Vector3(direction.x, 0, direction.y) * (speed * Time.deltaTime * speedMult);
            newPos += root.position;
            root.position = newPos;
        }

        public void Zoom(float deltaZoom)
        {
            if (!canZoom) return;
                
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