using System;
using System.Collections;
using System.Collections.Generic;
using Askeladd.Scripts.Characters.Player.PlayerLogics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Askeladd.Scripts.Characters.Player.PlayerLogics
{
    public class PlayerLunchClimbHook : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private PlayerDrawProjectile _playerDrawProjectile;
        [SerializeField] private GameObject _climbingHookGameObject;

        private Transform _startTransformClimbHook;
        private Rigidbody _climbHookRigidBody;
        private GameObject _parentGameObjectClimbHook;
        [SerializeField] private Vector3 _holderClimbHookPosition;

        private void Start()
        {
            _playerDrawProjectile.LunchClimbingHook += PlayerDrawProjectile_LunchClimbingHook;
            _playerDrawProjectile.DrawClimbHookLine += PlayerDrawProjectile_DrawClimbHookLine;
        }

        private void PlayerDrawProjectile_LunchClimbingHook(Vector3 startVelocity, float throwStregth)
        {
            LunchClimbingHook(startVelocity, throwStregth);
        }

        private void PlayerDrawProjectile_DrawClimbHookLine()
        {
            PrepareForLunch();
        }

        private void Awake()
        {
            _startTransformClimbHook = _climbingHookGameObject.transform;
            _climbHookRigidBody = _climbingHookGameObject.GetComponent<Rigidbody>();
            _parentGameObjectClimbHook = _climbingHookGameObject.transform.parent.gameObject;
            _climbingHookGameObject.SetActive(false);
        }
        
        private void PrepareForLunch()
        {
            _climbingHookGameObject.transform.SetParent(_parentGameObjectClimbHook.transform);
            _climbingHookGameObject.transform.localPosition = _holderClimbHookPosition;
            _climbHookRigidBody.isKinematic = true;
            _climbingHookGameObject.SetActive(false);
        }
        
        private void LunchClimbingHook(Vector3 startVelocity, float throwStregth)
        {
            _climbingHookGameObject.SetActive(true);
            _climbingHookGameObject.transform.SetParent(null);
            
            Vector3 initVelocity = startVelocity * throwStregth / _climbHookRigidBody.mass;

            _climbHookRigidBody.isKinematic = false;
            _climbHookRigidBody.velocity = Vector3.zero;
            _climbHookRigidBody.angularVelocity = Vector3.zero;
            
            _climbHookRigidBody.AddForce(initVelocity, ForceMode.Impulse);
        }
    }
}
