using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GinjaGaming.FinalCharacterController
{
    public class PlayerState : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;
        [field: SerializeField] public PlayerActionState CurrentPlayerActionState { get; private set; } = PlayerActionState.None;
        [field: SerializeField] public PlayerFishingState CurrentPlayerFishingState { get; private set; } = PlayerFishingState.None;

        public float TimeInNoneActionState { get; private set; } = 0f;
        public bool EnteredAttackStateLastFrame { get; private set; } = false;

        private PlayerAnimation _playerAnimation;

        private void Awake()
        {
            _playerAnimation = GetComponent<PlayerAnimation>();
        }

        private void Update()
        {
            EnteredAttackStateLastFrame = CurrentPlayerActionState == PlayerActionState.Attacking && EnteredAttackStateLastFrame == false;

            if (CurrentPlayerActionState == PlayerActionState.None)
                TimeInNoneActionState += Time.deltaTime;
            else
                TimeInNoneActionState = 0f;
        }

        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            CurrentPlayerMovementState = playerMovementState;
        }

        public void SetPlayerActionState(PlayerActionState playerActionState)
        {
            CurrentPlayerActionState = playerActionState;

            if (CurrentPlayerActionState == PlayerActionState.Fishing && CurrentPlayerFishingState == PlayerFishingState.None)
            {
                SetPlayerFishingState(PlayerFishingState.RodCharging);
                _playerAnimation.SetFishingTrigger();
            }
        }

        public void SetPlayerFishingState(PlayerFishingState playerFishingState)
        {
            CurrentPlayerFishingState = playerFishingState;
        }

        public bool InGroundedState()
        {
            return IsStateGroundedState(CurrentPlayerMovementState);
        }

        public bool IsStateGroundedState(PlayerMovementState movementState)
        {
            return movementState == PlayerMovementState.Idling ||
                   movementState == PlayerMovementState.Walking ||
                   movementState == PlayerMovementState.Running ||
                   movementState == PlayerMovementState.Sprinting;
        }
    }
    public enum PlayerMovementState
    {
        Idling = 0,
        Walking = 1,
        Running = 2,
        Sprinting = 3,
        Jumping = 4,
        Falling = 5,
        Strafing = 6,
    }

    public enum PlayerActionState
    {
        None = 0,
        Attacking = 1,
        Gathering = 2,
        Crafting = 3,
        Fishing = 4,
    }

    public enum PlayerFishingState
    {
        None = 0,
        RodCharging = 1,
        RodReleased = 2,
        BobInWater = 3,
        FishSpawned = 4,
        FishHooked = 5,
        FishCaught = 6,
    }
}
