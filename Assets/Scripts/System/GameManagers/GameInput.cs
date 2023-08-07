using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.GameManagers
{
    public class GameInput : MonoBehaviour
    {
        // Singleton
        public static GameInput Instance { get; private set; }

        #region "Events"

        public event Action OnJumpingPerformed;
        public event Action OnJumpingCanceled;
        public event Action OnCrouchPerformed;
        public event Action OnCrouchCanceled;
        public event Action OnNormalAttackPerformed;
        public event Action OnHeavyAttackPerformed;
        public event Action OnCrouchAttackPerformed;
        public event Action OnComboAttackPerformed;
        public event Action OnPanCameraPerformed;
        public event Action OnPanCameraCanceled;

        #endregion

        private UserInput _userInput;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            _userInput = new UserInput();
            _userInput.Enable();
        }

        private void Start()
        {
            _userInput.Player.Jumping.performed += Jumping_performed;
            _userInput.Player.Jumping.canceled += Jumping_canceled;
            _userInput.Player.Crouch.performed += Crouch_performed;
            _userInput.Player.Crouch.canceled += Crouch_canceled;
            _userInput.Player.NormalAttack.performed += NormalAttack_performed;
            _userInput.Player.HeavyAttack.performed += HeavyAttack_performed;
            _userInput.Player.CrouchAttack.performed += CrouchAttack_performed;
            _userInput.Player.ComboAttack.performed += ComboAttack_performed;
            _userInput.Player.PanCamera.performed += PanCamera_performed;
            _userInput.Player.PanCamera.canceled += PanCamera_canceled;
        }
        
        private void PanCamera_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnPanCameraCanceled?.Invoke();
        }

        private void PanCamera_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnPanCameraPerformed?.Invoke();
        }

        private void ComboAttack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnComboAttackPerformed?.Invoke();
        }

        private void CrouchAttack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnCrouchAttackPerformed?.Invoke();
        }

        private void HeavyAttack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnHeavyAttackPerformed?.Invoke();
        }

        private void NormalAttack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnNormalAttackPerformed?.Invoke();
        }

        private void Crouch_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnCrouchCanceled?.Invoke();
        }

        private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnCrouchPerformed?.Invoke();
        }

        private void Jumping_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnJumpingCanceled?.Invoke();
        }

        private void Jumping_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnJumpingPerformed?.Invoke();
        }

        private void OnDestroy()
        {
            _userInput.Dispose();

            _userInput.Player.Jumping.performed -= Jumping_performed;
            _userInput.Player.Jumping.canceled -= Jumping_canceled;
            _userInput.Player.Crouch.performed -= Crouch_performed;
            _userInput.Player.Crouch.canceled -= Crouch_canceled;
            _userInput.Player.NormalAttack.performed -= NormalAttack_performed;
            _userInput.Player.HeavyAttack.performed -= HeavyAttack_performed;
            _userInput.Player.CrouchAttack.performed -= CrouchAttack_performed;
            _userInput.Player.ComboAttack.performed -= ComboAttack_performed;
            _userInput.Player.PanCamera.performed -= PanCamera_performed;
            _userInput.Player.PanCamera.canceled -= PanCamera_canceled;
        }

        /// <summary>
        /// Get normalized player movement input
        /// </summary>
        /// <returns> Vector2 movement input </returns>
        public Vector2 GetMovementInput2DNormalized()
        {
            Vector2 movementInput = _userInput.Player.Movement.ReadValue<Vector2>();
            movementInput = movementInput.normalized;
            return movementInput;
        }

        public Vector2 GetPanCameraDirectionNormalized()
        {
            Vector2 upDownInput = _userInput.Player.PanCamera.ReadValue<Vector2>();

            return upDownInput;
        }

        public bool IsDrawProjectile()
        {
            return _userInput.Player.DrawProjectile.ReadValue<float>() > 0.1f;
        }
    }
}
