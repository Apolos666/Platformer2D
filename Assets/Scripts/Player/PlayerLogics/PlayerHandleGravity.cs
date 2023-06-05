using Askeladd.Scripts.GameManagers;
using Askeladd.Scripts.ScriptableObjects.PlayerSO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.Player.PlayerLogics
{
    public class PlayerHandleGravity : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Rigidbody playerRb;
        [SerializeField]
        private PlayerStateChecker playerStateChecker;

        [Header("Player Settings")]
        [SerializeField]
        private PlayerDataSO playerDataSO;

        private void FixedUpdate()
        {
            // Increase gravity if the player releases the jump button too early.
            if (playerRb.velocity.y >= 0f && playerStateChecker.p_isJumpCut) { JumpCut(); return; }

            // Reduce gravity while close to peek of jump
            if (( playerStateChecker.p_isJumping || playerStateChecker.p_isFalling ) 
                && Mathf.Abs(playerRb.velocity.y) < playerDataSO.JumpHangTimeThreshold ) { JumpHang(); return; }

            // set default gravity
            if (playerRb.velocity.y >= 0f) { SetGravityScaleRigidBody(playerDataSO.GravityScale); return;  }          

            // fast falling speed if player pressing the down key
            if (GameInput.Instance.GetMovementInput2DNormalized().y < 0) { FastFallingSpeed(); return; }

            // Increase falling speed if player are falling
            IncreaseFallingSpeed();
            LimitFallingSpeed(playerDataSO.MaxFallSpeed);          
        }

        /// <summary>
        /// Using custom gravity instead of system gravity.
        /// </summary>
        private void SetGravityScaleRigidBody(float scale)
        {
            Vector3 gravity = scale * Physics.gravity;

            playerRb.AddForce(gravity, ForceMode.Acceleration);
        }

        private void JumpHang()
        {
            SetGravityScaleRigidBody(playerDataSO.GravityScale * playerDataSO.JumpHangGravityMult);
        }

        private void FastFallingSpeed()
        {
            SetGravityScaleRigidBody(playerDataSO.GravityScale * playerDataSO.FastFallSpeedMult);
            LimitFallingSpeed(playerDataSO.MaxFastFallSpeed);
        }

        private void IncreaseFallingSpeed()
        {
            SetGravityScaleRigidBody(playerDataSO.GravityScale * playerDataSO.FallSpeedMult);
        }

        private void JumpCut()
        {
            SetGravityScaleRigidBody(playerDataSO.GravityScale * playerDataSO.JumpCutGravityMult);
            LimitFallingSpeed(playerDataSO.MaxJumpCutGravity);
        }

        private void LimitFallingSpeed(float limitFallingSpeed)
        {
            // if the value of playerRb.velocity.y is less than the MaxFallSpeed,
            // it will still use that value because both values are negative and the smaller negative value is actually larger.
            // However, when the value of playerRb.velocity.y is greater, it will use the MaxFallSpeed to limit the player's falling speed.
            playerRb.velocity = new Vector2(playerRb.velocity.x, Mathf.Max(playerRb.velocity.y, limitFallingSpeed));
        }    
    }
}
