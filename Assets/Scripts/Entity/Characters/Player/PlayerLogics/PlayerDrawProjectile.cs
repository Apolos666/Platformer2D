using System;
using System.Collections;
using System.Collections.Generic;
using Askeladd.Scripts.GameManagers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Askeladd.Scripts.Characters.Player.PlayerLogics
{
    public class PlayerDrawProjectile : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private LineRenderer _lineRenderer;

        [SerializeField] private Rigidbody _climbingHook;
        [SerializeField] private Transform _climbingHookTransformHolder;
        
        // Climbing Hook
        private Vector3 _initVelocityClimbingHook;
        public event Action<Vector3, float> LunchClimbingHook;
        public event Action DrawClimbHookLine; 
        private bool _wasDrawLine;

        [Header("Display control climbing hook")] 
        [SerializeField] private float _throwStregth;
        [Range(10, 100)]
        [SerializeField] private int _linePoint;
        [Range(0.01f, 0.25f)]
        [SerializeField] private float _timeBetweenPoints;
        [SerializeField] private LayerMask _climbingHookLayerMask;

        private void Update()
        {
            if (!Keyboard.current.rKey.isPressed && _wasDrawLine)
            {
                LunchClimbingHook?.Invoke(_initVelocityClimbingHook, _throwStregth);
            }
            
            if (Keyboard.current.rKey.isPressed)
            {
                _initVelocityClimbingHook = (UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition) -
                                             _climbingHookTransformHolder.position).normalized;
                DrawProjectileTrajectory();
                DrawClimbHookLine?.Invoke();
                _wasDrawLine = true;
            }
            else
            {
                _lineRenderer.enabled = false;
                _wasDrawLine = false;
            }
        }

        private void DrawProjectileTrajectory()
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = Mathf.CeilToInt(_linePoint / _timeBetweenPoints) + 1;
                
            Vector3 initVelocity = _initVelocityClimbingHook * _throwStregth / _climbingHook.mass;
            Vector3 initPostion = _climbingHookTransformHolder.position;

            int i = 0;
            _lineRenderer.SetPosition(i, new Vector3(initPostion.x, initPostion.y, 0));

            for (float time = 0; time < _linePoint; time += _timeBetweenPoints)
            {
                i++;
                Vector3 point = initPostion + initVelocity * time;
                point.y = initPostion.y + initVelocity.y * time + (Physics.gravity.y / 2f * time * time);
                
                _lineRenderer.SetPosition(i, new Vector3(point.x, point.y, 0));

                Vector3 lastPosition = _lineRenderer.GetPosition(i - 1);
                
                if (Physics.Raycast(new Vector3(lastPosition.x, lastPosition.y, 0), 
                        (new Vector3(point.x, point.y, 0) - new Vector3(lastPosition.x, lastPosition.y, 0)).normalized, 
                        out RaycastHit hit,
                        (new Vector3(point.x, point.y, 0) - new Vector3(lastPosition.x, lastPosition.y, 0)).magnitude,
                        _climbingHookLayerMask))
                {
                    _lineRenderer.SetPosition(i, hit.point);
                    _lineRenderer.positionCount = i + 1;
                    return;
                }
            }
        }
    }
}
