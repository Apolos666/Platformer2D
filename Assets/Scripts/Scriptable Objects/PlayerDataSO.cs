using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Player Data")]
    public class PlayerDataSO : ScriptableObject
    {
        [Header("Gravity")]
        [HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
        [HideInInspector] public float gravityScale; // Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics).
                                                     // Also the value the player's rigidbody2D.gravityScale is set to.

        [Header("Movement horizontal")]
        public float RunMaxSpeed; // Target speed we want the player to reach
        public float RunAcceleration; // The speed at which our player acceleration to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
        [HideInInspector] public float RunAccelAmount; // This variable is intended to adjust the acceleration level to adapt RunMaxSpeed.
        public float RunDecceleration; // The speed at which our player decceleration to from current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
        [HideInInspector] public float RunDeccelAmount; // This variable is intended to adjust the decceleration level to adapt RunMaxSpeed.

        [Header("Jumping")]
        public float JumpHeight; // Height of the player's jump
        public float JumpTimeToApex; // Time between jump force and reaching the desire jump height. These values also control the player's gravity and jump force.
        [HideInInspector] public float JumpForce; //The actual force applied (upwards) to the player when they jump.

        private void OnValidate()
        {        
            // Calculate gravity strength (Due to being near the ground, the acceleration is equivalent to gravity.)
            // using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
            // Apply the Kinematic Equations and Free Fall formula to calculate gravityStrength.
            gravityStrength = -(2 * JumpHeight) / (JumpTimeToApex * JumpTimeToApex);

            // Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics)
            // Divide to calculate the ratio between the character's gravity and the system's gravity.
            gravityScale = gravityStrength / Physics2D.gravity.y;

            #region "not get it"
            // Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
            // If runAcceleration is equal to runMaxSpeed, consider runAccelAmount as zero.
            RunAccelAmount = (50 * RunAcceleration) / RunMaxSpeed;  
            RunDeccelAmount = (50 * RunDecceleration) / RunMaxSpeed;

            // Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
            JumpForce = Mathf.Abs(gravityStrength) * JumpTimeToApex;
            #endregion
        }
    }

    
}
