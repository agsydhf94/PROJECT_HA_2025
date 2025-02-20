using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;

namespace HA
{
    public class PlayerControllerRPG : MonoBehaviour
    {
        public static PlayerControllerRPG instance;

        public static PlayerControllerRPG Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<PlayerControllerRPG>();
                }
                return instance;
            }
        }

        [Header("Camera Clamping")]
        public float topClamp = 70.0f;
        public float bottomClamp = -30.0f;
        public GameObject cinemachineCameraTarget;
        public float cameraAngleOverride = 0.0f;

        private Vector2 look;
        private const float _threshold = 0.01f;
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;


        [Header("Camera Setting")]
        public float cameraHorizontalSpeed = 2.0f;
        public float cameraVerticalSpeed = 2.0f;


        [Header("Weapon FOV")]
        public float defaultFOV;
        public float aimFOV;


        [Header("Interaction UI")]
        public InteractionSensor interactionSensor;

        [Header("RPG Mode Detect")]
        public bool isRPG = true;



        private Camera mainCamera;
        private CharacterController controller;
        private Animator animator;
        private bool isSprint = false;
        private Vector2 move;
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        [Range(0.0f, 0.3f)] public float rotationSmoothTime = 0.12f;
        private float rotationVelocity;
        private float verticalVelocity;





        public float moveSpeed = 3.0f;
        public float sprintSpeed = 5.0f;
        public float speedChangeRate = 10.0f;
        public float dashSpeedDeltaMultiplier = 2f;
        

        private bool isStrafe = false;

        [Header("Weapon Holder")]
        public GameObject weaponHolder;
        private Weapon currentWeapon;
        public GameObject katana;

        private int attackComboCount = 0;
        public int AttackComboCount
        {
            set => attackComboCount = value;
        }


        private bool isEnablemovement = true;
        public bool IsEnableMovemnt
        {
            set => isEnablemovement = value;
        }


        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            var weaponGameObject = TransformUtility.FindGameObjectWithTag(weaponHolder, "Weapon");
            currentWeapon = weaponGameObject.GetComponent<Weapon>();

            katana = GameObject.Find("Katana");

            // 추가 6/17
            interactionSensor = GameObject.Find("Interaction Sensor").GetComponent<InteractionSensor>();

        }

        // 추가 6/17
        private void OnEnable()
        {
            interactionSensor.OnDetected += OnDetectedInteraction;
            interactionSensor.OnLost += OnLostInteraction;
        }

        private void OnLostInteraction(IInteractable interactable)
        {
            InteractionUI.Instance.RemoveInteractionData(interactable);
        }

        private void OnDetectedInteraction(IInteractable interactable)
        {
            InteractionUI.Instance.AddInteractionData(interactable);
        }

        // 추가 6/17

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            Move();

            // 6/17 추가 Interaction UI 버튼
            if (Input.GetKeyDown(KeyCode.F))
            {
                // 만약에 현재 UI에서 선택한 상호작용 콘텐츠가 ItemBox라면 => Player Character 의 줍기 모션을 트리거한다.
                // var contents = InteractionUI.Instance.GetInteractionContents();
                // if(contents == ItemBox)
                //   Player Character 의 줍기 모션을 트리거.

                InteractionUI.Instance.DoInteract();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                InteractionUI.Instance.SelectNext();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                InteractionUI.Instance.SelectPrev();
            }
            // 6/17 추가

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            move = new Vector2(horizontal, vertical);

            /*if(interactionSensor.HasInteractable)
            {
                move = Vector2.zero;
            }*/

            float hMouse = Input.GetAxis("Mouse X");
            float vMouse = Input.GetAxis("Mouse Y") * (-1);
            look = new Vector2(hMouse, vMouse);

            isSprint = Input.GetKey(KeyCode.LeftShift);
            isStrafe = Input.GetKey(KeyCode.Mouse1); // Mouse Right Button
            if (isStrafe)
            {
                Vector3 cameraForward = Camera.main.transform.forward.normalized;
                cameraForward.y = 0;
                transform.forward = cameraForward;
            }



            // 애니메이션 컨트롤
            animator.SetFloat("Speed", animationBlend);
            animator.SetFloat("Horizontal", move.x);
            animator.SetFloat("Vertical", move.y);
            animator.SetFloat("Strafe", isStrafe ? 1 : 0);

            // 왼쪽 시프트키 눌렀을 때, 애니메이션 속도 증가
            // GetKey : 누르는 동안 true
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetFloat("DashSpeedMultiplier", dashSpeedDeltaMultiplier);
            }
            else
            {
                animator.SetFloat("DashSpeedMultiplier", 1f);
            }

            // 콤보 액션
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                if(attackComboCount == 0)
                {
                    animator.SetTrigger("Trigger_Attack");
                    attackComboCount++;
                }
                else
                {
                    attackComboCount++;
                    animator.SetInteger("ComboCount", attackComboCount);
                }
            }



        }

        

        private void Move()
        {
            if (!isEnablemovement)
                return;

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = isSprint ? sprintSpeed : moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = move.magnitude;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * speedChangeRate);

                // round speed to 3 decimal places
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (animationBlend < 0.01f) animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (move != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    rotationSmoothTime);

                if (!isStrafe)
                {
                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            // move the player
            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                             new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;

                cinemachineTargetYaw += look.x * deltaTimeMultiplier * cameraHorizontalSpeed;
                cinemachineTargetPitch += look.y * deltaTimeMultiplier * cameraVerticalSpeed;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}

