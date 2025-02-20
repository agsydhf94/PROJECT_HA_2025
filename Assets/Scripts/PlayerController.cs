using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;
using Random = UnityEngine.Random;
using Photon.Pun;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;

namespace HA
{
    public class PlayerController : MonoBehaviourPun
    {
        public static PlayerController Instance { get; private set; } = null;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
        }

        public CharacterBase PlayerCharacterBase => characterBase;
        public GameManager gameManager;


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
        public float aimSmoothSpeed;
        public LayerMask aimMask;
        private float minDistance = 500.0f; // �ּ� �Ÿ� ����
        private float maxDistance = 100f; // �ִ� ���� �Ÿ�

        public float aimingIKBlendCurrent;
        public float aimingIKBlendTarget;
        public Rig aimingRig;


        [Header("Camera Setting")]
        public float cameraHorizontalSpeed = 2.0f;
        public float cameraVerticalSpeed = 2.0f;
        public Vector3 cameraRotation;


        [Header("Weapon FOV")]
        public float defaultFOV;
        public float aimFOV;

        [Header("Interaction UI")]
        public InteractionSensor interactionSensor;

        


        public AnimationEventListener animationEventListener;

        private CharacterBase characterBase;
        private PlayerData playerData;
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

        [Header("Jump Settings")]
        public float jumpForce = 5.0f;
        public float jumpTimeout;
        public float jumpTimeoutDelta;
        public float groundOffset;
        public float groundRadius;
        public LayerMask groundLayer;

        public delegate void Trigger_CommandJump();
        public Trigger_CommandJump commandJump;


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
        public Transform weaponHandTransform;
        public Transform weaponBackTransform;
        public WeaponGrenade currentGrenade;
        public GameObject scifiRifle_Dummy;
        
        // public RigBuilder rigbuilder;

        public bool isShowCursor;

        

        private int attackComboCount = 0;
        public int AttackComboCount
        {
            set => attackComboCount = value;
        }


        private bool isEnablemovement = true;
        private bool isGrounded = false;

        public bool IsEnableMovemnt
        {
            set => isEnablemovement = value;
        }

        SkillManager skillManager;

        private void Awake()
        {
            Instance = this;


            // �ִϸ��̼� Ÿ�̹� ���� ��������Ʈ
            animationEventListener = GetComponentInChildren<AnimationEventListener>();
            animationEventListener.OnTakeAnimationEvent += RifleDrawTiming;
            animationEventListener.OnTakeAnimationEvent += RifleHolsterTiming;
            animationEventListener.OnTakeAnimationEvent += WeaponManager.Instance.GrenadeThrow;
            animationEventListener.OnTakeAnimationEvent += SlashAnimationHitEvent;




            // ������Ʈ 
            characterBase = GetComponent<CharacterBase>();
            playerData = GetComponent<PlayerData>();
            animator = GetComponentInChildren<Animator>();
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            rigBuilder = GetComponentInChildren<RigBuilder>();        
            skillManager = SkillManager.Instance;         
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
                // ���� ��ġ�� �÷��̾� ������ �̵�
                GetComponent<PlayerData>().currentWeapon.transform.SetParent(weaponBackTransform); // weaponBackTransform�� � �ش��ϴ� Ʈ������                

            }
        }

        private void RifleDrawTiming(string eventName)
        {
            if (eventName.Equals("Equip_Rifle"))
            {
                GetComponent<PlayerData>().currentWeapon.gameObject.SetActive(true);               

                // ���� ��ġ�� �÷��̾� ������ �̵�
                GetComponent<PlayerData>().currentWeapon.transform.SetParent(weaponHandTransform); // weaponHandTransform�� �� ��ġ�� Ʈ������
            }
        }


        

        private void SlashAnimationHitEvent(string eventName)
        {
            if(GetComponent<PlayerData>().currentWeapon != null)
            {
                if (eventName.Equals("KunoichiSlash_1"))
                {
                    // StartCoroutine(GetComponent<PlayerData>().currentWeapon.SlashVFX());
                }

                if (eventName.Equals("RPG_Combo1"))
                {
                    GetComponent<PlayerData>().currentWeapon.DetectAndDealDamage(0);
                }
                else if (eventName.Equals("RPG_Combo2"))
                {
                    GetComponent<PlayerData>().currentWeapon.DetectAndDealDamage(10);
                }
                else if (eventName.Equals("RPG_Combo3"))
                {
                    GetComponent<PlayerData>().currentWeapon.DetectAndDealDamage(20);
                }
            }

            
        }




        private void Start()
        {            
            commandJump += CommandJump;
        }

        private void Update()
        {
            // �¶��� ��Ƽ�÷��̿��� ����Ʈ �������� ����
            if((PhotonNetwork.IsConnected && !photonView.IsMine) || GameManager.Instance.isPause)
            {
                return;
            }


            // ��ȣ�ۿ� Ű
            if (Input.GetKeyDown(KeyCode.F))
            {
                var interactionUI = UIManager.Instance.GetUI<InteractionUI>(UIList.InteractionUI);
                interactionUI.DoInteract();
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

            CheckGround();
            FreeFall();
            Move();



            // �ִϸ��̼� ��Ʈ��
            animator.SetBool("IsMoving", move.magnitude != 0);
            animator.SetFloat("Magnitude", move.magnitude);
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
                animator.SetTrigger("JumpTrigger");
                Jump();
            }

            

            if (this.gameObject.GetComponent<PlayerData>().characterMode == CharacterMode.Shooter)
            {
                TPS_Shooting_Part();
            }
            else
            {
                RPG_Part();
            }


        }

        // -------------------------------------------------------------------------------------------------------------------------

        private void TPS_Shooting_Part()
        {
            /*
            // ĳ���� ���� ���� ��ǲA
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, aimMask))
            {
                aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
            }

            
            */


            // ĳ���� ���� ���� ��ǲB
            // ī�޶� ���߾ӿ��� ����ĳ��Ʈ ���
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            // ���� ����� ������ aimTarget ��ġ�� �����
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                float distance = Vector3.Distance(transform.position, hit.point);

                // �ּ� �Ÿ��� �����ϵ��� ����
                if (distance > minDistance)
                {
                    aimPos.position = hit.point;
                }
                else
                {
                    aimPos.position = ray.origin + ray.direction * minDistance;
                }
            }
            else
            {
                // �浹���� ������ �⺻ �Ÿ��� ����
                aimPos.position = ray.origin + ray.direction * maxDistance;
            }



            aimingIKBlendCurrent = Mathf.Lerp(aimingIKBlendCurrent, aimingIKBlendTarget, Time.deltaTime * 10f);
            aimingRig.weight = aimingIKBlendCurrent;

            if (animator.GetInteger("Rifle_Fire") == 1)
            {
                aimingIKBlendTarget = 1f; // ��� ���¿��� ��ǥ �� 1
            }
            else
            {
                aimingIKBlendTarget = 0f; // ���� ���¿��� ��ǥ �� 0
            }

            // ����ġ �ε巴�� ���� (������ ���� �Ǵ� ����)
            aimingIKBlendCurrent = Mathf.Lerp(aimingIKBlendCurrent, aimingIKBlendTarget, Time.deltaTime * 5f);

            // ���� ���̾� 0 (Body Aim, Right Hand Aim) Ȱ��ȭ ����ġ ����
            rigBuilder.layers[0].rig.weight = aimingIKBlendCurrent;

            

            // ����, ������� ��ȯ
            if (Input.GetKeyDown(KeyCode.U) && !isArmed && gameObject.GetComponent<PlayerData>().weaponCache.Count > 0)
            {
                isArmed = true;
                animator.SetBool("isArmed", true);

            }
            else if (Input.GetKeyDown(KeyCode.U) && isArmed)
            {
                isArmed = false;
                animator.SetBool("isArmed", false);
            }

            isShooting = Input.GetKey(KeyCode.Mouse0) && GetComponent<PlayerData>().currentWeapon 
                && GetComponent<PlayerData>().currentWeapon.currentBullet_inMagazine > 0;


            // ����ź ������
            if (Input.GetKeyDown(KeyCode.Mouse2) && WeaponManager.Instance.grenadeCount > 0)
            {
                animator.SetTrigger("Grenade");
                WeaponManager.Instance.grenade_Dummy.gameObject.SetActive(true);
            }




            if (isArmed)
            {

                

                animator.SetTrigger("Armed_Rifle");

                // ������ ����
                if (Input.GetKeyDown(KeyCode.R) && !GetComponent<PlayerData>().currentWeapon.isReload &&
                        GetComponent<PlayerData>().currentWeapon.currentBullet_inMagazine < GetComponent<PlayerData>().currentWeapon.weaponSO.magazineCapacity && GetComponent<PlayerData>().currentWeapon.bulletTotal != 0)
                {
                    isReloading = true;
                    GetComponent<PlayerData>().currentWeapon.PlaySE(GetComponent<PlayerData>().currentWeapon.weaponSO.reload_Sound);
                    animator.SetTrigger("Reload");
                }


                // �ܹ� ��� ��, �÷��� ����
                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<PlayerData>().currentWeapon.canShoot = true;
                }
                else if(Input.GetMouseButton(0))
                {
                    GetComponent<PlayerData>().currentWeapon.canShoot = false;
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    GetComponent<PlayerData>().currentWeapon.canShoot = true;
                }


                Debug.Log(GetComponent<PlayerData>().currentWeapon.isReload);

                if (Input.GetKey(KeyCode.Mouse0) && !GetComponent<PlayerData>().currentWeapon.isReload)
                {
                    if (GetComponent<PlayerData>().currentWeapon.currentBullet_inMagazine > 0 && !isReloading)
                    {
                        animator.SetInteger("Rifle_Fire", 1);
                        GetComponent<PlayerData>().currentWeapon?.Attack(); // ���� ����
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


            if (Input.GetKeyDown(KeyCode.Alpha2) && skillManager.activeSkillSlot[0].GetComponentInChildren<SkillSlot>().isCooled)
            {
                if (skillManager.activeSkillSlot[0].transform.childCount != 0 && !skillManager.isSkillUsing)
                {
                    SkillManager.Instance.SkillExecute(0);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && skillManager.activeSkillSlot[1].GetComponentInChildren<SkillSlot>().isCooled)
            {
                if (skillManager.activeSkillSlot[1].transform.childCount != 0 && !skillManager.isSkillUsing) 
                {
                    SkillManager.Instance.SkillExecute(1);
                }
            }



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


        private void RPG_Part()
        {
            // �޺� �׼�
            if (Input.GetKeyDown(KeyCode.Mouse0) && GetComponent<PlayerData>().currentWeapon != null)
            {
                if (attackComboCount == 0)
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

            if(Input.GetKeyDown(KeyCode.Alpha2) && skillManager.activeSkillSlot[0].GetComponentInChildren<SkillSlot>().isCooled)
            {
                if (skillManager.activeSkillSlot[0].transform.childCount != 0 && !skillManager.isSkillUsing)
                {
                    SkillManager.Instance.SkillExecute(0);
                }
            }
            if(Input.GetKeyDown(KeyCode.Alpha3) && skillManager.activeSkillSlot[1].GetComponentInChildren<SkillSlot>().isCooled)
            {
                if (skillManager.activeSkillSlot[1].transform.childCount != 0 && !skillManager.isSkillUsing)
                {
                    SkillManager.Instance.SkillExecute(1);
                }
            }
        }

        // Common Settings

        private void CommandJump()
        {
            Jump();
        }

        public void Jump()
        {
            if (isGrounded == false || jumpTimeoutDelta > 0f)
                return;

            verticalVelocity = jumpForce;
            jumpTimeoutDelta = jumpTimeout;

            // Todo: �����ϴ� �ִϸ��̼� �����ϴ� ó��

        }

        public void FreeFall()
        {
            jumpTimeoutDelta -= Time.deltaTime;
            if (isGrounded == false)
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                if (jumpTimeoutDelta > 0)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    verticalVelocity = 0f;
                }
            }
        }

        public void CheckGround()
        {
            Vector3 spherePosition = transform.position + (Vector3.down * groundOffset);
            isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);

            // Todo : ĳ���Ͱ� �����ִٰ� �ִϸ��������� �˷��ִ� ó��
            animator.SetBool("isGrounded", isGrounded);

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
            GetComponent<PlayerData>().currentWeapon.Reload();
        }


        


        void OnAnimatorIK(int layerIndex)
        {
            if (aimPos != null)
            {
                Vector3 directionToTarget = aimPos.position - transform.position;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                // ���� ������ 60�� ������ ��쿡�� IK ���� (���� ���� ����)
                if (angleToTarget < 60f)
                {
                    animator.SetIKPosition(AvatarIKGoal.RightHand, aimPos.position);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, Mathf.Lerp(animator.GetIKPositionWeight(AvatarIKGoal.RightHand), 1.0f, Time.deltaTime * 5f));
                }
                else
                {
                    // ������ ����� IK �������� ����
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                }
            }
        }

        



    }
}

