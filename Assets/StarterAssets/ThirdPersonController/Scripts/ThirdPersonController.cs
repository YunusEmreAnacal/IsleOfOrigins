 using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 3.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 8f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDx;
        private int _animIDy;
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

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



    //MY CODES ***************************************************************************************************
    public GameObject crouchButton;
    public GameObject standButton;
    private Animator anim;
    private CharacterController controller;

    private float standHeight;
    [SerializeField]  private float crouchHeight;
    [SerializeField] private GameObject HeadPosition;
    [SerializeField] private bool isCrouch = false;
    [SerializeField] private bool Canstand ;

    float x;
    float y;

        public float attackRange = 10f;
        public float attackDamage = 25f;
        public LayerMask sheepLayer;


        public void ToggleCrouch()
    {
        if (isCrouch && Canstand)
        {
            StandUp();
        }
        else if (!isCrouch && Canstand)
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        isCrouch = true;
        anim.SetBool("Crouch", true);
        controller.height = crouchHeight;
        controller.center = new Vector3(0f, controller.height/2, 0f);
        SprintSpeed = 5f;
        JumpHeight = 0f;
        crouchButton.SetActive(false);
        standButton.SetActive(true);
        FootstepAudioVolume = 0.25f;
    }

    private void StandUp()
    {
        isCrouch = false;
        anim.SetBool("Crouch", false);
        controller.height = standHeight;
        controller.center = new Vector3(0f, standHeight / 2, 0f);
        SprintSpeed = 8f;
        JumpHeight = 1.2f;
        standButton.SetActive(false);
        crouchButton.SetActive(true);
        FootstepAudioVolume = 0.5f;
    }


    public void Attack()
        {
            
            if (!isCrouch)
            {
                int attackMode = Random.Range(1, 3);
                anim.SetTrigger("attack" + attackMode);
            }

            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            Vector3 rayDirection = transform.forward + Vector3.down * 0.3f;

            Debug.DrawRay(rayOrigin, rayDirection * attackRange, Color.red, 2.0f);

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, attackRange))
            {
                Debug.Log("Hit something: " + hit.collider.gameObject.name);
                SheepHealth sheepHealth = hit.collider.GetComponent<SheepHealth>();
                Zombie_Data zombieHealth  = hit.collider.GetComponent<Zombie_Data>();
                if (sheepHealth != null)
                {
                    Debug.Log("Hit a sheep!");
                    sheepHealth.TakeDamage(attackDamage, transform.position);
                }
                else if (zombieHealth != null)
                {
                    Debug.Log("Hit a Zombie");
                    zombieHealth.TakeDamage(attackDamage, transform.position);
                }
            }
            else
            {
                Debug.Log("Raycast didn't hit anything.");
            }
        }

    
    //ENDS**********************************************************************************
        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            controller = GetComponent<CharacterController>();
            anim = GetComponent<Animator>();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            
            standHeight = controller.height;
            controller.center = new Vector3(0f, standHeight / 2 , 0f);
            SprintSpeed = 10f;
            JumpHeight = 1.2f;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            //Crouch();

            if (Physics.Raycast(HeadPosition.transform.position, Vector3.up, 0.5f))
        {
            Canstand = false;
            Debug.DrawRay(HeadPosition.transform.position, Vector3.up, Color.green);
        }
        else
        {
            Canstand = true;
        }  

            

        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDy = Animator.StringToHash("y");
            _animIDx = Animator.StringToHash("x");
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        protected virtual void Move()
        {
            // Hedef hızı belirleme
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // Eğer hareket yoksa hedef hızı 0 yap
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // Mevcut hız
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            // Hızlanma ve yavaşlama
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                    Time.deltaTime * SpeedChangeRate);
            }
            else
            {
                _speed = targetSpeed;
            }

            // Normalize input direction
            Vector3 inputDirection = Vector3.ClampMagnitude(new Vector3(_input.move.x, 0, _input.move.y), 1);

            // Always face the camera direction
            _targetRotation = _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            Vector3 targetDirection = Vector3.ClampMagnitude(Quaternion.Euler(0.0f, _targetRotation, 0.0f) * inputDirection, 1); 
                //Quaternion.Euler(0.0f, _targetRotation, 0.0f) * inputDirection;

            // Move the player
            _controller.Move(targetDirection * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // Animator güncellemesi
            if (_hasAnimator)
            {
                // Current x and y values in the animator
                float currentX = _animator.GetFloat("x");
                float currentY = _animator.GetFloat("y");

                // Target x and y values based on input and runMultiplier
                float runMultiplier = _input.sprint ? 1.0f : 0.1f;
                float targetX = Mathf.Min(_input.move.x,1) * runMultiplier;
                float targetY = Mathf.Min(_input.move.y, 1) * runMultiplier;

                // Smoothly transition to the target values
                float x = Mathf.Lerp(currentX, targetX, Time.deltaTime * SpeedChangeRate);
                float y = Mathf.Lerp(currentY, targetY, Time.deltaTime * SpeedChangeRate);

                

                _animator.SetFloat("x", x);
                _animator.SetFloat("y", y);

                
            }
        }

        public void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
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