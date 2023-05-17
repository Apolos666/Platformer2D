using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.GameManagers
{
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; private set; }

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
            // Events          
        }

        private void OnDestroy()
        {
            _userInput.Dispose();
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
