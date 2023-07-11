using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Askeladd.Scripts.Characters.Player.PlayerLogics;
using Askeladd.Scripts.ScriptableObjects.PlayerSO;
using System;
using Askeladd.Scripts.Camera;

namespace Askeladd.Scripts.Characters.Player.PlayerVisuals
{
    public class PlayerVisual : MonoBehaviour
    {
        [Header("String Name")]
        private const string IS_PLAYER_MOVING = "_IsPlayerMoving";

        [Header("References")]
        [SerializeField]
        private PlayerStateChecker _playerStateChecker;
        [SerializeField]
        private Transform _player;
        [SerializeField]
        private PlayerAnimationNameSO _playerAnimationNameSO;
        [SerializeField]
        private CameraFollowObject _cameraFollowObject;
        private Animator _animator;

        // Animation
        private int _currentState;
        private float _lockedTill;

        // Events
        public event EventHandler<OnLockedTillChangedEventArgs> OnLockedTillChanged;

        // Bools
        [SerializeField]
        private bool _isPreviousStateComboAttack;

        public class OnLockedTillChangedEventArgs : EventArgs
        {
            public float LockedTill;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _playerStateChecker.OnDirChanged += PlayerStateChecker_OnDirChanged;
        }

        private void OnDestroy()
        {
            _playerStateChecker.OnDirChanged -= PlayerStateChecker_OnDirChanged;
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
            if (_playerStateChecker.p_isComboAttack && !_isPreviousStateComboAttack) { _isPreviousStateComboAttack = true; return LockState(_playerAnimationNameSO.PlayerHeavyAttack, _playerAnimationNameSO.HeavyAttackAnimTime); }

            if (Time.time < _lockedTill) return _currentState;

            if (_playerStateChecker.p_isJumping) { _isPreviousStateComboAttack = false; return _playerAnimationNameSO.PlayerJumping; }

            if (_playerStateChecker.p_isFalling) { _isPreviousStateComboAttack = false; return _playerAnimationNameSO.PlayerFalling; }

            if (_playerStateChecker.p_isNormalAttack) { _isPreviousStateComboAttack = false; return LockState(_playerAnimationNameSO.PlayerNormalAttack, _playerAnimationNameSO.NormalAttackAnimTime); }

            if (_playerStateChecker.p_isHeavyAttack) { _isPreviousStateComboAttack = false; return LockState(_playerAnimationNameSO.PlayerHeavyAttack, _playerAnimationNameSO.HeavyAttackAnimTime); }

            if (_playerStateChecker.P_isCrouchAttack) { _isPreviousStateComboAttack = false; return LockState(_playerAnimationNameSO.PlayerCrouchAttack, _playerAnimationNameSO.CrouchAttackAnimTime); }

            if (_playerStateChecker.p_isCrouching) { _isPreviousStateComboAttack = false; return _playerAnimationNameSO.PlayerCrouching; }

            if (_playerStateChecker.p_isUserMoving) { _isPreviousStateComboAttack = false; return _playerAnimationNameSO.PlayerMoving; }

            _isPreviousStateComboAttack = false;

            return _playerAnimationNameSO.PlayerIdle; 

            int LockState(int s, float t)
            {
                _lockedTill = Time.time + t;

                OnLockedTillChanged?.Invoke(this, new OnLockedTillChangedEventArgs
                {
                    LockedTill = _lockedTill
                });

                return s;
            }
        }

        private void TurnCharacter()
        {
            Vector3 lookDirection = _playerStateChecker.IsFacingRight() ? Vector3.right : Vector3.left;

            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, lookDirection);

            _player.rotation = targetRotation;

            _cameraFollowObject.CallTurn();
        }
    }
}
