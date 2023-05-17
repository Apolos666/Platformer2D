using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Askeladd.Scripts.GameManagers;
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
        private float moveSpeed = 5f;

        [Description("Player States")]
        public bool p_IsMoving { get; private set; } = false;
        public bool p_IsFacingRight { get; private set; } = true;

        private void FixedUpdate()
        {
            HandleMovementHorizontal();
            IsUserMoving();
            IsFacingRight();
        }

        private void HandleMovementHorizontal()
        {
            float moveDir = GameInput.Instance.GetMovementInput2DNormalized().x;

            playerRb.velocity = new Vector3(moveSpeed * moveDir, playerRb.velocity.y, playerRb.velocity.z);
        }

        private bool IsUserMoving()
        {
            p_IsMoving = GameInput.Instance.GetMovementInput2DNormalized().x != 0;
            return p_IsMoving;
        }

        private bool IsFacingRight()
        {
            if (GameInput.Instance.GetMovementInput2DNormalized().x == 0) return p_IsFacingRight;

            p_IsFacingRight = GameInput.Instance.GetMovementInput2DNormalized().x > 0;

            return p_IsFacingRight;
        }
    }
}
