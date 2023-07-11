using Askeladd.Scripts.Characters.Interfaces;
using Askeladd.Scripts.Characters.Player.PlayerVisuals;
using Askeladd.Scripts.GameManagers;
using Askeladd.Scripts.GeneralScripts;
using Askeladd.Scripts.ScriptableObjects.PlayerSO;
using System;
using UnityEngine;
using System.Collections;

namespace Askeladd.Scripts.Characters.Player.PlayerLogics
{
    public enum PlayerCombatState
    {
        NotAttack,
        NormalAttack,
        HeavyAttack,
        CrouchAttack,
        ComboAttack        
    }

    public class PlayerHandleCombat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PlayerStateChecker playerStateChecker;
        [SerializeField]
        private PlayerTimeTracker playerTimeTracker;
        [SerializeField]
        private PlayerVisual playerVisual;
        [SerializeField]
        private PlayerDataSO playerDataSO;

        [Header("Combat Check Points")]
        [SerializeField]
        private Transform attackSpotSphere;
        [SerializeField]
        private float attackSpotSphereRadius;

        [Header("Layers")]
        [SerializeField]
        private LayerMask enemiesLayer;

        // Enemies Colliders
        private Collider[] _enemiesColliders = new Collider[10];

        // Coroutines
        private Coroutine _currentCoroutine;

        // Events
        public event Action OnNormalAttack;
        public event Action OnHeavyAttack;
        public event Action OnCrouchAttack;
        public event Action OnComboAttack;
        public event Action OnNotAttack;
        
        private PlayerCombatState _currentState = PlayerCombatState.NotAttack;
        private PlayerCombatState _previousState = PlayerCombatState.NotAttack;

        // Animation time
        private float _lockedTillChanged;
        private bool _isLockedTillChanged;

        private void Start()
        {
            GameInput.Instance.OnNormalAttackPerformed += GameInput_OnNormalAttackPerformed;
            GameInput.Instance.OnHeavyAttackPerformed += GameInput_OnHeavyAttackPerformed;
            GameInput.Instance.OnCrouchAttackPerformed += GameInput_OnCrouchAttackPerformed;
            GameInput.Instance.OnComboAttackPerformed += GameInput_OnComboAttackPerformed;
            playerVisual.OnLockedTillChanged += PlayerVisual_OnLockedTillChanged;
        }

        private void OnDestroy()
        {
            GameInput.Instance.OnNormalAttackPerformed -= GameInput_OnNormalAttackPerformed;
            GameInput.Instance.OnHeavyAttackPerformed -= GameInput_OnHeavyAttackPerformed;
            GameInput.Instance.OnCrouchAttackPerformed -= GameInput_OnCrouchAttackPerformed;
            GameInput.Instance.OnComboAttackPerformed -= GameInput_OnComboAttackPerformed;
            playerVisual.OnLockedTillChanged -= PlayerVisual_OnLockedTillChanged;
        }
        private void OnDisable()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
        }

        private void GameInput_OnComboAttackPerformed()
        {
            if (_currentState != PlayerCombatState.NormalAttack) return;

            _currentState = PlayerCombatState.ComboAttack;
        }

        private void GameInput_OnCrouchAttackPerformed()
        {
            if (!playerStateChecker.p_isCrouching) return;

            if (playerTimeTracker.p_attackCoolDown > 0) return;

            _currentState = PlayerCombatState.CrouchAttack;
        }

        private void PlayerVisual_OnLockedTillChanged(object sender, PlayerVisual.OnLockedTillChangedEventArgs e)
        {
            _lockedTillChanged = e.LockedTill;
            _isLockedTillChanged = true;
        }

        private void GameInput_OnHeavyAttackPerformed()
        {
            if (!playerStateChecker.p_IsGrounded) return;

            if (playerStateChecker.p_isCrouching) return;

            if (playerTimeTracker.p_attackCoolDown > 0) return;

            _currentState = PlayerCombatState.HeavyAttack; 
        }

        private void GameInput_OnNormalAttackPerformed()
        {
            if (_currentState == PlayerCombatState.ComboAttack) return;

            if (!playerStateChecker.p_IsGrounded) return;

            if (playerStateChecker.p_isCrouching) return;

            if (playerTimeTracker.p_attackCoolDown > 0) return;

            _currentState = PlayerCombatState.NormalAttack;
        }

        private void Update()
        {
            if (_previousState == _currentState) return;

            switch (_currentState)
            {
                case PlayerCombatState.NotAttack:
                    OnNotAttack?.Invoke();

                    _previousState = _currentState;

                    if (_currentCoroutine != null)
                    {
                        StopCoroutine(_currentCoroutine);
                    }

                    break;
                case PlayerCombatState.NormalAttack:
                    OnNormalAttack?.Invoke();

                    HandleNormalAttack();

                    _previousState = _currentState;

                    if (_currentCoroutine != null)
                    {
                        StopCoroutine(_currentCoroutine);
                    }

                    _currentCoroutine = StartCoroutine(WaitForAnimationCompleted(() =>
                    {
                        _currentState = PlayerCombatState.NotAttack;
                    }));

                    break;
                case PlayerCombatState.HeavyAttack:
                    OnHeavyAttack?.Invoke();

                    HandleHeavyAttack();

                    _previousState = _currentState;

                    if (_currentCoroutine != null)
                    {
                        StopCoroutine(_currentCoroutine);
                    }

                    _currentCoroutine = StartCoroutine(WaitForAnimationCompleted(() =>
                    {
                        _currentState = PlayerCombatState.NotAttack;
                    }));

                    break;
                case PlayerCombatState.CrouchAttack:
                    OnCrouchAttack?.Invoke();

                    HandleCrouchAttack();

                    _previousState = _currentState;

                    if (_currentCoroutine != null)
                    {
                        StopCoroutine(_currentCoroutine);
                    }

                    _currentCoroutine = StartCoroutine(WaitForAnimationCompleted(() =>
                    {
                        _currentState = PlayerCombatState.NotAttack;
                    }));

                    break;
                case PlayerCombatState.ComboAttack:
                    OnComboAttack?.Invoke();

                    HandleHeavyAttack();

                    _previousState = _currentState;

                    if (_currentCoroutine != null)
                    {
                        StopCoroutine(_currentCoroutine);
                    }

                    _currentCoroutine = StartCoroutine(WaitForAnimationCompleted(() =>
                    {
                        _currentState = PlayerCombatState.NotAttack;
                    }));

                    break;
            }        
        }

        IEnumerator WaitForAnimationCompleted(Action OnComplete)
        {
            yield return new WaitUntil(() => _isLockedTillChanged);

            yield return new WaitUntil(() => Time.time + 0.1f >= _lockedTillChanged);

            OnComplete?.Invoke();
        }

        private void HandleNormalAttack()
        {
            HandleAttack(playerDataSO.NormalAttackSpot, playerDataSO.NormalAttackRadius, playerDataSO.NormalAttackDamage);
        }

        private void HandleHeavyAttack()
        {
            HandleAttack(playerDataSO.NormalAttackSpot, playerDataSO.NormalAttackRadius, playerDataSO.HeavyAttackDamage);
        }

        private void HandleCrouchAttack()
        {
            HandleAttack(playerDataSO.CrouchAttackSpot, playerDataSO.CrouchAttackRadius, playerDataSO.CrouchAttackDamage);
        }


        private void HandleAttack(Vector3 attackSpot, float attackRadius, float attackDamage)
        {
            CommonFunctions.ClearArray(_enemiesColliders, 0, _enemiesColliders.Length);

            int enemiesCollider = Physics.OverlapSphereNonAlloc(attackSpotSphere.position, attackRadius, _enemiesColliders, enemiesLayer);

            for (int i = 0; i < enemiesCollider; i++)
            {
                IDamageAble damageAble = _enemiesColliders[i].GetComponentInChildren<IDamageAble>();

                if (damageAble != null)
                {
                    damageAble.TakeDamage(attackDamage);
                }
            }
        }


        private void OnDrawGizmos()
        {
            if (!playerStateChecker.p_isCrouching)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackSpotSphere.position, playerDataSO.NormalAttackRadius);
            } else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackSpotSphere.position, playerDataSO.CrouchAttackRadius);
            }            
        }
    }
}
