using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using System.Collections;

namespace Netcode
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class NetworkPlayer : NetworkBehaviour
    {
        [Header("Player Movement Settings")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.15f;

        [Header("Audio")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Header("Grounded Check")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public Vector2 MapSize = new Vector2(50f, 50f); // Define map size for spawning
        public LayerMask GroundLayers;

        private CharacterController _controller;
        private StarterAssetsInputs _input;
#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private GameObject _mainCamera;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private float _verticalVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private int _animIDDodge;

        private float _speed;
        private float _rotationVelocity;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private bool _spawnedGrounded;

        private float _timeDogde = 1.0f;

        // Networked variables to sync animation
        public NetworkVariable<float> NetworkedSpeed = new NetworkVariable<float>();
        public NetworkVariable<bool> NetworkedGrounded = new NetworkVariable<bool>();
        public NetworkVariable<bool> NetworkedJump = new NetworkVariable<bool>();
        public NetworkVariable<bool> NetworkedFreeFall = new NetworkVariable<bool>();
        public NetworkVariable<bool> NetworkedDodge = new NetworkVariable<bool>();

        private bool _hasAnimator;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Spawn the player at a random position on the server
                Vector3 spawnPosition = GetRandomPositionOnMap();
                transform.position = spawnPosition;
            }

            if (IsOwner)
            {
                // Only enable input for the owner, after a short delay to ensure connection
                StartCoroutine(EnableInputAfterConnection());
            }
            else
            {
                // Disable input for non-owners
                if (_playerInput && _input)
                {
                    _playerInput.enabled = false;
                    _input.enabled = false;
                }
            }
        }

        // Coroutine to delay input activation for owner client
        private IEnumerator EnableInputAfterConnection()
        {
            yield return new WaitForSeconds(0.5f); // Delay to ensure client connection
            _playerInput.enabled = true;
            _input.enabled = true;
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            _animator = GetComponent<Animator>();
            _hasAnimator = _animator != null;

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            AssignAnimationIDs();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDDodge = Animator.StringToHash("Dodge");
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateAnimationParametersServerRpc(float speed, bool grounded, bool jump, bool freeFall)
        {
            // Server updates the networked values
            NetworkedSpeed.Value = speed;
            NetworkedGrounded.Value = grounded;
            NetworkedJump.Value = jump;
            NetworkedFreeFall.Value = freeFall;
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateDodgeServerRpc(bool dodge)
        {
            // Server updates the networked values
            NetworkedDodge.Value = dodge;
        }

        private Vector3 GetRandomPositionOnMap()
        {
            // Generate a random position within the defined map bounds
            float randomX = Random.Range(-MapSize.x / 2, MapSize.x / 2);
            float randomZ = Random.Range(-MapSize.y / 2, MapSize.y / 2);
            Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);

            // Ensure the random position is grounded
            if (Physics.Raycast(new Vector3(randomX, 10f, randomZ), Vector3.down, out RaycastHit hit, Mathf.Infinity, GroundLayers))
            {
                Debug.Log($"THe player is spawning on the {hit.collider.gameObject.name}");
                randomPosition.y = hit.point.y;
            }

            return randomPosition;
        }

        private void Update()
        {
            if (IsOwner && NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient)
            {
                // Only process input and update movement for the owner client
                GroundedCheck();
                HandleGravityAndJumping();
                Move();

                // Update animation parameters based on local input
                UpdateAnimationParametersServerRpc(_animationBlend, Grounded, _animator.GetBool(_animIDJump), _animator.GetBool(_animIDFreeFall));
            }

            UpdateAnimator();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        Vector2 MoveInput()
        {
            // horizontal
            float moveX = Input.GetAxis("Horizontal");
            // vertical
            float moveY = Input.GetAxis("Vertical");

            return new Vector2(moveX, moveY);
        }

        bool JumpInput()
        {
            if (!Grounded)
            {
                return false;
            }
            return Input.GetButtonDown("Jump");
        }

        bool SprintInput()
        {
            return Input.GetButton("Run");
        }

        private void Move()
        {
            if (!IsOwner) return;

            float targetSpeed = SprintInput() ? SprintSpeed : MoveSpeed;

            if (MoveInput() == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? MoveInput().magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(MoveInput().x, 0.0f, MoveInput().y).normalized;

            if (MoveInput() != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            // left click to dodge
            if (Input.GetMouseButtonDown(1) && !stateInfo.IsName("Dodge")) {
                UpdateDodgeServerRpc(true);
                _timeDogde = 0.0f;
            }


            // Comparer le nom de l'état
            if (_timeDogde < 0.6f) {
                if (_timeDogde > 0.05f) {
                    UpdateDodgeServerRpc(false);
                    _speed = 10f;
                }
                _timeDogde += Time.deltaTime;
            }

            // Apply movement locally
            if (_spawnedGrounded)
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else
            {
                _spawnedGrounded = true;
            }

            // Send updated position to the server periodically
            if (Time.frameCount % 1 == 0) // Send every 1 frames
            {
                SendMovementToServerRpc(transform.position, transform.rotation);
            }
        }

        // ServerRpc to sync client position
        [ServerRpc]
        private void SendMovementToServerRpc(Vector3 clientPosition, Quaternion clientRotation)
        {
            // Threshold to determine if correction is needed
            float correctionThreshold = 0.5f;

            if (Vector3.Distance(transform.position, clientPosition) > correctionThreshold)
            {
//                Debug.Log($"Server correcting position from {clientPosition} to {transform.position}");
                
                // Update the client to match the server's position
                UpdatePositionClientRpc(transform.position, clientRotation);
            }
            else
            {
                // Accept client position as valid
                transform.position = clientPosition;
                transform.rotation = clientRotation;
            }
            // Now, update non-owner clients with the position of the player
            BroadcastMovementClientRpc(clientPosition, clientRotation); // Send to other clients
        }

        [ClientRpc]
        private void BroadcastMovementClientRpc(Vector3 serverPosition, Quaternion serverRotation)
        {
            // Only non-owner clients should update their position and rotation based on the server's position
            if (!IsOwner)
            {
                transform.position = serverPosition;
                transform.rotation = serverRotation;
            }
        }

        [ClientRpc]
        private void UpdatePositionClientRpc(Vector3 serverPosition, Quaternion serverRotation)
        {
            StartCoroutine(SmoothCorrection(serverPosition, serverRotation));
        }

        private IEnumerator SmoothCorrection(Vector3 targetPosition, Quaternion targetRotation)
        {
            float duration = 0.1f; // Duration of the smoothing time
            float elapsedTime = 0f;
            Vector3 initialPosition = transform.position;
            Quaternion initialRotation = transform.rotation;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
                transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure that the final position and rotation is exactly set
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        }

        private void HandleGravityAndJumping()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (JumpInput() && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    if (_hasAnimator) _animator.SetBool(_animIDJump, true);
                }

                if (_jumpTimeoutDelta >= 0.0f) _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            if (_verticalVelocity < 53.0f)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void UpdateAnimator()
        {
            if (_hasAnimator)
            {
                if (IsOwner)
                {
                    _animator.SetFloat(_animIDSpeed, _animationBlend);
                    _animator.SetFloat(_animIDMotionSpeed, _input.analogMovement ? MoveInput().magnitude : 1f);
                    _animator.SetBool(_animIDDodge, NetworkedDodge.Value);
                }
                else
                {
                    _animator.SetFloat(_animIDSpeed, NetworkedSpeed.Value);
                    _animator.SetFloat(_animIDMotionSpeed, 1f);
                    _animator.SetBool(_animIDGrounded, NetworkedGrounded.Value);
                    _animator.SetBool(_animIDJump, NetworkedJump.Value);
                    _animator.SetBool(_animIDFreeFall, NetworkedFreeFall.Value);
                    _animator.SetBool(_animIDDodge, NetworkedDodge.Value);
                }
            }
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}
