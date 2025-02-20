using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;
using Random = UnityEngine.Random;

namespace HA
{
    public class PlayerController_Multiplayer : MonoBehaviour
    {
        public static PlayerController_Multiplayer Instance { get; private set; } = null;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
        }

        //public CharacterBase PlayerCharacterBase { get { return characterBase; } }
        public CharacterBase PlayerCharacterBase => characterBase;


        [Header("Camera Clamping")]
        public float topClamp = 70.0f;
        public float bottomClamp = -30.0f;
        public GameObject cinemachineCameraTarget;
        public float cameraAngleOverride = 0.0f;

        private Vector2 look;
        private const float _threshold = 0.01f;
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;


        [Header("Player Aiming")]
        public Transform aimPos;
        public float aimSmoothSpeed = 20f;
        public LayerMask aimMask;


        [Header("Camera Setting")]
        public float cameraHorizontalSpeed = 2.0f;
        public float cameraVerticalSpeed = 2.0f;


        [Header("Weapon FOV")]
        public float defaultFOV;
        public float aimFOV;

        [Header("Interaction UI")]
        public InteractionSensor interactionSensor;




        public AnimationEventListener animationEventListener;

        private CharacterBase characterBase;
        private Camera mainCamera;
        private CharacterController controller;
        private Animator animator;
        private RigBuilder rigBuilder;



        public bool isMove;
        public bool isSprint = false;
        public bool isWalk = false;
        public bool isClosedAim = false;
        public bool isShooting = false;
        public bool isReloading = false;
        private Vector2 move;
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        [Range(0.0f, 0.3f)] public float rotationSmoothTime = 0.12f;
        private float rotationVelocity;
        private float verticalVelocity;


        public LayerMask groundLayers;
        public float groundCheckRadius = 0.3f;
        public Vector3 groundCheckOffset;
        public float freefallTimeValue = 0f;


        public float jumpForce = 5.0f;
        public float jumpHeight = 1.0f;
        public float jumpTimeDelta = 1f;


        public float moveSpeed = 3.0f;
        public float sprintSpeed = 5.0f;
        public float speedChangeRate = 10.0f;
        public float dashSpeedDeltaMultiplier = 2f;
        private float rifleHoldingTimer = 0f;
        public bool isArmed = false;

        private bool isStrafe = false;

        [Header("Weapon Holder")]
        public WeaponManager weaponManager;
        public GameObject weaponHolder;
        public Weapon currentWeapon;
        public WeaponGrenade currentGrenade;
        public GameObject scifiRifle_Dummy;
        public GameObject grenadePrefab;
        public GameObject grenade_Dummy;
        // public RigBuilder rigbuilder;





        private bool isEnablemovement = true;
        private bool isGrounded = false;

        public bool IsEnableMovemnt
        {
            set => isEnablemovement = value;
        }


        private void Awake()
        {
            Instance = this;


            // �ִϸ��̼� Ÿ�̹� ���� ��������Ʈ
            animationEventListener = GetComponentInChildren<AnimationEventListener>();
            animationEventListener.OnTakeAnimationEvent += RifleDrawTiming;
            animationEventListener.OnTakeAnimationEvent += RifleHolsterTiming;
            animationEventListener.OnTakeAnimationEvent += GrenadeThrow;


            // ������Ʈ 
            characterBase = GetComponent<CharacterBase>();
            animator = GetComponentInChildren<Animator>();
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            rigBuilder = GetComponentInChildren<RigBuilder>();




            //var weaponGameObject = TransformUtility.FindGameObjectWithTag(weaponHolder, "Weapon");
            //currentWeapon = weaponGameObject.gameObject.GetComponent<Weapon>();
            // currentWeapon = weaponManager.weapon[0].GetComponent<Weapon>();
            


            // �ƹ�Ÿ
            // scifiRifle_Dummy = GameObject.Find("ScifiRifleWLT78MasterPrefab_Dummy");


            // �÷��̾�� ��ȣ�ۿ� ����
            interactionSensor = GetComponentInChildren<InteractionSensor>();

        }


        private void OnDestroy()
        {
            Instance = null;
        }


        private void RifleHolsterTiming(string name)
        {
            if (name.Equals("Holster_Rifle"))
            {
                currentWeapon.gameObject.SetActive(false);
                scifiRifle_Dummy.gameObject.SetActive(true);
            }
        }

        private void RifleDrawTiming(string eventName)
        {
            if (eventName.Equals("Equip_Rifle"))
            {
                currentWeapon.gameObject.SetActive(true);
                scifiRifle_Dummy.gameObject.SetActive(false);
            }
        }

        private void GrenadeThrow(string eventName)
        {
            if (eventName.Equals("Grenade_Throw"))
            {
                grenade_Dummy.SetActive(false);
                GameObject grenade = Instantiate(grenadePrefab, grenade_Dummy.transform.position, grenade_Dummy.transform.rotation);
                var rigidbody = grenade.gameObject.GetComponent<Rigidbody>();
                rigidbody.AddForce(transform.parent.forward * 500f);
                //grenade.GetComponent<WeaponGrenade>().Settings(30f, transform.parent.forward);
            }
        }


        // ��ȣ�ۿ�
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



        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // �̷��� �ȵ�
            // GameObject.Find("GameDataModel").GetComponent<GameDataModel>.myDummyData.characterMoveSpeed

            // moveSpeed = GameDataModel.Instance.myDummyData.characterMoveSpeed;

        }

        private void Update()
        {
            // �̰� ��� ������ ĳ���� �� ������
            // moveSpeed = GameDataModel.Singleton.myDummyData.characterMoveSpeed;

            // ��ȣ�ۿ� Ű
            if (Input.GetKeyDown(KeyCode.F))
            {
                // ���࿡ ���� UI���� ������ ��ȣ�ۿ� �������� ItemBox��� => Player Character �� �ݱ� ����� Ʈ�����Ѵ�.
                // var contents = InteractionUI.Instance.GetInteractionContents();
                // if(contents == ItemBox)
                //   Player Character �� �ݱ� ����� Ʈ����.

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

            // ĳ���� ���� ���� ��ǲ
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimMask))
            {
                aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
            }




            // ĳ���� ���� ���� ��ǲ

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            move = new Vector2(horizontal, vertical);

            float hMouse = Input.GetAxis("Mouse X");
            float vMouse = Input.GetAxis("Mouse Y") * (-1);
            look = new Vector2(hMouse, vMouse);

            isSprint = Input.GetKey(KeyCode.LeftShift);
            isWalk = move.magnitude != 0 && !isSprint;
            isStrafe = Input.GetKey(KeyCode.Mouse1); // Mouse Right Button
            if (isStrafe)
            {
                Vector3 cameraForward = Camera.main.transform.forward.normalized;
                cameraForward.y = 0;
                transform.forward = cameraForward;
            }

            GroundCheck();
            FreeFall();
            Move();

            /*if(interactionSensor.HasInteractable)
            {
                move = Vector2.zero;
            }*/

            // �ִϸ��̼� ��Ʈ��
            animator.SetBool("IsMoving", move.magnitude != 0);
            animator.SetFloat("Speed", animationBlend);
            animator.SetFloat("Horizontal", move.x);
            animator.SetFloat("Vertical", move.y);
            animator.SetFloat("Strafe", isStrafe ? 1 : 0);



            // ���� ����ƮŰ ������ ��, �ִϸ��̼� �ӵ� ����
            // GetKey : ������ ���� true
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetFloat("DashSpeedMultiplier", dashSpeedDeltaMultiplier);
            }
            else
            {
                animator.SetFloat("DashSpeedMultiplier", 1f);
            }

            // ����
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                animator.SetTrigger("Jump");
                Jump();
                // verticalVelocity = Mathf.Sqrt(this.jumpHeight * 9.81f);
            }



            // ���� ������ ���� ���� ����
            // scifiRifle.SetActive(isArmed);

            // ���� ���� ���� ��, Draw Rifle �ִϸ��̼��� ������ Grip Ready ���� ����
            /*rigbuilder.enabled = animator.GetBool("Ready_Rifle");
             */






            // ��� ������ ���� ���� ���� ������ �۵���
            int rifle_fire = animator.GetInteger("Rifle_Fire");
            rigBuilder.layers[0].active = Convert.ToBoolean(rifle_fire);


            // ����, ������� ��ȯ
            if (Input.GetKeyDown(KeyCode.U) && !isArmed)
            {
                isArmed = true;
                animator.SetBool("isArmed", true);
                //bool isEquipped = animator.GetBool("Rifle_Active");
                //currentWeapon.gameObject.SetActive(true);

            }
            else if (Input.GetKeyDown(KeyCode.U) && isArmed)
            {
                isArmed = false;
                animator.SetBool("isArmed", false);
                //bool isEquipped = animator.GetBool("Rifle_Active");
                //currentWeapon.gameObject.SetActive(false);
            }

            isShooting = Input.GetKey(KeyCode.Mouse0) && currentWeapon.currentBullet_inMagazine > 0;


            // ����ź ������
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                animator.SetTrigger("Grenade");
                grenade_Dummy.gameObject.SetActive(true);
            }




            if (isArmed)
            {

                // ���� ��� ���Ⱑ ���ǰ� �ִ��� ������Ʈ
                /*
                foreach (var weapon_ in weaponManager.weapon)
                {
                    if (weapon_.activeSelf == true)
                        currentWeapon = weapon_.GetComponent<Weapon>();
                }
                */

                animator.SetTrigger("Armed_Rifle");

                // ������ ����
                if (Input.GetKeyDown(KeyCode.R) && !currentWeapon.isReload &&
                        currentWeapon.currentBullet_inMagazine < currentWeapon.weaponSO.magazineCapacity && currentWeapon.bulletTotal != 0)
                {
                    isReloading = true;

                    //currentWeapon.StartCoroutine(currentWeapon.ReloadCoroutine());
                    animator.SetTrigger("Reload");
                }



                if (Input.GetKey(KeyCode.Mouse0) && !currentWeapon.isReload)
                {
                    if (currentWeapon.currentBullet_inMagazine > 0 && !isReloading)
                    {
                        animator.SetInteger("Rifle_Fire", 1);
                        currentWeapon?.Attack(); // ���� ����
                        animator.SetTrigger("Rifle_Shoot");
                        rifleHoldingTimer = 0.5f;
                    }


                    var cameraForward = Camera.main.transform.forward.normalized;
                    cameraForward.y = 0;
                    transform.forward = cameraForward;

                }
                // �� �� ���� �������� �� ��� �ٽ� ���� �������´�
                else
                {
                    rifleHoldingTimer -= Time.deltaTime;
                }

                if (rifleHoldingTimer > 0)
                {
                    animator.SetInteger("Rifle_Fire", 1);
                }
                else
                {
                    animator.SetInteger("Rifle_Fire", 0);
                }

            }



            // Idle ��� ���¿��� ���� �ڼ��� Ż���ϴ� ����
            // isWalk = isMove || isSprint;
            // animator.SetBool("RifleFire_Idle_Exit", isMove || rifleHoldingTimer < 0);



            // ���콺 ��Ŭ���� �ϰ� ������ �����ϱ�
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                // zoom in
                isClosedAim = true;
                CameraSystem.Instance.TargetFOV = aimFOV;
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                // zoom out
                isClosedAim = false;
                CameraSystem.Instance.TargetFOV = defaultFOV;
            }



        }

        // -------------------------------------------------------------------------------------------------------------------------



        public void Jump()
        {
            verticalVelocity = jumpForce;
            jumpTimeDelta = 0.3f;
        }

        private void GroundCheck()
        {
            Ray ray = new Ray(transform.position + groundCheckOffset, Vector3.down);
            isGrounded = Physics.SphereCast(ray, groundCheckRadius, 0.1f, groundLayers);

            if (!isGrounded)
            {
                freefallTimeValue += Time.deltaTime;
            }
            else
            {
                freefallTimeValue = 0f;
            }
        }

        private void FreeFall()
        {
            if (jumpTimeDelta > 0f)
            {
                jumpTimeDelta -= Time.deltaTime;
            }

            if (jumpTimeDelta <= 0f)
            {
                if (isGrounded)
                {
                    verticalVelocity = 0f;
                }
                else
                {
                    verticalVelocity = -9.81f * freefallTimeValue;
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

        public void ReloadFinished()
        {
            isReloading = false;
            currentWeapon.Reload();
        }
    }
}


