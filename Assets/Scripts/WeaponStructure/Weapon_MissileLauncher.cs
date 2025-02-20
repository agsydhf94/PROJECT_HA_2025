using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class Weapon_MissileLauncher : Weapon
    {
        [Header("UI Target Indicator")]
        [SerializeField] private MissileTargetUI currentTargetIndicator; // 현재 활성화된 UI

        [Header("Targeting Settings")]
        [SerializeField] private List<TargetToStrike> validTargets = new List<TargetToStrike>();
        [SerializeField] private int currentTargetIndex = 0; // 현재 선택된 타겟
        public TargetToStrike target;
        public Queue<Missile_Projectile> missile_Projectiles = new Queue<Missile_Projectile>();

        /*
        private void Awake()
        {
            centerAimCircle = GameObject.FindGameObjectWithTag("MissileLauncherCenterAim");
        }
        */

        private void Update()
        {
            HandleTargetSwitch();

            

            UpdateTargetIndicatorUI();

            // 생성된 조준점이 계속 적을 따라다니게 만듬
            if (currentTargetIndicator != null && GetCurrentTarget() != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(GetCurrentTarget().transform.position);
                currentTargetIndicator.transform.position = screenPos;
            }
        }


        public override void Attack()
        {
            MissileLaunch();
        }

        private void MissileLaunch()
        {
            if (bulletTotal == 0 || currentBullet_inMagazine <= 0)
            {
                Debug.Log("미사일 탄약 부족!");
                return;
            }

            TargetToStrike target = GetCurrentTarget();
            StartCoroutine(FireMissile(target)); // 타겟이 있든 없든 미사일 발사
            currentBullet_inMagazine--;
        }

        private IEnumerator FireMissile(TargetToStrike target)
        {
            yield return new WaitForSecondsRealtime(0.2f);

            var missileInstance = MissilePrefab_Instantiate();
            Missile_Projectile missileScript = missileInstance.GetComponent<Missile_Projectile>();

            if (missileScript != null)
            {
                if (target != null)
                {
                    missileScript.SetTarget(target); // 유도 모드 (타겟이 있을 때)
                }
                else
                {
                    missileScript.SetStraightMode(firePosition.transform.forward); // 직선 모드 (타겟이 없을 때)
                }
            }
        }

        private Missile_Projectile MissilePrefab_Instantiate()
        {            
            var missilePrefab = ObjectPool.Instance.GetFromPool<Missile_Projectile>("MissilePrefab");
            missilePrefab.transform.position = firePosition.transform.TransformPoint(weaponSO.missileFire_Position);
            missilePrefab.transform.rotation = firePosition.transform.rotation * Quaternion.Euler(weaponSO.missileFire_Rotation);
            missilePrefab.transform.localScale = weaponSO.missileFire_Scale;

            return missilePrefab;
        }





        public void UpdateTargetList()
        {
            Debug.Log("UpdateTargetList 갱신중");

            List<TargetToStrike> detectedTargets = new List<TargetToStrike>();

            // 감지 시작점 설정 (카메라 위치 + 약간 앞)
            Vector3 rayStart = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            Vector3 rayDirection = Camera.main.transform.forward; // 시선 방향
            float maxDistance = weaponSO.targetingRange; // 감지 범위
            float sphereRadius = weaponSO.aimRadius; // 감지 반지름

            // CapsuleCastAll을 사용하여 감지
            RaycastHit[] hits = Physics.SphereCastAll(rayStart, sphereRadius, rayDirection, maxDistance, weaponSO.enemyLayer);

            Debug.Log($"현재 감지된 적 개수: {hits.Length}");


            // 스크린 공간에서의 감지 반지름 변환
            Vector3 worldRadiusPoint = rayStart + Camera.main.transform.right * sphereRadius;
            float screenRadius = Vector2.Distance(
                Camera.main.WorldToScreenPoint(rayStart),
                Camera.main.WorldToScreenPoint(worldRadiusPoint));


            foreach (RaycastHit hit in hits)
            {
                TargetToStrike target = hit.collider.GetComponent<TargetToStrike>();
                if (target != null)
                {
                    // 적의 월드 좌표를 화면 좌표로 변환
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

                    // 카메라 뒤쪽에 있는 경우 제외
                    if (screenPos.z < 0)
                    {
                        Debug.Log($"카메라 뒤쪽이므로 제외: {target.gameObject.name}");
                        continue;
                    }

                    // 화면 중앙에서 조준 원까지의 거리 계산
                    float distanceToCenter = Vector2.Distance(
                        new Vector2(Screen.width / 2, Screen.height / 2),
                        new Vector2(screenPos.x, screenPos.y)
                    );

                    Debug.Log($"적: {target.gameObject.name}, 거리: {distanceToCenter}, 감지 범위: {screenRadius}");

                    // 조준 원 안에 들어온 경우만 추가
                    if (distanceToCenter <= screenRadius)
                    {
                        Debug.Log($"감지됨: {target.gameObject.name}");
                        detectedTargets.Add(target);
                    }
                    else
                    {
                        Debug.Log($"감지 범위 밖: {target.gameObject.name}");
                    }
                }
            }

            // 타겟이 변경되지 않았을 경우 기존 타겟 유지
            if (detectedTargets.SequenceEqual(validTargets)) return;

            // 새로운 리스트 적용
            validTargets = detectedTargets;

            // 리스트가 비어있지 않으면 중앙과 가까운 적을 먼저 선택
            if (validTargets.Count > 0)
            {
                validTargets.Sort((a, b) => CompareTargets(a, b));
                currentTargetIndex = 0;
            }
        }

        // 마우스 스크롤을 이용해 타겟 변경
        private void HandleTargetSwitch()
        {
            if (validTargets.Count == 0) return;

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll > 0)
            {
                currentTargetIndex = (currentTargetIndex + 1) % validTargets.Count;
            }
            else if (scroll < 0)
            {
                currentTargetIndex = (currentTargetIndex - 1 + validTargets.Count) % validTargets.Count;
            }
        }

        // 중앙에 가까운 순서대로 정렬하는 비교 함수
        private int CompareTargets(TargetToStrike a, TargetToStrike b)
        {
            // 1. 중앙에서 화면 픽셀 거리 비교
            Vector3 screenPosA = Camera.main.WorldToScreenPoint(a.transform.position);
            Vector3 screenPosB = Camera.main.WorldToScreenPoint(b.transform.position);

            float distanceA = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), screenPosA);
            float distanceB = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), screenPosB);

            int result = distanceA.CompareTo(distanceB);

            // 2. 거리가 동일하다면 랜덤으로 선택
            if (result == 0)
            {
                return Random.Range(-1, 2);
            }

            return result;
        }

        // 현재 선택된 타겟을 반환
        public TargetToStrike GetCurrentTarget()
        {
            if (validTargets.Count == 0) return null;

            WeaponManager.Instance.targetToStrike = validTargets[currentTargetIndex];
            return validTargets[currentTargetIndex];
        }


        private void UpdateTargetIndicatorUI()
        {
            TargetToStrike target = GetCurrentTarget();

            if (target != null)
            {
                // 기존 UI가 없을 경우 오브젝트 풀에서 가져오기
                if (currentTargetIndicator == null)
                {
                    currentTargetIndicator = ObjectPool.Instance.GetFromPool<MissileTargetUI>("MissileTargetUI");
                    currentTargetIndicator.transform.SetParent(GameObject.FindGameObjectWithTag("MissileTargetUI").transform);
                }

                // 적의 위치를 UI 좌표로 변환
                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

                // UI 위치를 RectTransform 기준으로 변경
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)currentTargetIndicator.transform.parent,
                    screenPos, Camera.main, out Vector2 localPoint);

                currentTargetIndicator.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }
            else
            {
                if (currentTargetIndicator != null)
                {
                    ObjectPool.Instance.ReturnToPool("MissileTargetUI", currentTargetIndicator);
                    currentTargetIndicator = null;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (Camera.main == null) return;

            // 캡슐(원기둥) 형태의 감지 시작점과 끝점 계산
            Vector3 rayStart = Camera.main.transform.position;  // 카메라 위치
            Vector3 rayEnd = rayStart + Camera.main.transform.forward * weaponSO.targetingRange; // 시선 방향으로 연장
            float capsuleRadius = weaponSO.aimRadius;  // 감지 반지름

            // 감지 영역을 기즈모로 표시 (CapsuleCast)
            Gizmos.color = new Color(1, 0, 0, 0.3f); // 반투명 빨간색 (감지 영역)
            Gizmos.DrawSphere(rayStart, capsuleRadius);
            Gizmos.DrawSphere(rayEnd, capsuleRadius);
            Gizmos.DrawLine(rayStart + Vector3.up * capsuleRadius, rayEnd + Vector3.up * capsuleRadius);
            Gizmos.DrawLine(rayStart - Vector3.up * capsuleRadius, rayEnd - Vector3.up * capsuleRadius);
            Gizmos.DrawLine(rayStart + Vector3.right * capsuleRadius, rayEnd + Vector3.right * capsuleRadius);
            Gizmos.DrawLine(rayStart - Vector3.right * capsuleRadius, rayEnd - Vector3.right * capsuleRadius);

            // 감지된 적 위치 표시 (초록색)
            foreach (var target in validTargets)
            {
                if (target != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(target.transform.position, 0.5f);
                }
            }

            // 현재 선택된 타겟 강조 (파란색)
            TargetToStrike currentTarget = GetCurrentTarget();
            if (currentTarget != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(currentTarget.transform.position, 0.7f);
            }
        }
    }
}
