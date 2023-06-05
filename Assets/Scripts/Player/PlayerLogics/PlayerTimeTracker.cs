using Askeladd.Scripts.GameManagers;
using Askeladd.Scripts.ScriptableObjects.PlayerSO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.Player.PlayerLogics
{
    public class PlayerTimeTracker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PlayerStateChecker playerStateChecker;
        [SerializeField]
        private PlayerDataSO playerDataSO;

        // Times
        public float p_lastOnGroundedTime { get; private set; } = 0f;  // If the player is standing on the ground, the value is greater than 0; otherwise,
                                                 // the value is smaller than 0.
        public float p_lastPressedJumpTime { get; private set; } = 0f; // If the player pressed jump button, the value is greater than 0; otherwise,
                                                                       // the value is smaller than 0.

        private void Start()
        {
            GameInput.Instance.OnJumpingPerformed += GameInput_OnJumpingPerformed;
        }

        #region "Events"

        /// <summary>
        /// Reset p_lastPressedJumpTime while pressed jump
        /// </summary>
        private void GameInput_OnJumpingPerformed()
        {
            p_lastPressedJumpTime = playerDataSO.JumpInputBufferTime;
        }

        #endregion

        private void Update()
        {
            ReduceValuesByTime();
            SetTimerValues();
        }

        /// <summary>
        /// Reduce the value of the Grace time by Time.deltaTime
        /// </summary>
        private void ReduceValuesByTime()
        {
            p_lastOnGroundedTime -= Time.deltaTime;
            p_lastPressedJumpTime -= Time.deltaTime;
        }

        // Need to change name or something
        /// Reset p_lastOnGroundTime while standing on the ground
        private void SetTimerValues()
        {
            if (playerStateChecker.p_IsGrounded) p_lastOnGroundedTime = playerDataSO.CoyoteTime;
        }

        public void SetDefaultValue()
        {
            p_lastOnGroundedTime = 0f;
            p_lastPressedJumpTime = 0f;
        }
    }
}
