using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

namespace Askeladd.Scripts.Camera
{
    public class CameraControlTrigger : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";

        public CustomInspectorObjects CustomInspectorObjects;

        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>(); 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
            {
                if (CustomInspectorObjects.PanCameraOnContact)
                {
                    // Pan the camera
                    CameraManager.Instance.PanCameraOnContact(CustomInspectorObjects.PanDistance, CustomInspectorObjects.PanTime, CustomInspectorObjects.PanDirection, false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
            {
                Vector3 exitDirection = (other.transform.position - _boxCollider.bounds.center).normalized;

                if (CustomInspectorObjects.SwapCameras && CustomInspectorObjects.CameraOnLeft != null && CustomInspectorObjects.CameraOnRight != null)
                {
                    CameraManager.Instance.SwapCamera(CustomInspectorObjects.CameraOnLeft, CustomInspectorObjects.CameraOnRight, exitDirection);
                }
                
                if (CustomInspectorObjects.PanCameraOnContact)
                {
                    // Pan the camera
                    CameraManager.Instance.PanCameraOnContact(CustomInspectorObjects.PanDistance, CustomInspectorObjects.PanTime, CustomInspectorObjects.PanDirection, true);
                }
            }
        }
    }

    [System.Serializable]
    public class CustomInspectorObjects
    {
        public bool SwapCameras = false;
        public bool PanCameraOnContact = false;

        [HideInInspector] public CinemachineVirtualCamera CameraOnLeft;
        [HideInInspector] public CinemachineVirtualCamera CameraOnRight;

        [HideInInspector] public PanDirection PanDirection;
        [HideInInspector] public float PanDistance = 2f;
        [HideInInspector] public float PanTime = 0.35f;
    }

    public enum PanDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(CameraControlTrigger))]
    [CanEditMultipleObjects]
    public class MyScriptEditor : Editor
    {
        private CameraControlTrigger _cameraControlTrigger;

        private void OnEnable()
        {
            _cameraControlTrigger = (CameraControlTrigger)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_cameraControlTrigger.CustomInspectorObjects.SwapCameras)
            {
                _cameraControlTrigger.CustomInspectorObjects.CameraOnLeft = EditorGUILayout.ObjectField("Camera On Left", _cameraControlTrigger.CustomInspectorObjects.CameraOnLeft,
                    typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

                _cameraControlTrigger.CustomInspectorObjects.CameraOnRight = EditorGUILayout.ObjectField("Camera On Right", _cameraControlTrigger.CustomInspectorObjects.CameraOnRight,
                   typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            }

            if (_cameraControlTrigger.CustomInspectorObjects.PanCameraOnContact)
            {
                _cameraControlTrigger.CustomInspectorObjects.PanDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction", 
                    _cameraControlTrigger.CustomInspectorObjects.PanDirection);

                _cameraControlTrigger.CustomInspectorObjects.PanDistance = EditorGUILayout.FloatField("Pan Distance", _cameraControlTrigger.CustomInspectorObjects.PanDistance);

                _cameraControlTrigger.CustomInspectorObjects.PanTime = EditorGUILayout.FloatField("Pan Time", _cameraControlTrigger.CustomInspectorObjects.PanTime);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_cameraControlTrigger);
            }
        }
    }
    #endif

}
