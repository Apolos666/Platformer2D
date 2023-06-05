using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Askeladd.Scripts.Player.PlayerLogics;
using Askeladd.Scripts.ScriptableObjects.PlayerSO;

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
        [SerializeField]
        private PlayerAnimationNameSO playerAnimationNameSO;
        private Animator _animator;

        // Animation
        private int _currentState;
        private float _lockedTill;

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
            var state = GetState();

            if (state == _currentState) return;

            _animator.CrossFade(state, 0, 0);
            _currentState = state;
        }

        private int GetState()
        {
            if (Time.time < _lockedTill) return _currentState;

            if (playerStateChecker.p_isJumping) return playerAnimationNameSO.PlayerJumping;

            if (playerStateChecker.p_isFalling) return playerAnimationNameSO.PlayerFalling;

            if (playerStateChecker.p_IsGrounded && playerStateChecker.p_isUserMoving) return playerAnimationNameSO.PlayerMoving;

            return playerAnimationNameSO.PlayerIdle; 

            int LockState(int s, float t)
            {
                _lockedTill = Time.time + t;
                return s;
            }
        }

        private void TurnCharacter()
        {
            Vector3 lookDirection = playerStateChecker.IsFacingRight() ? Vector3.right : Vector3.left;

            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, lookDirection);

            player.rotation = targetRotation;
        }
    }
}
