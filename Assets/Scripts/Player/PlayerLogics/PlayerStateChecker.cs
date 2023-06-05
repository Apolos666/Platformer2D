using Askeladd.Scripts.GameManagers;
using Askeladd.Scripts.ScriptableObjects.PlayerSO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.Player.PlayerLogics
{
    public class PlayerStateChecker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PlayerHandleMovement playerHandleMovement;
        [SerializeField]
        private PlayerTimeTracker playerTimeTracker;
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
        public bool p_isFacingRight { get; private set; } = true;
        private bool _previousIsFacingRight;

        public bool p_isUserMoving { get; private set; } = false;

        public bool p_isJumping { get; private set; } = false;
        public bool p_isFalling { get; private set; } = false;
        public bool p_isJumpCut { get; private set; } 

        public bool p_IsGrounded { get; private set; }
        

        // Collider Arrays
        private Collider[] groundColliders = new Collider[10];

        private void Start()
        {
            GameInput.Instance.OnJumpingCanceled += GameInput_OnJumpingCanceled;
            playerHandleMovement.OnJumping += PlayerHandleMovement_OnJumping;
        }

        #region "Events"
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

        private void ClearArray(System.Array array, int index, int arrayLength)
        {
            System.Array.Clear(array, index, arrayLength);
        }

        /// <summary>
        /// Check the player's collider with the surrounding environment, if the player collides with any object that has a layerMask of 'ground', 
        /// then assign the value true to the variable p_IsGrounded; otherwise, assign false
        /// </summary>
        private void GroundCheckOverLapBox()
        {
            // Clear the previous array and then assign new values to that array.
            ClearArray(groundColliders, 0, groundColliders.Length);

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
            if (p_IsGrounded) { p_isJumping = false; p_isJumpCut = false; p_isFalling = false; return; }

            if (playerRb.velocity.y > 0) { p_isJumping = true; return; }

            p_isJumping = false;
            p_isFalling = true;
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
            p_isUserMoving = p_IsGrounded && GameInput.Instance.GetMovementInput2DNormalized().x != 0;

            return p_isUserMoving;
        }

        /// <summary>
        /// You can only jump when you are on the ground and the player is not in a jumping state.
        /// </summary>
        public bool CanJump()
        {
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
