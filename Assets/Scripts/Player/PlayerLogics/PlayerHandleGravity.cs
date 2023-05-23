using Askeladd.Scripts.ScriptableObjects;
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
        private PlayerHandleMovement playerHandleMovement; // To retrieve the fields from playHandleMovement to adjust gravity accordingly.

        [Header("Player Settings")]
        [SerializeField]
        private PlayerDataSO playerDataSO;

        private void Awake()
        {
            playerHandleMovement = GetComponent<PlayerHandleMovement>();
        }

        private void FixedUpdate()
        {
            // set default gravity
            if (playerRb.velocity.y >= 0f) { SetGravityScaleRigidBody(playerDataSO.GravityScale); return;  }

            IncreaseFallingSpeed();
            LimitFallingSpeed();
        }

        /// <summary>
        /// Using custom gravity instead of system gravity.
        /// </summary>
        private void SetGravityScaleRigidBody(float scale)
        {
            Vector3 gravity = scale * Physics.gravity;

            playerRb.AddForce(gravity, ForceMode.Acceleration);
        }

        private void IncreaseFallingSpeed()
        {
            SetGravityScaleRigidBody(playerDataSO.GravityScale * playerDataSO.FallSpeedMult);
        }

        private void LimitFallingSpeed()
        {
            // if the value of playerRb.velocity.y is less than the MaxFallSpeed,
            // it will still use that value because both values are negative and the smaller negative value is actually larger.
            // However, when the value of playerRb.velocity.y is greater, it will use the MaxFallSpeed to limit the player's falling speed.
            playerRb.velocity = new Vector2(playerRb.velocity.x, Mathf.Max(playerRb.velocity.y, playerDataSO.MaxFallSpeed));
        }    
    }
}
