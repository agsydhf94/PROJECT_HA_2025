using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
namespace HA
{
    public class BossEnemy : CharacterBase
    {
        // Segment �ı� ����
        public delegate void DestroySequence_Start();
        public DestroySequence_Start destroySequence_Start;

        public static Action<(Transform, int)> destroySequence_Grounded;

        public static Action<ParticleSystem> bossProjectile_BackToPool;



        public Transform playerTransform; // �÷��̾� Transform
        public float currentBossPosition;
        public Transform bossHead; // ������ �Ӹ� Transform
        public float headTurnSpeed = 2f; // �Ӹ��� ȸ���ϴ� �ӵ�
        public EnemySegment[] segments;
        public Canvas bossHPBar;

        [Header("Attack Settings")]
        public GameObject[] projectilePrefab; // �߻�ü ������
        public Transform firePoint; // �߻�ü �߻� ��ġ
        public float projectileSpeed; // �߻�ü �ӵ�

        [Header("Cinemachine Settings")]
        public CinemachineSmoothPath cinemachineSmoothPath;
        public CinemachineDollyCart dollyCart; // DollyCart�� ���� ��ġ ����
        public int[] attackWaypoints;
        private int lastWaypointIndex = -1; // ���� ��������Ʈ ����

        [Header("Phase Settings")]
        public float bossHealth = 100f; // ���� ü��
        public float phase2HealthThreshold = 70f; // ������ 2�� ��ȯ�Ǵ� ü��
        public float phase3HealthThreshold = 30f; // ������ 3���� ��ȯ�Ǵ� ü��

        private Phase currentPhase = Phase.Phase1; // ���� ������ (enum���� ����)

        [Header("Phase 1")]
        public float phase1AttackInterval = 5f;

        [Header("Phase 2")]
        public float phase2AttackInterval = 3f;
        public int phase2ProjectileCount = 5;

        [Header("Phase 3")]
        public float phase3AttackInterval = 2f;
        public float spiralAttackAngleStep = 30f;

        [Header("Boss Attack Properties")]
        private Vector3 attackDirection;
        private float attackProjectile_Speed;

        [Header("Laser Beam")]
        public GameObject rayStartObject;
        public Transform rayStartTransform; // ����ĳ������ ���۵Ǵ� ��ġ
        public GameObject beamLength_ByScale; // �´� �������� �Ÿ��� ����Ͽ� Beam ������Ʈ�� ������ ����
        public GameObject rayTouchedResult; // �´� ��ġ�� ��� ���
        public ParticleSystem bossWarningCirele;
        public Vector3 bossWarningCircle_PosCorrection;
        private float beamHitDistance;
        

        private bool warningStartFlag = false;
        private bool isWarningCircleOn = false;
        [SerializeField] private float elapsedTime;
        public float warningTime;
        private float attackTimer = 0f;
        private int dummyCounter;
        public static Dictionary<int, DetachedSegment> detachedSegment = new Dictionary<int, DetachedSegment>();

        public Vector3 rayStart;
        public Vector3 characterPos;

        // Enum ����
        private enum Phase
        {
            Phase1,
            Phase2,
            Phase3
        }

        private void Awake()
        {
            foreach(var segment in segments)
            {
                currentHP += segment.currentHP;
            }

            dummyCounter = 0;

            destroySequence_Grounded -= SecondExplosion;
            destroySequence_Grounded += SecondExplosion;
        }

        void Update()
        {
            playerTransform = GameManager.Instance.currentActiveCharacter.transform;
            rayStartTransform = rayStartObject.transform;
            characterPos = GameManager.Instance.currentActiveCharacter.transform.position;
            rayStart = rayStartTransform.position;


            RotateHeadTowardsPlayer();
            CheckWaypointAndAttack();

            CheckPhase();

            if(elapsedTime >= warningTime)
            {
                elapsedTime = 0f;
                isWarningCircleOn = false;
                warningStartFlag = false;
                
            }


            rayStartObject.transform.LookAt(playerTransform);
            beamLength_ByScale.transform.localScale = new Vector3(0.01666667f, 0.01666667f, beamHitDistance);
            
        }

        

        private void CheckWaypointAndAttack()
        {
            if (dollyCart == null || cinemachineSmoothPath == null) return;

            // ���������� ���� ���� �ֱ��� ��������Ʈ �ε��� ���
            int currentWaypointIndex = GetLastPassedWaypoint();
            Debug.Log($"���� ��ġ �ε��� : {currentWaypointIndex}");

            if(Array.Exists(attackWaypoints, x => x == currentWaypointIndex) && !warningStartFlag)
            {
                warningStartFlag = true;
                Debug.Log("��� ����!!!!!!!!!!!!!!!!!!!!!!!");
                LaserWarningAndAttack();
            }

            

            // Ư�� ��������Ʈ�� ó�� �������� ���� ���� ����
            /*
            if (attackWaypoints.Contains(currentWaypointIndex) && currentWaypointIndex != lastWaypointIndex)
            {
                PerformAttack();
                lastWaypointIndex = currentWaypointIndex; // ����� ��������Ʈ ���
            }
            */
        }

        
        private int GetLastPassedWaypoint()
        {
            if (cinemachineSmoothPath.m_Waypoints.Length == 0)
            {
                Debug.LogWarning("��������Ʈ�� �������� �ʾҽ��ϴ�.");
                return -1;
            }

            int left = 0;
            int right = cinemachineSmoothPath.m_Waypoints.Length - 1;
            int lastWaypointIndex = 0;
            float cartPosition = dollyCart.m_Position;

            while (left <= right)
            {
                int mid = left + (right - left) / 2; // �߰� �ε��� ���
                float waypointStartPos = cinemachineSmoothPath.FromPathNativeUnits(mid, CinemachinePathBase.PositionUnits.Distance);
                float waypointEndPos = (mid + 1 < cinemachineSmoothPath.m_Waypoints.Length)
                    ? cinemachineSmoothPath.FromPathNativeUnits(mid + 1, CinemachinePathBase.PositionUnits.Distance)
                    : float.MaxValue; // ������ ��������Ʈ�� ���, ���� ���Ѵ�� ����


                // ���� ��ġ�� �� ��������Ʈ ���� �ȿ� �ִ� ���
                if (cartPosition >= waypointStartPos && cartPosition < waypointEndPos)
                {
                    lastWaypointIndex = mid;
                    break;
                }
                else if (cartPosition < waypointStartPos)
                {
                    right = mid - 1; // ���� ������ �̵�
                }
                else
                {
                    left = mid + 1; // ������ ������ �̵�
                }
            }

            Debug.Log($"���������� ���� ��������Ʈ: {lastWaypointIndex}");
            return lastWaypointIndex;
        }
        


        /*
        private int GetLastPassedWaypoint()
        {
            int lastWaypointIndex = 0;
            float cartPosition = dollyCart.m_Position; // ���� dolly īƮ ��ġ

            //Debug.Log($"Cart Position: {cartPosition} / Total Path Length: {totalPathLength}");

            for (int i = 0; i < cinemachineSmoothPath.m_Waypoints.Length - 1; i++)
            {
                // i��° ��������Ʈ�� i+1��° ��������Ʈ�� ��ü ��ο����� �Ÿ����� ������
                float waypointStartPos = cinemachineSmoothPath.FromPathNativeUnits(i, CinemachinePathBase.PositionUnits.Distance);
                float waypointEndPos = cinemachineSmoothPath.FromPathNativeUnits(i + 1, CinemachinePathBase.PositionUnits.Distance);

                Debug.Log($"Waypoint {i}: Start={waypointStartPos}, End={waypointEndPos}");

                // ���� dollyCart.m_Position�� �ش� ��������Ʈ ������ �ִ��� üũ
                if (cartPosition >= waypointStartPos && cartPosition < waypointEndPos)
                {
                    lastWaypointIndex = i;
                    Debug.Log($"Last Passed Waypoint Found: {lastWaypointIndex}");
                    break;
                }
            }

            return lastWaypointIndex;
        }
        */
       

        public void AttackByTimeCount()
        {
            attackTimer += Time.deltaTime;
            float currentAttackInterval = GetCurrentAttackInterval();
            if (attackTimer >= currentAttackInterval)
            {
                PerformAttack();
                attackTimer = 0f;
            }
        }

        public void UpdateTotalHealth()
        {
            float totalHealth = 0f;
            foreach (var segment in segments)
            {
                totalHealth += segment.currentHP;
            }
            currentHP = totalHealth;

            // ������ ������ ��ü ����
            if (bossHealth <= 0)
            {
                Die();
            }
        }

        private void RotateHeadTowardsPlayer()
        {
            // �÷��̾� ���� ���
            Vector3 directionToPlayer = (playerTransform.position - bossHead.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            bossHead.rotation = Quaternion.Slerp(bossHead.rotation, targetRotation, Time.deltaTime * headTurnSpeed);
        }

        private void CheckPhase()
        {
            if (bossHealth <= phase3HealthThreshold && currentPhase != Phase.Phase3)
            {
                currentPhase = Phase.Phase3;
                Debug.Log("Phase 3 Activated!");
            }
            else if (bossHealth <= phase2HealthThreshold && currentPhase != Phase.Phase2)
            {
                currentPhase = Phase.Phase2;
                Debug.Log("Phase 2 Activated!");
            }
        }

        private float GetCurrentAttackInterval()
        {
            switch (currentPhase)
            {
                case Phase.Phase1: return phase1AttackInterval;
                case Phase.Phase2: return phase2AttackInterval;
                case Phase.Phase3: return phase3AttackInterval;
                default: return phase1AttackInterval;
            }
        }

        private void PerformAttack()
        {
            switch (currentPhase)
            {
                case Phase.Phase1:
                    Phase1_SingleProjectileAttack();
                    break;
                case Phase.Phase2:
                    Phase2_MultiProjectileAttack();
                    break;
                case Phase.Phase3:
                    Phase3_SpiralAttack();
                    break;
            }
        }


        private async void Phase1_SingleProjectileAttack()
        {
            // ���� �Ӹ��� �÷��̾ �ٶ󺸵��� ȸ��
            var targetDirection = playerTransform.position - rayStartTransform.position;
            RotateHeadTowardsPlayer();

            var projectile = ObjectPool.Instance.GetFromPool<VisualEffect>("boss_ProjectileA");
            projectile.GetComponent<Boss_ProjectileA>().impactFlag = false;
            projectile.GetComponent<Boss_ProjectileA>().SetTargetDirection(targetDirection, projectileSpeed);

            // firePoint���� �����Ͽ� ���� �Ӹ��� �ٶ󺸴� �������� �߻�
            projectile.transform.position = firePoint.position;
            projectile.transform.LookAt(playerTransform);

            Quaternion rotationAdjust = Quaternion.Euler(0, -90, 0);
            projectile.transform.rotation *= rotationAdjust;

            await BossProjectileReturnPool(projectile);


        }

        public async UniTask BossProjectileReturnPool(VisualEffect projectile)
        {
            var projectileComponent = projectile.GetComponent<Boss_ProjectileA>();

            await UniTask.WaitUntil(() => projectileComponent.impactFlag);
            await UniTask.Delay(1000);

            ObjectPool.Instance.ReturnToPool("boss_ProjectileA", projectile);
        }

        private void Phase2_MultiProjectileAttack()
        {
            if (projectilePrefab != null && firePoint != null)
            {
                for (int i = 0; i < phase2ProjectileCount; i++)
                {
                    // ������ �������� ���ݾ� �����Ͽ� ���� �������� �߻�
                    float angleOffset = UnityEngine.Random.Range(-15f, 15f);
                    Quaternion rotation = Quaternion.Euler(0, angleOffset, 0) * firePoint.rotation;

                    GameObject projectile = Instantiate(projectilePrefab[1], firePoint.position, rotation);
                    Rigidbody rb = projectile.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = projectile.transform.forward * projectileSpeed;
                    }
                }
            }
        }

        private void Phase3_SpiralAttack()
        {
            if (projectilePrefab != null && firePoint != null)
            {
                float currentAngle = 0f;
                while (currentAngle < 360f)
                {
                    Quaternion rotation = Quaternion.Euler(0, currentAngle, 0) * firePoint.rotation;
                    GameObject projectile = Instantiate(projectilePrefab[2], firePoint.position, rotation);
                    Rigidbody rb = projectile.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = projectile.transform.forward * projectileSpeed;
                    }
                    currentAngle += spiralAttackAngleStep;
                }
            }
        }


        

        // Segment Destroy Sequence �� ���õ� �޼���



        public void SegmentHide_And_SetDummy(int index, Transform lastTransform)
        {
            
            var segment = segments[index];
            segment.segmentRenderer.enabled = false;
            segment.sphereCollider.enabled = false;

            var dummySegment = ObjectPool.Instance.GetFromPool<DetachedSegment>("bossEnemySegmenmt_Dummy");
            dummySegment.key = dummyCounter;
            dummySegment.transform.position = lastTransform.position;
            dummySegment.AddComponent<Rigidbody>();
            dummySegment.AddComponent<SphereCollider>();
            detachedSegment.Add(dummyCounter, dummySegment);
            Debug.Log($"��ųʸ��� �����߰� Adding key: {dummyCounter}, Object: {dummySegment}");
            dummyCounter++;
            
            

            StartCoroutine(segment.FirstExplosion(lastTransform));
            
        }

        private async void SecondExplosion((Transform, int) infoTuple)
        {
            var momentOiImpact_Transform = infoTuple.Item1;
            var key = infoTuple.Item2;

            var explosionEffect = ObjectPool.Instance.GetFromPool<ParticleSystem>("BossSegmentExplosionPrefab");
            explosionEffect.transform.position = momentOiImpact_Transform.position;

            Debug.Log($"Accessing key: {key}");
            var dummySegment = detachedSegment[key];
            dummySegment.key = 0;
            ObjectPool.Instance.ReturnToPool("bossEnemySegmenmt_Dummy", dummySegment);
            detachedSegment.Remove(key);

            // StartCoroutine(ExplosionEffect_Dequeue(explosionEffect));
            await ExplosionEffect_Dequeue(explosionEffect);
        }

        public async UniTask ExplosionEffect_Dequeue(ParticleSystem explosionEffect)
        {
            await UniTask.Delay(4000);

            ObjectPool.Instance.ReturnToPool("BossSegmentExplosionPrefab", explosionEffect);
            //Debug.Log("�ٽ� ��!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        

        private async void LaserWarningAndAttack()
        {
            beamLength_ByScale.SetActive(true);
            Debug.Log("���� Ȱ��ȭ!");

            await LaserBeam();

            PerformAttack();    
        }
      
        private async UniTask LaserBeam()
        {
            while(elapsedTime < warningTime)
            {
                // �������� �÷��̾� �������� ����ĳ��Ʈ ����
                Vector3 directionToPlayer = (playerTransform.position - rayStartTransform.position).normalized;
                Physics.Raycast(rayStartTransform.position, directionToPlayer, out RaycastHit hit, 10000, LayerMask.GetMask("Ground"));
                beamHitDistance = hit.distance;

                // ��� ��Ŭ�� ���� Ȱ��ȭ���� �ʾҴٸ� ������Ʈ Ǯ���� ������
                if (!isWarningCircleOn)
                {
                    var particleEffect = ObjectPool.Instance.GetFromPool<ParticleSystem>("bossWarningCircle");
                    bossWarningCirele = particleEffect;
                    isWarningCircleOn = true;
                }
                // ��� ��Ŭ�� ��ġ�� ȸ�� ���� (���鿡 ���� ��ġ)
                bossWarningCirele.transform.position = hit.point + bossWarningCircle_PosCorrection;
                bossWarningCirele.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rayStartTransform.forward, hit.normal), hit.normal);

                await UniTask.Yield(); // ���� �����ӱ��� ���
                elapsedTime += Time.deltaTime;
                
            }

            ObjectPool.Instance.ReturnToPool("bossWarningCircle", bossWarningCirele);

            beamLength_ByScale.SetActive(false);
            Debug.Log("���� ��Ȱ��ȭ");
        }

        public override void Die()
        {
            base.Die();
            bossHPBar.planeDistance = -0.5f;
        }

        /*
        private async UniTask LaserBeam()
        {
            Vector3 lastTargetPosition = playerTransform.position; // �ʱ� Ÿ�� ��ġ ����
            Vector3 lastDirection = playerTransform.position - rayStartObject.transform.position; // �ʱ� ȸ�� ����

            while (elapsedTime < warningTime)
            {
                // ��ǥ ���� ��� (���� �����ӿ��� ���� ������ ���󰡵��� ����)
                Vector3 targetPosition = Vector3.Lerp(lastTargetPosition, playerTransform.position, Time.deltaTime * 0.8f);
                lastTargetPosition = targetPosition;

                Vector3 targetDirection = Vector3.Lerp(lastDirection, playerTransform.position - rayStartObject.transform.position, Time.deltaTime * 0.8f);
                lastDirection = targetDirection;

                Vector3 directionToTarget = (lastTargetPosition - rayStartTransform.position).normalized;
                

                // Raycast ����
                if (Physics.Raycast(rayStartTransform.position, directionToTarget, out RaycastHit hit, 10000, LayerMask.GetMask("Ground")))
                {
                    beamHitDistance = hit.distance;
                    rayStartObject.transform.LookAt(hit.point);

                    if (!isWarningCircleOn)
                    {
                        var particleEffect = ObjectPool.Instance.GetFromPool<ParticleSystem>("bossWarningCircle");
                        bossWarningCirele = particleEffect;
                        isWarningCircleOn = true;
                    }

                    // ��� ��Ŭ ��ġ ����
                    bossWarningCirele.transform.position = hit.point + bossWarningCircle_PosCorrection;
                    bossWarningCirele.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rayStartTransform.forward, hit.normal), hit.normal);

                }

                await UniTask.Yield(); // ���� �����ӱ��� ���
                elapsedTime += Time.deltaTime;
            }

            ObjectPool.Instance.ReturnToPool("bossWarningCircle", bossWarningCirele);

            beamLength_ByScale.SetActive(false);
            Debug.Log("���� ��Ȱ��ȭ");
        }
        */


        private void OnDrawGizmos()
        {
            if (rayStartTransform == null || playerTransform == null) return;

            // ���� ���� ���
            Vector3 directionToPlayer = (playerTransform.position - rayStartTransform.position).normalized;

            // Raycast ���� (�浹 �˻�)
            if (Physics.Raycast(rayStartTransform.position, directionToPlayer, out RaycastHit hit, 10000))
            {
                // �浹�� �������� ���������� �� ǥ��
                Gizmos.color = Color.red;
                Gizmos.DrawLine(rayStartTransform.position, hit.point);

                // �浹�� ������ �� ǥ�� (������ 0.2)
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(hit.point, 0.2f);
            }
            else
            {
                // �浹���� �ʾ��� ���, �ִ� �Ÿ����� ��� �� ǥ��
                Gizmos.color = Color.green;
                Gizmos.DrawLine(rayStartTransform.position, rayStartTransform.position + directionToPlayer * 10000);
            }
        }

    }
}
