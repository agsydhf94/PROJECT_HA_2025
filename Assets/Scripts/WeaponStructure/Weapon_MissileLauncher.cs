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
        [SerializeField] private MissileTargetUI currentTargetIndicator; // ���� Ȱ��ȭ�� UI

        [Header("Targeting Settings")]
        [SerializeField] private List<TargetToStrike> validTargets = new List<TargetToStrike>();
        [SerializeField] private int currentTargetIndex = 0; // ���� ���õ� Ÿ��
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

            // ������ �������� ��� ���� ����ٴϰ� ����
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
                Debug.Log("�̻��� ź�� ����!");
                return;
            }

            TargetToStrike target = GetCurrentTarget();
            StartCoroutine(FireMissile(target)); // Ÿ���� �ֵ� ���� �̻��� �߻�
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
                    missileScript.SetTarget(target); // ���� ��� (Ÿ���� ���� ��)
                }
                else
                {
                    missileScript.SetStraightMode(firePosition.transform.forward); // ���� ��� (Ÿ���� ���� ��)
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
            Debug.Log("UpdateTargetList ������");

            List<TargetToStrike> detectedTargets = new List<TargetToStrike>();

            // ���� ������ ���� (ī�޶� ��ġ + �ణ ��)
            Vector3 rayStart = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            Vector3 rayDirection = Camera.main.transform.forward; // �ü� ����
            float maxDistance = weaponSO.targetingRange; // ���� ����
            float sphereRadius = weaponSO.aimRadius; // ���� ������

            // CapsuleCastAll�� ����Ͽ� ����
            RaycastHit[] hits = Physics.SphereCastAll(rayStart, sphereRadius, rayDirection, maxDistance, weaponSO.enemyLayer);

            Debug.Log($"���� ������ �� ����: {hits.Length}");


            // ��ũ�� ���������� ���� ������ ��ȯ
            Vector3 worldRadiusPoint = rayStart + Camera.main.transform.right * sphereRadius;
            float screenRadius = Vector2.Distance(
                Camera.main.WorldToScreenPoint(rayStart),
                Camera.main.WorldToScreenPoint(worldRadiusPoint));


            foreach (RaycastHit hit in hits)
            {
                TargetToStrike target = hit.collider.GetComponent<TargetToStrike>();
                if (target != null)
                {
                    // ���� ���� ��ǥ�� ȭ�� ��ǥ�� ��ȯ
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

                    // ī�޶� ���ʿ� �ִ� ��� ����
                    if (screenPos.z < 0)
                    {
                        Debug.Log($"ī�޶� �����̹Ƿ� ����: {target.gameObject.name}");
                        continue;
                    }

                    // ȭ�� �߾ӿ��� ���� �������� �Ÿ� ���
                    float distanceToCenter = Vector2.Distance(
                        new Vector2(Screen.width / 2, Screen.height / 2),
                        new Vector2(screenPos.x, screenPos.y)
                    );

                    Debug.Log($"��: {target.gameObject.name}, �Ÿ�: {distanceToCenter}, ���� ����: {screenRadius}");

                    // ���� �� �ȿ� ���� ��츸 �߰�
                    if (distanceToCenter <= screenRadius)
                    {
                        Debug.Log($"������: {target.gameObject.name}");
                        detectedTargets.Add(target);
                    }
                    else
                    {
                        Debug.Log($"���� ���� ��: {target.gameObject.name}");
                    }
                }
            }

            // Ÿ���� ������� �ʾ��� ��� ���� Ÿ�� ����
            if (detectedTargets.SequenceEqual(validTargets)) return;

            // ���ο� ����Ʈ ����
            validTargets = detectedTargets;

            // ����Ʈ�� ������� ������ �߾Ӱ� ����� ���� ���� ����
            if (validTargets.Count > 0)
            {
                validTargets.Sort((a, b) => CompareTargets(a, b));
                currentTargetIndex = 0;
            }
        }

        // ���콺 ��ũ���� �̿��� Ÿ�� ����
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

        // �߾ӿ� ����� ������� �����ϴ� �� �Լ�
        private int CompareTargets(TargetToStrike a, TargetToStrike b)
        {
            // 1. �߾ӿ��� ȭ�� �ȼ� �Ÿ� ��
            Vector3 screenPosA = Camera.main.WorldToScreenPoint(a.transform.position);
            Vector3 screenPosB = Camera.main.WorldToScreenPoint(b.transform.position);

            float distanceA = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), screenPosA);
            float distanceB = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), screenPosB);

            int result = distanceA.CompareTo(distanceB);

            // 2. �Ÿ��� �����ϴٸ� �������� ����
            if (result == 0)
            {
                return Random.Range(-1, 2);
            }

            return result;
        }

        // ���� ���õ� Ÿ���� ��ȯ
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
                // ���� UI�� ���� ��� ������Ʈ Ǯ���� ��������
                if (currentTargetIndicator == null)
                {
                    currentTargetIndicator = ObjectPool.Instance.GetFromPool<MissileTargetUI>("MissileTargetUI");
                    currentTargetIndicator.transform.SetParent(GameObject.FindGameObjectWithTag("MissileTargetUI").transform);
                }

                // ���� ��ġ�� UI ��ǥ�� ��ȯ
                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

                // UI ��ġ�� RectTransform �������� ����
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

            // ĸ��(�����) ������ ���� �������� ���� ���
            Vector3 rayStart = Camera.main.transform.position;  // ī�޶� ��ġ
            Vector3 rayEnd = rayStart + Camera.main.transform.forward * weaponSO.targetingRange; // �ü� �������� ����
            float capsuleRadius = weaponSO.aimRadius;  // ���� ������

            // ���� ������ ������ ǥ�� (CapsuleCast)
            Gizmos.color = new Color(1, 0, 0, 0.3f); // ������ ������ (���� ����)
            Gizmos.DrawSphere(rayStart, capsuleRadius);
            Gizmos.DrawSphere(rayEnd, capsuleRadius);
            Gizmos.DrawLine(rayStart + Vector3.up * capsuleRadius, rayEnd + Vector3.up * capsuleRadius);
            Gizmos.DrawLine(rayStart - Vector3.up * capsuleRadius, rayEnd - Vector3.up * capsuleRadius);
            Gizmos.DrawLine(rayStart + Vector3.right * capsuleRadius, rayEnd + Vector3.right * capsuleRadius);
            Gizmos.DrawLine(rayStart - Vector3.right * capsuleRadius, rayEnd - Vector3.right * capsuleRadius);

            // ������ �� ��ġ ǥ�� (�ʷϻ�)
            foreach (var target in validTargets)
            {
                if (target != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(target.transform.position, 0.5f);
                }
            }

            // ���� ���õ� Ÿ�� ���� (�Ķ���)
            TargetToStrike currentTarget = GetCurrentTarget();
            if (currentTarget != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(currentTarget.transform.position, 0.7f);
            }
        }
    }
}
