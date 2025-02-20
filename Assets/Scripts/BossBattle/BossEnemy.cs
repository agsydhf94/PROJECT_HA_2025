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
        // Segment 파괴 절차
        public delegate void DestroySequence_Start();
        public DestroySequence_Start destroySequence_Start;

        public static Action<(Transform, int)> destroySequence_Grounded;

        public static Action<ParticleSystem> bossProjectile_BackToPool;



        public Transform playerTransform; // 플레이어 Transform
        public float currentBossPosition;
        public Transform bossHead; // 보스의 머리 Transform
        public float headTurnSpeed = 2f; // 머리가 회전하는 속도
        public EnemySegment[] segments;
        public Canvas bossHPBar;

        [Header("Attack Settings")]
        public GameObject[] projectilePrefab; // 발사체 프리팹
        public Transform firePoint; // 발사체 발사 위치
        public float projectileSpeed; // 발사체 속도

        [Header("Cinemachine Settings")]
        public CinemachineSmoothPath cinemachineSmoothPath;
        public CinemachineDollyCart dollyCart; // DollyCart로 현재 위치 추적
        public int[] attackWaypoints;
        private int lastWaypointIndex = -1; // 이전 웨이포인트 저장

        [Header("Phase Settings")]
        public float bossHealth = 100f; // 보스 체력
        public float phase2HealthThreshold = 70f; // 페이즈 2로 전환되는 체력
        public float phase3HealthThreshold = 30f; // 페이즈 3으로 전환되는 체력

        private Phase currentPhase = Phase.Phase1; // 현재 페이즈 (enum으로 관리)

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
        public Transform rayStartTransform; // 레이캐스팅이 시작되는 위치
        public GameObject beamLength_ByScale; // 맞는 지점과의 거리를 고려하여 Beam 오브젝트의 스케일 조정
        public GameObject rayTouchedResult; // 맞는 위치에 띄울 요소
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

        // Enum 정의
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

            // 마지막으로 지난 가장 최근의 웨이포인트 인덱스 계산
            int currentWaypointIndex = GetLastPassedWaypoint();
            Debug.Log($"현재 위치 인덱스 : {currentWaypointIndex}");

            if(Array.Exists(attackWaypoints, x => x == currentWaypointIndex) && !warningStartFlag)
            {
                warningStartFlag = true;
                Debug.Log("경고 개시!!!!!!!!!!!!!!!!!!!!!!!");
                LaserWarningAndAttack();
            }

            

            // 특정 웨이포인트에 처음 도달했을 때만 공격 실행
            /*
            if (attackWaypoints.Contains(currentWaypointIndex) && currentWaypointIndex != lastWaypointIndex)
            {
                PerformAttack();
                lastWaypointIndex = currentWaypointIndex; // 실행된 웨이포인트 기록
            }
            */
        }

        
        private int GetLastPassedWaypoint()
        {
            if (cinemachineSmoothPath.m_Waypoints.Length == 0)
            {
                Debug.LogWarning("웨이포인트가 설정되지 않았습니다.");
                return -1;
            }

            int left = 0;
            int right = cinemachineSmoothPath.m_Waypoints.Length - 1;
            int lastWaypointIndex = 0;
            float cartPosition = dollyCart.m_Position;

            while (left <= right)
            {
                int mid = left + (right - left) / 2; // 중간 인덱스 계산
                float waypointStartPos = cinemachineSmoothPath.FromPathNativeUnits(mid, CinemachinePathBase.PositionUnits.Distance);
                float waypointEndPos = (mid + 1 < cinemachineSmoothPath.m_Waypoints.Length)
                    ? cinemachineSmoothPath.FromPathNativeUnits(mid + 1, CinemachinePathBase.PositionUnits.Distance)
                    : float.MaxValue; // 마지막 웨이포인트인 경우, 끝을 무한대로 설정


                // 현재 위치가 이 웨이포인트 범위 안에 있는 경우
                if (cartPosition >= waypointStartPos && cartPosition < waypointEndPos)
                {
                    lastWaypointIndex = mid;
                    break;
                }
                else if (cartPosition < waypointStartPos)
                {
                    right = mid - 1; // 왼쪽 범위로 이동
                }
                else
                {
                    left = mid + 1; // 오른쪽 범위로 이동
                }
            }

            Debug.Log($"마지막으로 지난 웨이포인트: {lastWaypointIndex}");
            return lastWaypointIndex;
        }
        


        /*
        private int GetLastPassedWaypoint()
        {
            int lastWaypointIndex = 0;
            float cartPosition = dollyCart.m_Position; // 현재 dolly 카트 위치

            //Debug.Log($"Cart Position: {cartPosition} / Total Path Length: {totalPathLength}");

            for (int i = 0; i < cinemachineSmoothPath.m_Waypoints.Length - 1; i++)
            {
                // i번째 웨이포인트와 i+1번째 웨이포인트의 전체 경로에서의 거리값을 가져옴
                float waypointStartPos = cinemachineSmoothPath.FromPathNativeUnits(i, CinemachinePathBase.PositionUnits.Distance);
                float waypointEndPos = cinemachineSmoothPath.FromPathNativeUnits(i + 1, CinemachinePathBase.PositionUnits.Distance);

                Debug.Log($"Waypoint {i}: Start={waypointStartPos}, End={waypointEndPos}");

                // 현재 dollyCart.m_Position이 해당 웨이포인트 범위에 있는지 체크
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

            // 보스가 죽으면 전체 제거
            if (bossHealth <= 0)
            {
                Die();
            }
        }

        private void RotateHeadTowardsPlayer()
        {
            // 플레이어 방향 계산
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
            // 보스 머리가 플레이어를 바라보도록 회전
            var targetDirection = playerTransform.position - rayStartTransform.position;
            RotateHeadTowardsPlayer();

            var projectile = ObjectPool.Instance.GetFromPool<VisualEffect>("boss_ProjectileA");
            projectile.GetComponent<Boss_ProjectileA>().impactFlag = false;
            projectile.GetComponent<Boss_ProjectileA>().SetTargetDirection(targetDirection, projectileSpeed);

            // firePoint에서 생성하여 보스 머리가 바라보는 방향으로 발사
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
                    // 각도를 랜덤으로 조금씩 변경하여 여러 방향으로 발사
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


        

        // Segment Destroy Sequence 에 관련된 메서드



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
            Debug.Log($"딕셔너리에 원소추가 Adding key: {dummyCounter}, Object: {dummySegment}");
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
            //Debug.Log("다시 들어감!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        

        private async void LaserWarningAndAttack()
        {
            beamLength_ByScale.SetActive(true);
            Debug.Log("경고빔 활성화!");

            await LaserBeam();

            PerformAttack();    
        }
      
        private async UniTask LaserBeam()
        {
            while(elapsedTime < warningTime)
            {
                // 보스에서 플레이어 방향으로 레이캐스트 실행
                Vector3 directionToPlayer = (playerTransform.position - rayStartTransform.position).normalized;
                Physics.Raycast(rayStartTransform.position, directionToPlayer, out RaycastHit hit, 10000, LayerMask.GetMask("Ground"));
                beamHitDistance = hit.distance;

                // 경고 서클이 아직 활성화되지 않았다면 오브젝트 풀에서 가져옴
                if (!isWarningCircleOn)
                {
                    var particleEffect = ObjectPool.Instance.GetFromPool<ParticleSystem>("bossWarningCircle");
                    bossWarningCirele = particleEffect;
                    isWarningCircleOn = true;
                }
                // 경고 서클의 위치와 회전 설정 (지면에 맞춰 배치)
                bossWarningCirele.transform.position = hit.point + bossWarningCircle_PosCorrection;
                bossWarningCirele.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rayStartTransform.forward, hit.normal), hit.normal);

                await UniTask.Yield(); // 다음 프레임까지 대기
                elapsedTime += Time.deltaTime;
                
            }

            ObjectPool.Instance.ReturnToPool("bossWarningCircle", bossWarningCirele);

            beamLength_ByScale.SetActive(false);
            Debug.Log("경고빔 비활성화");
        }

        public override void Die()
        {
            base.Die();
            bossHPBar.planeDistance = -0.5f;
        }

        /*
        private async UniTask LaserBeam()
        {
            Vector3 lastTargetPosition = playerTransform.position; // 초기 타겟 위치 저장
            Vector3 lastDirection = playerTransform.position - rayStartObject.transform.position; // 초기 회전 저장

            while (elapsedTime < warningTime)
            {
                // 목표 방향 계산 (이전 프레임에서 일정 비율만 따라가도록 보간)
                Vector3 targetPosition = Vector3.Lerp(lastTargetPosition, playerTransform.position, Time.deltaTime * 0.8f);
                lastTargetPosition = targetPosition;

                Vector3 targetDirection = Vector3.Lerp(lastDirection, playerTransform.position - rayStartObject.transform.position, Time.deltaTime * 0.8f);
                lastDirection = targetDirection;

                Vector3 directionToTarget = (lastTargetPosition - rayStartTransform.position).normalized;
                

                // Raycast 실행
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

                    // 경고 서클 위치 보간
                    bossWarningCirele.transform.position = hit.point + bossWarningCircle_PosCorrection;
                    bossWarningCirele.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rayStartTransform.forward, hit.normal), hit.normal);

                }

                await UniTask.Yield(); // 다음 프레임까지 대기
                elapsedTime += Time.deltaTime;
            }

            ObjectPool.Instance.ReturnToPool("bossWarningCircle", bossWarningCirele);

            beamLength_ByScale.SetActive(false);
            Debug.Log("경고빔 비활성화");
        }
        */


        private void OnDrawGizmos()
        {
            if (rayStartTransform == null || playerTransform == null) return;

            // 레이 방향 계산
            Vector3 directionToPlayer = (playerTransform.position - rayStartTransform.position).normalized;

            // Raycast 실행 (충돌 검사)
            if (Physics.Raycast(rayStartTransform.position, directionToPlayer, out RaycastHit hit, 10000))
            {
                // 충돌된 지점까지 빨간색으로 선 표시
                Gizmos.color = Color.red;
                Gizmos.DrawLine(rayStartTransform.position, hit.point);

                // 충돌된 지점에 구 표시 (반지름 0.2)
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(hit.point, 0.2f);
            }
            else
            {
                // 충돌되지 않았을 경우, 최대 거리까지 녹색 선 표시
                Gizmos.color = Color.green;
                Gizmos.DrawLine(rayStartTransform.position, rayStartTransform.position + directionToPlayer * 10000);
            }
        }

    }
}
