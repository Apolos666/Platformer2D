using Askeladd.Scripts.Camera;
using Askeladd.Scripts.GameManagers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Askeladd
{
    public class CameraPanOnMoveDir : MonoBehaviour
    {
        [Header("Adjust the configuration when panning the camera")]
        [SerializeField] private float _panDistance;
        [SerializeField] private float _panTime;

        private void Start()
        {
            GameInput.Instance.OnPanCameraPerformed += GameInput_OnPanCameraPerformed;
            GameInput.Instance.OnPanCameraCanceled += GameInput_OnPanCameraCanceled;
        }

        private void GameInput_OnPanCameraCanceled()
        {
            CameraManager.Instance.PanCameraOnContact(_panDistance, _panTime, PanDirection.Up, true);
        }

        private void GameInput_OnPanCameraPerformed()
        {
            switch (GameInput.Instance.GetPanCameraDirectionNormalized().y)
            {
                case 1f:
                    CameraManager.Instance.PanCameraOnContact(_panDistance, _panTime, PanDirection.Up, false);
                    break;
                case -1f:
                    CameraManager.Instance.PanCameraOnContact(_panDistance, _panTime, PanDirection.Down, false);
                    break;
            }
        }
    }    
}
