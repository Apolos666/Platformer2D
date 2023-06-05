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
        }

        /// <summary>
        /// Get normalized player movement input
        /// </summary>
        /// <returns> Vector2 movement input </returns>
        public Vector2 GetMovementInput2DNormalized()
        {
            Vector2 f_movementInput = _userInput.Player.Movement.ReadValue<Vector2>();
            f_movementInput = f_movementInput.normalized;
            return f_movementInput;
        }
    }
}
