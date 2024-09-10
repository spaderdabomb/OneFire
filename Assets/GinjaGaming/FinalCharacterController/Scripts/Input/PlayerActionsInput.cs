using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GinjaGaming.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerActionsInput : MonoBehaviour, PlayerControls.IPlayerActionsMapActions
    {
        #region Class Variables
        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerAnimation _playerAnimation;
        private PlayerState _playerState;
        public bool AttackHit { get; private set; } = false;

        public event Action OnPunchHit;
        public event Action onInteract;
        public void Interact() => onInteract?.Invoke();
        #endregion

        #region Startup
        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerAnimation = GetComponent<PlayerAnimation>();
            _playerState = GetComponent<PlayerState>();
        }
        private void OnEnable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerActionsMap.Enable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionsMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            if (PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot disable");
                return;
            }

            PlayerInputManager.Instance.PlayerControls.PlayerActionsMap.Disable();
            PlayerInputManager.Instance.PlayerControls.PlayerActionsMap.RemoveCallbacks(this);
        }
        #endregion

        #region Update
        private void Update()
        {
            if (_playerLocomotionInput.MovementInput != Vector2.zero ||
                _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping ||
                _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling)
            {
                // if in an inturruptible state, set to none
                if (_playerState.CurrentPlayerActionState == PlayerActionState.Gathering)
                {
                    _playerState.SetPlayerActionState(PlayerActionState.None);
                }
            }
        }

        public void SetInteractPressedFalse()
        {
            _playerState.SetPlayerActionState(PlayerActionState.None);
        }

        public void SetAttackPressedFalse()
        {
            _playerState.SetPlayerActionState(PlayerActionState.None);
            AttackHit = false;
        }

        public void PunchHit()
        {
            if (AttackHit)
                return;

            OnPunchHit?.Invoke();
            AttackHit = true;
        }

        #endregion

        #region Input Callbacks
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            _playerState.SetPlayerActionState(PlayerActionState.Gathering);
            Interact();
        }

        public void OnAttacking(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            _playerState.SetPlayerActionState(PlayerActionState.Attacking);
        }
        #endregion
    }
}
