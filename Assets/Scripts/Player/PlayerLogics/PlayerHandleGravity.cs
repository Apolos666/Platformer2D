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
            SetGravityScaleRigidBody(playerDataSO.gravityScale);
        }

        /// <summary>
        /// Using custom gravity instead of system gravity.
        /// </summary>
        private void SetGravityScaleRigidBody(float scale)
        {
            #region "not get it"

            Vector3 gravity = scale * Physics.gravity;

            #endregion

            playerRb.AddForce(gravity, ForceMode.Acceleration);
        }
    }
}
