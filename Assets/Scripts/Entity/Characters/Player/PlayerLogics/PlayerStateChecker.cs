using Askeladd.Scripts.GameManagers;
using Askeladd.Scripts.GeneralScripts;
using Askeladd.Scripts.ScriptableObjects.PlayerSO;
using System;
using UnityEngine;
using System.Collections;
using Askeladd.Scripts.Camera;

namespace Askeladd.Scripts.Characters.Player.PlayerLogics
{
    public class PlayerStateChecker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PlayerHandleMovement playerHandleMovement;
        [SerializeField]
        private PlayerTimeTracker playerTimeTracker;
        [SerializeField]
        private PlayerHandleCombat playerHandleCombat;
        [SerializeField]
        private Rigidbody playerRb;
        [SerializeField]
        private PlayerDataSO playerDataSO;

        [Header("Transform Check Points")]
        [SerializeField]
        private Transform groundCheckTransform;
        [SerializeField]
        private Vector3 groundCheckSize;

        [Header("Layers")]
        [SerializeField]
        private LayerMask groundLayer;

        // Events
        public event Action OnDirChanged;

        // Player states
        [field:Header("Player States")]
        [field:SerializeField] public bool p_isFacingRight { get; private set; } = true;
        private bool _previousIsFacingRight;

        [field: SerializeField] public bool p_isCanMove { get; private set; } = true;
        [field: SerializeField] public bool p_isCanJump { get; private set; } = true;
        [field: SerializeField] public bool p_isUserMoving { get; private set; } = false;
        [field: SerializeField] public bool p_isCrouching { get; private set; } = false;

        [field: SerializeField] public bool p_isJumping { get; private set; } = false;
        [field: SerializeField] public bool p_isFalling { get; private set; } = false;
        [field: SerializeField] public bool p_isJumpCut { get; private set; }

        [field: SerializeField] public bool p_isNormalAttack { get; private set; } = false;
        [field: SerializeField] public bool p_isHeavyAttack { get; private set; } = false;
        [field: SerializeField] public bool P_isCrouchAttack { get; private set; } = false;
        [field: SerializeField] public bool p_isComboAttack { get; private set; } = false;

        [field: SerializeField] public bool p_IsGrounded { get; private set; }
        

        // Collider Arrays
        private Collider[] groundColliders = new Collider[10];

        // Coroutines
        private Coroutine _currentCoroutine;

        private void Start()
        {
            GameInput.Instance.OnJumpingCanceled += GameInput_OnJumpingCanceled;
            playerHandleMovement.OnJumping += PlayerHandleMovement_OnJumping;
            playerHandleMovement.OnCrouch += PlayerHandleMovement_OnCrouch;
            playerHandleMovement.OnUnCrouch += PlayerHandleMovement_OnUnCrouch;
            playerHandleCombat.OnNormalAttack += PlayerHandleCombat_OnNormalAttack;
            playerHandleCombat.OnHeavyAttack += PlayerHandleCombat_OnHeavyAttack;
            playerHandleCombat.OnCrouchAttack += PlayerHandleCombat_OnCrouchAttack;
            playerHandleCombat.OnComboAttack += PlayerHandleCombat_OnComboAttack;
            playerHandleCombat.OnNotAttack += PlayerHandleCombat_OnNotAttack;
        }    

        private void OnDestroy()
        {
            GameInput.Instance.OnJumpingCanceled -= GameInput_OnJumpingCanceled;
            playerHandleMovement.OnJumping -= PlayerHandleMovement_OnJumping;
            playerHandleMovement.OnCrouch -= PlayerHandleMovement_OnCrouch;
            playerHandleMovement.OnUnCrouch -= PlayerHandleMovement_OnUnCrouch;
            playerHandleCombat.OnNormalAttack -= PlayerHandleCombat_OnNormalAttack;
            playerHandleCombat.OnHeavyAttack -= PlayerHandleCombat_OnHeavyAttack;
            playerHandleCombat.OnCrouchAttack -= PlayerHandleCombat_OnCrouchAttack;
            playerHandleCombat.OnComboAttack -= PlayerHandleCombat_OnComboAttack;
            playerHandleCombat.OnNotAttack -= PlayerHandleCombat_OnNotAttack;
        }

        private void OnDisable()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
        }

        #region "Events"

        private void PlayerHandleCombat_OnNotAttack()
        {
            _currentCoroutine = StartCoroutine(WaitForStunned(() => 
            {
                if (p_isCrouching) return;

                p_isCanMove = true;
                p_isCanJump = true;
            }));

            p_isNormalAttack = false;
            p_isHeavyAttack = false;
            P_isCrouchAttack = false;
            p_isComboAttack = false;
        }

        private void PlayerHandleCombat_OnComboAttack()
        {
            playerRb.velocity = Vector3.zero;
            p_isComboAttack = true;
            p_isNormalAttack = false;
            p_isCanMove = false;
            p_isCanJump = false;
        }

        private void PlayerHandleCombat_OnCrouchAttack()
        {
            P_isCrouchAttack = true;
            p_isCanMove = false;
            p_isCanJump = false;
        }

        private void PlayerHandleCombat_OnHeavyAttack()
        {
            playerRb.velocity = Vector3.zero;
            p_isHeavyAttack = true;
            p_isCanMove = false;
            p_isCanJump = false;
        }

        private void PlayerHandleCombat_OnNormalAttack()
        {
            playerRb.velocity = Vector3.zero;
            p_isNormalAttack = true;
            p_isCanMove = false;
            p_isCanJump = false;
        }

        private void PlayerHandleMovement_OnUnCrouch()
        {
            p_isCrouching = false;
            p_isCanMove = true;
            p_isCanJump = true;
        }

        private void PlayerHandleMovement_OnCrouch()
        {
            if (!p_IsGrounded) return;

            playerRb.velocity = Vector3.zero;
            p_isCrouching = true;
            p_isCanMove = false;
            p_isCanJump = false;
        }
        
        private void PlayerHandleMovement_OnJumping()
        {
            p_isJumping = true;
        }

        private void GameInput_OnJumpingCanceled()
        {
            if (CanJumpCut())
                p_isJumpCut = true;
        }

        #endregion

        private void FixedUpdate()
        {
            IsFacingRight();          
            GroundCheckOverLapBox();
            IsUserMoving();
            CheckJumpingStatus();
        }    

        IEnumerator WaitForStunned(Action OnCompleted)
        {
            yield return new WaitForSeconds(playerDataSO.StunnedAfterSwingTime);

            OnCompleted?.Invoke();
        }

        /// <summary>
        /// Check the player's collider with the surrounding environment, if the player collides with any object that has a layerMask of 'ground', 
        /// then assign the value true to the variable p_IsGrounded; otherwise, assign false
        /// </summary>
        private void GroundCheckOverLapBox()
        {
            // Clear the previous array and then assign new values to that array.
            CommonFunctions.ClearArray(groundColliders, 0, groundColliders.Length);

            Physics.OverlapBoxNonAlloc(groundCheckTransform.position, groundCheckSize / 2, groundColliders, Quaternion.identity, groundLayer);

            for (int i = 0; i < groundColliders.Length; i++)
            {
                // The first value of the array will always be either grounded or null. Therefore,
                // if the first value is grounded, we do not need to loop to the end of the array.
                // If the first value is null, the rest of the array will also be null.
                if (groundColliders[i] != null) { p_IsGrounded = true; break; }

                p_IsGrounded = false;
            }
        }

        /// <summary>
        /// Check the jump states of the player
        /// </summary>
        private void CheckJumpingStatus()
        {
            if (p_IsGrounded) 
            { 
                p_isJumping = false; p_isJumpCut = false; p_isFalling = false;

                if (!CameraManager.Instance.p_isLerpYDamping && CameraManager.Instance.p_LerpFromPlayerFalling)
                {
                    // Reset so it can be called again
                    CameraManager.Instance.p_LerpFromPlayerFalling = false;
                    
                    CameraManager.Instance.LerpYDamping(false);
                }

                return; 
            }

            if (playerRb.velocity.y > 0) 
            { 
                p_isJumping = true;

                if (!CameraManager.Instance.p_isLerpYDamping && CameraManager.Instance.p_LerpFromPlayerFalling)
                {
                    // Reset so it can be called again
                    CameraManager.Instance.p_LerpFromPlayerFalling = false;

                    CameraManager.Instance.LerpYDamping(false);
                }

                return; 
            }

            p_isJumping = false;
            p_isFalling = true;

            // If we are falling past a certain speed threshold
            // LerpedFromPlayerFalling prevent when Coroutine complete then we still called that again
            if (!CameraManager.Instance.p_isLerpYDamping && playerRb.velocity.y <= CameraManager.Instance.FallSpeedYDampingChangeThreshold && !CameraManager.Instance.p_LerpFromPlayerFalling)
            {
                CameraManager.Instance.LerpYDamping(true);
            }
        }

        /// <summary>
        /// Check which direction the user is facing, if there is a change in direction, then trigger an event.
        /// </summary>
        public bool IsFacingRight()
        {
            if (GameInput.Instance.GetMovementInput2DNormalized().x == 0) return p_isFacingRight;

            _previousIsFacingRight = p_isFacingRight;

            p_isFacingRight = GameInput.Instance.GetMovementInput2DNormalized().x > 0;

            if (_previousIsFacingRight != p_isFacingRight) OnDirChanged?.Invoke();

            return p_isFacingRight;
        }

        /// <summary>
        /// If the player presses any movement button and Grounded, then p_isUserMoving is set to true; otherwise, it is set to false
        /// </summary>
        private bool IsUserMoving()
        {
            p_isUserMoving = p_IsGrounded && GameInput.Instance.GetMovementInput2DNormalized().x != 0 && playerRb.velocity.x != 0;

            return p_isUserMoving;
        }

        /// <summary>
        /// You can only jump when you are on the ground and the player is not in a jumping state.
        /// </summary>
        public bool CanJump()
        {
            if (!p_isCanJump) return p_isCanJump;

            return playerTimeTracker.p_lastOnGroundedTime > 0 && !p_isJumping;
        }

        /// <summary>
        /// You can only jump cut when you are velocity.y greater than 0 and player is in a jumping state.
        /// </summary>
        /// <returns></returns>
        private bool CanJumpCut()
        {
            return playerRb.velocity.y > 0 && p_isJumping;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheckTransform.position, groundCheckSize);
        }
    }
}
