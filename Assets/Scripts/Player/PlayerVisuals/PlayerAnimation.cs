using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Askeladd.Scripts.Player.PlayerLogics;

namespace Askeladd.Scripts.Player.PlayerVisuals
{
    public class PlayerAnimation : MonoBehaviour
    {
        [Header("String Name")]
        private const string IS_PLAYER_MOVING = "_IsPlayerMoving";

        [Header("References")]
        [SerializeField]
        private PlayerHandleMovement playerHandleMovement;
        [SerializeField]
        private Transform player;

        private Animator _animator;      

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            SetAnimation();
            TurnCharacter();
        }

        private void TurnCharacter()
        {
            if (playerHandleMovement.p_IsFacingRight)
                player.rotation = Quaternion.Euler(0f, 0f, 0f); 
            else
                player.rotation = Quaternion.Euler(0f, 180f, 0f); 
        }

        private void SetAnimation()
        {
            _animator.SetBool(IS_PLAYER_MOVING, playerHandleMovement.p_IsMoving);
        }
    }
}
