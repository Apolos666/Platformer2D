using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Askeladd.Scripts.Camera
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera[] _allVirtualCamera;

        [Header("Controls for lerping the Y damping during player jump/fall")]
        [SerializeField] private float _fallYPanAmount = 0.25f;
        [SerializeField] private float _fallYPanTime = 0.35f;
        public float FallSpeedYDampingChangeThreshold = -15f;
        
        // Camera
        private CinemachineVirtualCamera _currentCamera;
        private CinemachineFramingTransposer _cinemachineFramingTransposer;

        // Coroutine
        private Coroutine _lerpYPanCoroutine;
        private Coroutine _panCameraOnContact;

        private float _normYPanAmount;
        private Vector3 _startingTrackedObjectOffset;

        // Bools
        [field: SerializeField] public bool p_isLerpYDamping { get; private set; } = false;
        [field: SerializeField] public bool p_LerpFromPlayerFalling { get; set; } = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else 
            {
                Instance = this;
            }

            for (int i = 0; i < _allVirtualCamera.Length; i++)
            {
                if (_allVirtualCamera[i].enabled)
                {
                    // Set the current to the active camera
                    _currentCamera = _allVirtualCamera[i];

                    // Set the framing transposer from current camera
                    _cinemachineFramingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                }              
            }

            // Set the Ydamping amount so it's based on the inspector value
            _normYPanAmount = _cinemachineFramingTransposer.m_YDamping;

            // Set the starting point of the object offset
            _startingTrackedObjectOffset = _cinemachineFramingTransposer.m_TrackedObjectOffset;
        }

        #region "Lerp Y Damping"
        public void LerpYDamping(bool isPlayerFalling)
        {
            if (_lerpYPanCoroutine != null)
            {
                StopCoroutine(_lerpYPanCoroutine);
            }

            _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
        }

        private IEnumerator LerpYAction(bool isPlayerFalling)
        {
            p_isLerpYDamping = true;

            // Grab the starting damping amount
            float startDamping = _cinemachineFramingTransposer.m_YDamping;
            float endDamping = 0f;

            // Determine the end damping amount
            if (isPlayerFalling)
            {
                endDamping = _fallYPanAmount;

                // Make sure we are called that once per falling not called that again when coroutine completed
                p_LerpFromPlayerFalling = true;
            }
            else
            {
                endDamping = _normYPanAmount;
            }

            // lerp the pan amount
            float elapsedTime = 0f;

            while (elapsedTime < _fallYPanTime)
            {
                elapsedTime += Time.deltaTime;

                float lerpYDamping = Mathf.Lerp(startDamping, endDamping, (elapsedTime / _fallYPanTime));
                _cinemachineFramingTransposer.m_YDamping = lerpYDamping;

                yield return null;
            }

            p_isLerpYDamping = false;
        }

        #endregion

        #region "Pan Camera On Contact"
        public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
        {
            if (_panCameraOnContact != null)
            {
                StopCoroutine(_panCameraOnContact);
            }

            _panCameraOnContact = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
        }

        private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
        {
            Vector3 endPos = Vector3.zero;
            Vector3 startingPos = Vector3.zero;
            
            // Handle pan from trigger
            if (!panToStartingPos)
            {
                switch(panDirection)
                {
                    case PanDirection.Up:
                        endPos = Vector3.up;
                        break;
                    case PanDirection.Down:
                        endPos = Vector3.down;
                        break;
                    case PanDirection.Left:
                        endPos = Vector3.left;
                        break;
                    case PanDirection.Right:
                        endPos = Vector3.right;
                        break;
                }

                endPos *= panDistance;

                startingPos = _startingTrackedObjectOffset;

                endPos += startingPos;
            } 
            // Handle the direction settings when moving back to starting  
            else
            {
                startingPos = _cinemachineFramingTransposer.m_TrackedObjectOffset;
                endPos = _startingTrackedObjectOffset;
            }

            // Handle the actual panning of the camera
            float elapsedTime = 0f;

            while (elapsedTime < panTime)
            {
                elapsedTime += Time.deltaTime;

                Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
                _cinemachineFramingTransposer.m_TrackedObjectOffset = panLerp;

                yield return null;
            }
        }

        #endregion

        #region "Swap Cameras"
        public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector3 triggerExitDirection)
        {
            if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
            {
                // deactive the old camera
                cameraFromLeft.enabled = false;

                // active the new camera
                cameraFromRight.enabled = true;

                // set the new camera as the current camera
                _currentCamera = cameraFromRight;

                // Update our composer variable
                _cinemachineFramingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
            else if (_currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
            {
                // deactive the old camera
                cameraFromRight.enabled = false;

                // active the new camera
                cameraFromLeft.enabled = true;

                // set the new camera as the current camera
                _currentCamera = cameraFromLeft;

                // Update our composer variable
                _cinemachineFramingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }

        }
        #endregion
    }
}
