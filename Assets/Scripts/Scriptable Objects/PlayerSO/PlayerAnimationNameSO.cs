using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.ScriptableObjects.PlayerSO
{
    [CreateAssetMenu(fileName = "Player Animation Name", menuName = "Player/Player Animation Name")]
    public class PlayerAnimationNameSO : ScriptableObject
    {
        public readonly int PlayerIdle = Animator.StringToHash("Player_Idle");
        public readonly int PlayerMoving = Animator.StringToHash("Player_Moving");
        public readonly int PlayerJumping = Animator.StringToHash("Player_Jumping");
        public readonly int PlayerFalling = Animator.StringToHash("Player_Falling");
        public readonly int PlayerCrouching = Animator.StringToHash("Player_Crouching");
        public readonly int PlayerNormalAttack = Animator.StringToHash("Player_Normal_Attack");
        public readonly int PlayerHeavyAttack = Animator.StringToHash("Player_Heavy_Attack");

        public float NormalAttackAnimTime;
        public float HeavyAttackAnimTime;
    }
}
