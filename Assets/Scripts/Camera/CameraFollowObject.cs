using Askeladd.Scripts.Characters.Player.PlayerLogics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askeladd.Scripts.Camera
{
    public class CameraFollowObject : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private PlayerStateChecker playerStateChecker;

        [Header("Flip y rotation stat")]
        [SerializeField] private float _flipYRotationTime = 0.5f;

        // Coroutine
        private Coroutine _turnCoroutine;

        private void Update()
        {
            // Make the cameraFollowObject follow the player's position
            transform.position = _playerTransform.position;
        }

        public void CallTurn()
        {
            if (_turnCoroutine != null)
            {
                StopCoroutine(_turnCoroutine);
            }

            _turnCoroutine = StartCoroutine(FlipYLerp());
        }
         
        private IEnumerator FlipYLerp()
        {
            float startRotation = transform.eulerAngles.y;
            float endRotation = DetermineEndRotation();
            float yRotation = 0f;

            float elapsedTime = 0f;

            while (elapsedTime < _flipYRotationTime)
            {
                elapsedTime += Time.deltaTime;

                // lerp Y rotation
                yRotation = Mathf.Lerp(startRotation, endRotation, (elapsedTime / _flipYRotationTime));
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

                yield return null;
            }
        }

        public float DetermineEndRotation()
        {
            if (playerStateChecker.p_isFacingRight)
            {
                return 0f;
            } else
            {
                return 180f;
            }
        }
    }
}
