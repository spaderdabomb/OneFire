using Cinemachine;
using JSAM;
using OneFireUi;
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
        private PlayerEquippedItem _playerEquippedItem;
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
            _playerEquippedItem = GetComponent<PlayerEquippedItem>();
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
            if (context.started)
                return;

            if (context.canceled)
            {
                OnAttackCanceled();
                return;
            }

            if (context.performed)
            {
                OnAttackPressed();
                return;
            }
        }

        public void OnAttackCanceled()
        {
            // Fishing mouse click released
            if (_playerEquippedItem.ActiveItemData != null &&
                _playerEquippedItem.ActiveItemData.itemType.HasFlag(ItemType.FishingRod) &&
                _playerState.CurrentPlayerFishingState == PlayerFishingState.RodCharging)
            {
                _playerState.SetPlayerFishingState(PlayerFishingState.RodReleased);
                FishingManager.Instance.CastRod(1f);
                return;
            }
        }

        public void OnAttackPressed()
        {
            if (_playerEquippedItem.ActiveItemData != null &&
                _playerEquippedItem.ActiveItemData.itemType.HasFlag(ItemType.FishingRod))
            {
                OnFishingConfirmedPressed();
            }
            else
            {
                _playerState.SetPlayerActionState(PlayerActionState.Attacking);
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (_playerState.CurrentPlayerActionState == PlayerActionState.Fishing)
            {
                FishingManager.Instance.StopFishing();
            }
        }

        public void OnFishingConfirmedPressed()
        {
            if (_playerState.CurrentPlayerActionState != PlayerActionState.Fishing)
            {
                _playerState.SetPlayerActionState(PlayerActionState.Fishing);
                return;
            }

            if (_playerState.CurrentPlayerFishingState == PlayerFishingState.FishSpawned)
            {
                FishingManager.Instance.HookFish();
            }
            else if (_playerState.CurrentPlayerFishingState == PlayerFishingState.FishHooked)
            {
                FishingManager.Instance.ReelFishPressed();
            }
        }


        #endregion
    }
}
