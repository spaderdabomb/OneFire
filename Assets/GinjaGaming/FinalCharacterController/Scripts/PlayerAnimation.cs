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
        private AnimatorOverrideController overrideController;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        [SerializeField] private AnimationClip[] animationClips;

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

        // Actions
        private static int isAttackingHash = Animator.StringToHash("isAttacking");
        private static int isGatheringHash = Animator.StringToHash("isInteracting");
        private static int isPlayingActionHash = Animator.StringToHash("isPlayingAction");
        private int[] actionHashes;

        // Camera/Rotation
        private static int isRotatingToTargetHash = Animator.StringToHash("isRotatingToTarget");
        private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");

        private Vector3 _currentBlendInput = Vector3.zero;

        private float _sprintMaxBlendValue = 1.5f;
        private float _runMaxBlendValue = 1.0f;
        private float _walkMaxBlendValue = 0.5f;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _playerController = GetComponent<PlayerController>();
            _playerActionsInput = GetComponent<PlayerActionsInput>();
            _playerEquippedItem = GetComponent<PlayerEquippedItem>();

            actionHashes = new int[] { isGatheringHash };

            overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        }

        private void Update()
        {
            UpdateAttackAnimation();
            UpdateLayerWeights();
            UpdateAnimationState();
        }

        private void UpdateAttackAnimation()
        {
            AnimationClip newClip = _playerEquippedItem.GetAttackAnimationFromActiveItem();
            AnimatorOverrideController tempOverrideController = new AnimatorOverrideController(overrideController);
            tempOverrideController["PunchRight_EVENT"] = tempOverrideController[newClip.name];
            _animator.runtimeAnimatorController = tempOverrideController;
        }

        private void UpdateLayerWeights()
        {
            if (_playerState.CurrentPlayerMovementState == PlayerMovementState.Idling)
            {
                _animator.SetLayerWeight(1, 0);
                _animator.SetLayerWeight(2, 1);
            }
            else
            {
                _animator.SetLayerWeight(1, 1);
                _animator.SetLayerWeight(2, 0);
            }
        }

        private void UpdateAnimationState()
        {
            bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
            bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool isJumping = _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
            bool isFalling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
            bool isGrounded = _playerState.InGroundedState();
            bool isPlayingAction = actionHashes.Any(hash => _animator.GetBool(hash));

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
            _animator.SetBool(isAttackingHash, _playerActionsInput.AttackPressed);
            _animator.SetBool(isGatheringHash, _playerActionsInput.InteractPressed);
            _animator.SetBool(isPlayingActionHash, isPlayingAction);

            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
            _animator.SetFloat(rotationMismatchHash, _playerController.RotationMismatch);
        }
    }
}
