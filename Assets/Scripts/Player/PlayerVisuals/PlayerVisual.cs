using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Askeladd.Scripts.Player.PlayerLogics;

namespace Askeladd.Scripts.Player.PlayerVisuals
{
    public class PlayerVisual : MonoBehaviour
    {
        [Header("String Name")]
        private const string IS_PLAYER_MOVING = "_IsPlayerMoving";

        [Header("References")]
        [SerializeField]
        private PlayerStateChecker playerStateChecker;
        [SerializeField]
        private Transform player;

        private Animator _animator;      

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            playerStateChecker.OnDirChanged += PlayerStateChecker_OnDirChanged;
        }

        private void PlayerStateChecker_OnDirChanged()
        {
            TurnCharacter();
        }

        private void Update()
        {
            SetAnimation();          
        }

        private void TurnCharacter()
        {
            Vector3 lookDirection = playerStateChecker.IsFacingRight() ? Vector3.right : Vector3.left;

            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, lookDirection);

            player.rotation = targetRotation;
        }

        private void SetAnimation()
        {
            _animator.SetBool(IS_PLAYER_MOVING, playerStateChecker.p_isUserMoving);
        }
    }
}
