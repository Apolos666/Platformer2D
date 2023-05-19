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

        private void FixedUpdate()
        {
            HandleMovementHorizontal();
            IsUserMoving();
            IsFacingRight();
        }

        /// <summary>
        /// Apply acceleration to make the character's movement feel nicer
        /// </summary>
        private void HandleMovementHorizontal()
        {
            #region "need to research again"
            Vector2 moveDir = GameInput.Instance.GetMovementInput2DNormalized();

            #region "Target speed"

            // Calculate the direction we want to move in and our desired velocity
            float targetSpeed = moveDir.x * playerData.RunMaxSpeed;
            Debug.Log("targetSpeed: " + targetSpeed);

            // Using Lerp to smooths change velocity
            targetSpeed = Mathf.Lerp(playerRb.velocity.x, targetSpeed, 1);

            #endregion

            #region "Accel rate"

            // Calculate acceleration rate
            float accelRate = (Mathf.Abs(moveDir.x) > 0.01f) ? playerData.RunAccelAmount : playerData.RunDeccelAmount;

            #endregion

            // Calculates the remaining speed that the character has yet to move
            float speedRemain = targetSpeed - playerRb.velocity.x;

            // Calculates the force needed to apply to the player to reach maxSpeed
            // Apply acceleration to create a gradual increase or decrease in velocity.
            float movement = speedRemain * accelRate;
            Debug.Log("Movement: " + movement);

            playerRb.AddForce(movement * Vector2.right, ForceMode.Force);

            //Debug.Log("player velocity: " + playerRb.velocity.x);
            #endregion
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
