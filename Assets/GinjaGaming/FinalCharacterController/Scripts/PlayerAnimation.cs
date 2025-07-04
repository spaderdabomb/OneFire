using JSAM;
using OneFireUi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace GinjaGaming.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimatorOverrideController overrideController;
        [SerializeField] private float locomotionBlendSpeed = 4f;
        [SerializeField] private float maskLayerBlendSpeed = 4f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private PlayerController _playerController;
        private PlayerActionsInput _playerActionsInput;
        private PlayerEquippedItem _playerEquippedItem;

        // Locomotion
        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
        private static int isIdlingHash = Animator.StringToHash("isIdling");
        private static int isGroundedHash = Animator.StringToHash("isGrounded");
        private static int isFallingHash = Animator.StringToHash("isFalling");
        private static int isJumpingHash = Animator.StringToHash("isJumping");

        // Fishing
        private static int canStartFishingTriggerHash = Animator.StringToHash("canStartFishingTrigger");
        private static int canReelFishingTriggerHash = Animator.StringToHash("canReelFishingTrigger");
        private static int isFishCaughtHash = Animator.StringToHash("isFishCaught");

        // Actions
        private static int isAttackingHash = Animator.StringToHash("isAttacking");
        private static int isGatheringHash = Animator.StringToHash("isInteracting");
        private static int isFishingHash = Animator.StringToHash("isFishing");
        private static int isRodChargingHash = Animator.StringToHash("isRodCharging");
        private static int isRodReleasedHash = Animator.StringToHash("isRodReleased");
        private static int isBobInWaterHash = Animator.StringToHash("isBobInWater");
        private static int isPlayingActionHash = Animator.StringToHash("isPlayingAction");
        private int[] interruptibleActionHashes;

        // Camera/Rotation
        private static int isRotatingToTargetHash = Animator.StringToHash("isRotatingToTarget");
        private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");

        private Vector3 _currentBlendInput = Vector3.zero;

        private float _sprintMaxBlendValue = 1.5f;
        private float _runMaxBlendValue = 1.0f;
        private float _walkMaxBlendValue = 0.5f;

        private float _targetUpperBodyLayerWeight = 0f;
        private float _currentUpperBodyLayerWeight = 0f;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _playerController = GetComponent<PlayerController>();
            _playerActionsInput = GetComponent<PlayerActionsInput>();
            _playerEquippedItem = GetComponent<PlayerEquippedItem>();

            interruptibleActionHashes = new int[] { isGatheringHash };
        }

        private void Start()
        {
            RegisterCallbacks();
        }

        private void OnDestroy()
        {
            UnregisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            InventoryManager.Instance.OnHotbarItemSelectedChanged += UpdateAttackAnimation;
        }

        private void UnregisterCallbacks()
        {
            InventoryManager.Instance.OnHotbarItemSelectedChanged -= UpdateAttackAnimation;
        }


        private void Update()
        {
            UpdateLayerWeights();
            UpdateAnimationState();
        }

        private void UpdateAttackAnimation(InventorySlot inventorySlot)
        {
            AnimationClip newClip = _playerEquippedItem.GetAttackAnimationFromActiveItem();
            AnimatorOverrideController tempOverrideController = new AnimatorOverrideController(overrideController);

            overrideController["PunchRight_EVENT"] = tempOverrideController[newClip.name]; 
            _animator.runtimeAnimatorController = overrideController;
        }

        private void UpdateLayerWeights()
        {
            if (_playerState.CurrentPlayerMovementState == PlayerMovementState.Idling)
                _targetUpperBodyLayerWeight = 0f;
            else
                _targetUpperBodyLayerWeight = 1f;

            _currentUpperBodyLayerWeight = Mathf.Lerp(_currentUpperBodyLayerWeight, _targetUpperBodyLayerWeight, maskLayerBlendSpeed);
            _animator.SetLayerWeight(1, _currentUpperBodyLayerWeight);
            _animator.SetLayerWeight(2, 1f-_currentUpperBodyLayerWeight);
        }

        private void UpdateAnimationState()
        {
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isJumping = _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
            bool isFalling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
            bool isGrounded = _playerState.InGroundedState();

            bool isPlayingAction = interruptibleActionHashes.Any(hash => _animator.GetBool(hash));
            bool isAttacking = _playerState.CurrentPlayerActionState == PlayerActionState.Attacking;
            bool isGathering = _playerState.CurrentPlayerActionState == PlayerActionState.Gathering;
            bool isRodCharging = _playerState.CurrentPlayerFishingState == PlayerFishingState.RodCharging;
            bool isRodReleased = _playerState.CurrentPlayerFishingState == PlayerFishingState.RodReleased;
            bool isBobInWater = _playerState.CurrentPlayerFishingState == PlayerFishingState.BobInWater;
            bool isFishCaught = _playerState.CurrentPlayerFishingState == PlayerFishingState.FishCaught;
            bool isFishing = _playerState.CurrentPlayerActionState == PlayerActionState.Fishing;
            bool isRunBlendValue = isRunning || isJumping || isFalling;

            Vector2 inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * _sprintMaxBlendValue :
                                  isRunBlendValue ? _playerLocomotionInput.MovementInput * _runMaxBlendValue : 
                                                    _playerLocomotionInput.MovementInput * _walkMaxBlendValue;

            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

            _animator.SetBool(isGroundedHash, isGrounded);
            _animator.SetBool(isIdlingHash, isIdling);
            _animator.SetBool(isFallingHash, isFalling);
            _animator.SetBool(isJumpingHash, isJumping);
            _animator.SetBool(isRotatingToTargetHash, _playerController.IsRotatingToTarget);
            _animator.SetBool(isAttackingHash, isAttacking);
            _animator.SetBool(isGatheringHash, isGathering);
            _animator.SetBool(isFishingHash, isFishing);
            _animator.SetBool(isRodChargingHash, isRodCharging);
            _animator.SetBool(isRodReleasedHash, isRodReleased);
            _animator.SetBool(isBobInWaterHash, isBobInWater);
            _animator.SetBool(isFishCaughtHash, isFishCaught);
            _animator.SetBool(isPlayingActionHash, isPlayingAction);

            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
            _animator.SetFloat(rotationMismatchHash, _playerController.RotationMismatch);
        }

        public void SetFishingTrigger()
        {
            _animator.SetTrigger(canStartFishingTriggerHash);
        }

        public void SetReelingTrigger()
        {
            _animator.SetTrigger(canReelFishingTriggerHash);
        }

        public void PullFishOutOfWater()
        {
            SoundFileObject sfo = AudioManager.GetSoundSafe(MainLibrarySounds.CastRod);
            sfo.relativeVolume = 0.5f;
            AudioManager.PlaySound(MainLibrarySounds.CastRod);
            sfo.relativeVolume = 1f;

            AudioManager.PlaySound(MainLibrarySounds.Splash_CatchFish);
        }
    }
}
