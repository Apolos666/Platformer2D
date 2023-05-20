using Askeladd.Scripts.GameManagers;
using Askeladd.Scripts.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System;

namespace Askeladd.Scripts.Player.PlayerLogics
{
    public class PlayerHandleMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Rigidbody playerRb;

        [Header("Player Settings")]
        [SerializeField]
        private PlayerData playerData;

        [Description("Player States")]
        public bool p_IsMoving { get; private set; } = false;
        public bool p_IsFacingRight { get; private set; } = true;

        private void Start()
        {
            GameInput.Instance.OnJumpingPerformed += GameInput_OnJumpingPerformed;
        }

        #region "Events"

        private void GameInput_OnJumpingPerformed()
        {
            HandleJumping();
        }

        #endregion

        private void FixedUpdate()
        {
            SetGravityScale(playerData.gravityScale);
            HandleMovementHorizontal();
            IsUserMoving();
            IsFacingRight();
        }

        private void OnEnable()
        {
            // Turn off Physics/Gravity, instead of using custom gravity.
            playerRb.useGravity = false;
        }

        /// <summary>
        /// Using custom gravity instead of system gravity.
        /// </summary>
        private void SetGravityScale(float scale)
        {
            #region "not get it"

            Vector3 gravity = scale * Physics.gravity;

            #endregion

            playerRb.AddForce(gravity, ForceMode.Acceleration);
        }

        /// <summary>
        /// Apply acceleration to make the character's movement feel nicer
        /// </summary>
        private void HandleMovementHorizontal()
        {
            Vector2 moveDir = GameInput.Instance.GetMovementInput2DNormalized();

            #region "Target speed"

            // Calculate the direction we want to move in and our desired velocity
            float targetSpeed = moveDir.x * playerData.RunMaxSpeed;

            // Using Lerp to smooths change velocity
            // Here, we assign the value of targetSpeed to targetSpeed above.
            targetSpeed = Mathf.Lerp(playerRb.velocity.x, targetSpeed, 1);

            #endregion

            #region "Accel rate"

            // Calculate acceleration rate
            float accelRate = (Mathf.Abs(moveDir.x) > 0.01f) ? playerData.RunAccelAmount : playerData.RunDeccelAmount;

            #endregion

            #region "Apply the missing force"

            // Calculates the current velocity vs targetSpeed
            // To calculate how much additional force is needed for the player to reach maximum velocity.
            // If the player's current velocity is close to the maximum, just add a little bit of force; if the player's velocity is low, increase the force significantly.
            float speedDiff = targetSpeed - playerRb.velocity.x;

            // Calculates the force needed to apply to the player to reach maxSpeed
            // Apply acceleration to create a gradual increase or decrease in velocity.
            // Add the missing force to reach the maximum velocity.
            float movement = speedDiff * accelRate;

            playerRb.AddForce(movement * Vector2.right, ForceMode.Force);

            #endregion
        }

        private void HandleJumping()
        {
            float jumpForce = playerData.JumpForce;

            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }

        // Not clean yet
        private bool IsUserMoving()
        {
            p_IsMoving = GameInput.Instance.GetMovementInput2DNormalized().x != 0;
            return p_IsMoving;
        }


        // Not clean yet
        private bool IsFacingRight()
        {
            if (GameInput.Instance.GetMovementInput2DNormalized().x == 0) return p_IsFacingRight;

            p_IsFacingRight = GameInput.Instance.GetMovementInput2DNormalized().x > 0;

            return p_IsFacingRight;
        }
    }
}
