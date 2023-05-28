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
        [SerializeField]
        private PlayerStateChecker playerStateChecker;

        [Header("Player Settings")]
        [SerializeField]
        private PlayerDataSO playerDataSO;

        // Events 
        public event Action OnJumping;

        private void Awake()
        {
        }

        private void Start()
        {
            GameInput.Instance.OnJumpingPerformed += GameInput_OnJumpingPerformed;
        }

        
        #region "Events"

        private void GameInput_OnJumpingPerformed()
        {
            if (!playerStateChecker.p_IsGrounded) return;

            HandleJumping();

            OnJumping?.Invoke();
        }

        #endregion

        private void FixedUpdate()
        {
            HandleMovementHorizontal();
        }

        /// <summary>
        /// Apply acceleration to make the character's movement feel nicer
        /// </summary>
        private void HandleMovementHorizontal()
        {
            Vector2 moveDir = GameInput.Instance.GetMovementInput2DNormalized();

            #region "Target speed"

            // Calculate the direction we want to move in and our desired velocity
            float targetSpeed = moveDir.x * playerDataSO.RunMaxSpeed;

            // Using Lerp to smooths change velocity
            // Here, we assign the value of targetSpeed to targetSpeed above.
            targetSpeed = Mathf.Lerp(playerRb.velocity.x, targetSpeed, 1);

            #endregion

            #region "Accel rate"

            // Calculate acceleration rate
            float accelRate = (Mathf.Abs(moveDir.x) > 0.01f) ? playerDataSO.RunAccelAmount : playerDataSO.RunDeccelAmount;

            #endregion

            #region "Apply the missing force"

            // Calculates the current velocity vs targetSpeed
            // To calculate how much additional force is needed for the player to reach maximum velocity.
            // If the player's current velocity is close to the maximum, just add a little bit of force; if the player's velocity is low, increase the force significantly.
            float speedDiff = targetSpeed - playerRb.velocity.x;

            // Calculates the force needed to apply to the player to reach maxSpeed.
            // Apply acceleration to create a gradual increase or decrease in velocity.
            // Add the missing force to reach the maximum velocity.
            // The movement will be equal zero when player reach max speed.
            float movement = speedDiff * accelRate;

            playerRb.AddForce(movement * Vector2.right, ForceMode.Force);

            #endregion
        }

        private void HandleJumping()
        {
            float jumpForce = playerDataSO.JumpForce;

            // Increase jumpforce if we are falling
            // This means we'll always feel like we jump the same amount
            // This realy helpful if we implement double jump or something else
            if (playerRb.velocity.y <= 0)
                jumpForce -= playerRb.velocity.y;     

            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }
    }
}
