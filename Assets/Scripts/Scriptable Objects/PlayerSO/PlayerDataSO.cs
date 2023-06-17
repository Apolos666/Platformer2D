using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.ScriptableObjects.PlayerSO
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "Player/Player Data")]
    public class PlayerDataSO : ScriptableObject
    {
        [Header("Collider")]
        public Vector3 ColliderCenterNormalState; // The normal "Center" position of the collider.
        public float ColliderRadiusNormalState; // The normal "Radius" value of the collider.
        public float ColliderHeightNormalState; // The normal "Height" value of the collider.
        public Vector3 ColliderCenterCrouchState; // The crouch "Center" position of the collider. 
        public float ColliderRadiusCrouchState; // The crouch "Radius" value of the collider.
        public float ColliderHeightCrouchState; // The crouch "Height" value of the collider.

        [Header("Checks Holders")]
        public Vector3 NormalAttackSpot; // The position holds the spot for the "attacking spot" in normal state.
        public float NormalAttackRadius; // The radius for the "attacking spot" in normal state
        public Vector3 CrouchAttackSpot; // The position holds the spot for the "crouch spot" in crouch state.
        public float CrouchAttackRadius; // The radius for the "crouch spot" in crouch state

        [Header("Gravity")]
        [HideInInspector] public float GravityStrength; // Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
        [HideInInspector] public float GravityScale; // Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics).
                                                     // Also the value the player's rigidbody2D.gravityScale is set to.
        [Header("Gravity")]
        public float FallSpeedMult; // Mult with gravity when velocity < 0 to increase falling speed
        public float MaxFallSpeed; // Limit falling speed
        [Space(20)]
        public float FastFallSpeedMult; // Mult with gravity when velocity < 0 and player.moveDir.y < 0 to fast fall
        public float MaxFastFallSpeed; // Limit fast falling speed
        [Space(20)]
        [Range(0f, 1f)]
        public float JumpHangGravityMult; // Reduce gravity while close to the apex of the jump

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
        [Space(20)]
        public float JumpCutGravityMult; // Increase graivity if player release jump button while jumping
        public float MaxJumpCutGravity; // Limit jump cut gravity
        [Space(20)]
        public float JumpHangTimeThreshold; // Speeds (close to 0) where the player will experience extra "jump hang".
                                            // The player's velocity.y is closest to 0 at the jump's apex
        public float JumpHangAccelerationMult; // Increase acceleration while close to the peek of the jump 
        public float JumpHangMaxSpeedMult; // Increase max speed while close to the peek of the jump 

        [Header("Combat")]
        public float NormalAttackCoolDown; // The next time the player can perform a normal attack is when the cooldown period is over
        public float HeavyAttackCoolDown; // The next time the player can perform a heavy attack is when the cooldown period is over
        public float CrouchAttackCoolDown; // The next time the player can perform a crouch attack is when the cooldown period is over
        public float NormalAttackDamage; // The damage player does to the enemy
        public float HeavyAttackDamage; // The damage player does to the enemy
        public float CrouchAttackDamage; // The damage player does to the enemy
        public float StunnedAfterSwingTime; // The duration player being stunned after swing sword

        [Header("Assists")]
        [Range(0.01f, 0.5f)] public float CoyoteTime; // Grace period after falling off a platform, where you can still jump
        [Range(0.01f, 0.5f)] public float JumpInputBufferTime; // Grace period after pressing jump where a jump will be automatically performed 

        private void OnValidate()
        {        
            // Calculate gravity strength (Due to being near the ground, the acceleration is equivalent to gravity.)
            // using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
            // Apply the Kinematic Equations and Free Fall formula to calculate gravityStrength.
            // Not apply to player gravity yet
            // The gravity we want to achieve in the game
            GravityStrength = -(2 * JumpHeight) / (JumpTimeToApex * JumpTimeToApex);

            // Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics)
            // Divide to calculate the ratio between the character's gravity and the system's gravity.
            // To calculate the level of gravity influence, assuming gravityStrength is greater than Physics.gravity.y, the result will be greater than 1, and vice versa. Then, we multiply gravityScale with the current gravity of Unity to create the desired gravitational force.
            // Apply to player gravity
            // gravityScale * physics.gravity.y = gravityStrength, This step calculates the ratio number that will be multiplied by physics.gravity.y to achieve the desired gravity in the game
            GravityScale = GravityStrength / Physics.gravity.y;

            
            // Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
            // The formula is used to calculate the acceleration increase by applying a coefficient divided by the maximum level, thus generating a relative acceleration level for the object(e.g., 0.7).This allows the object to gradually increase its speed using the relative acceleration value over time, rather than instantaneously.
            RunAccelAmount = (50 * RunAcceleration) / RunMaxSpeed;  
            RunDeccelAmount = (50 * RunDecceleration) / RunMaxSpeed;

            // Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
            // using the formula Accelerated Motion Equations (v = u + gt) v = 0, u = JumpForce
            JumpForce = Mathf.Abs(GravityStrength) * JumpTimeToApex;
        }
    }

    
}
