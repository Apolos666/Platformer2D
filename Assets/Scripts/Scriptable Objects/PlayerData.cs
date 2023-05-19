using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Movement horizontal")]
        public float RunMaxSpeed; // Target speed we want the player to reach
        public float RunAcceleration; // The speed at which our player acceleration to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
        [HideInInspector] public float RunAccelAmount; // This variable is intended to adjust the acceleration level to adapt RunMaxSpeed.
        public float RunDecceleration; // The speed at which our player decceleration to from current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
        [HideInInspector] public float RunDeccelAmount; // This variable is intended to adjust the decceleration level to adapt RunMaxSpeed.

        private void OnValidate()
        {
            // Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
            // If runAcceleration is equal to runMaxSpeed, consider runAccelAmount as zero.
            RunAccelAmount = (50 * RunAcceleration) / RunMaxSpeed;
            RunDeccelAmount = (50 * RunDecceleration) / RunMaxSpeed;
        }
    }

    
}
