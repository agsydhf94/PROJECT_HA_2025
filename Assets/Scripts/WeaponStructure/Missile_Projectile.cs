using System.Collections;
using UnityEngine;


namespace HA
{
    public class Missile_Projectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private GameObject explosionPrefab;
        public float damagePoint;

        [Header("Moving Properties")]
        [SerializeField] private float missileSpeed = 100;
        [SerializeField] private float acceleration = 50; // 가속도 추가
        [SerializeField] private float rotateSpeed = 1500;
        [SerializeField] private TargetToStrike targetToStrike;
        [SerializeField] private bool isStraightMode;
        [SerializeField] private Vector3 straightDirection;

        [Header("Target Prediction")]
        [SerializeField] private float maxDistancePredict = 100;
        [SerializeField] private float minDistancePredict = 5;
        [SerializeField] private float maxTimePrediction = 5;
        private Vector3 standardPrediction, deviatedPrediction;

        [Header("Trajectory Deviation")]
        [SerializeField] private float deviationAmount = 10;
        [SerializeField] private float deviationSpeed = 2;


        // 유도 미사일 모드: 타겟 설정
        public void SetTarget(TargetToStrike newTarget)
        {
            targetToStrike = newTarget;
            isStraightMode = false;
        }

        // 직선 모드: 특정 방향으로 발사
        public void SetStraightMode(Vector3 direction)
        {
            isStraightMode = true;
            straightDirection = direction.normalized; // 방향을 정규화
        }

        private void FixedUpdate()
        {
            missileSpeed += acceleration * Time.deltaTime;

            rb.velocity = transform.forward * missileSpeed;

            if (!isStraightMode)
            {
                var target = WeaponManager.Instance.targetToStrike;
                var leadTimePercentage = Mathf.InverseLerp(minDistancePredict, maxDistancePredict, Vector3.Distance(transform.position, target.transform.position));

                PredictMovement(leadTimePercentage);

                AddDeviation(leadTimePercentage);

                AdjustSpeedBasedOnRotation(); // 방향 변경 시 감속 추가

                RotateRocket();
            }
            
        }

        private void PredictMovement(float leadTimePercentage)
        {
            var predictionTime = Mathf.Lerp(0, maxTimePrediction, leadTimePercentage);
            var target = WeaponManager.Instance.targetToStrike;

            // 목표 방향과 현재 위치를 보정하여 더 빠르게 목표 근처로 향하도록 조정
            Vector3 futurePosition = target.Rb.position + target.Rb.velocity * predictionTime;
            standardPrediction = Vector3.Lerp(target.Rb.position, futurePosition, 0.7f); // 기존: 1.0 → 0.7
        }

        private void AddDeviation(float leadTimePercentage)
        {
            var deviation = new Vector3(Mathf.Cos(Time.time * deviationSpeed), 0, 0);

            var predictionOffset = transform.TransformDirection(deviation) * deviationAmount * leadTimePercentage;

            deviatedPrediction = standardPrediction + predictionOffset;
        }

        private void AdjustSpeedBasedOnRotation()
        {
            var heading = deviatedPrediction - transform.position; // 목표 방향
            float angleDifference = Vector3.Angle(transform.forward, heading); // 현재 방향과 목표 방향의 각도 차이 계산

            float rotationFactor = Mathf.Clamp01(angleDifference / 90f); // 최대 90도까지 감속 영향 (0~1 정규화)
            float adjustedSpeed = Mathf.Lerp(missileSpeed * 0.6f, missileSpeed, 1 - rotationFactor); // 회전량이 클수록 감속

            rb.velocity = transform.forward * adjustedSpeed; // 감속 적용
        }

        private void RotateRocket()
        {
            var heading = deviatedPrediction - transform.position;

            var rotation = Quaternion.LookRotation(heading);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime * 2.5f));
        }

        private void OnCollisionEnter(Collision collision)
        {
            /*
            if (explosionPrefab) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            {
                (collision.transform.TryGetComponent<TargetToStrike>(out var ex)) ex.Explode();
            }
            
            Destroy(gameObject);
            */

            if (collision.transform.TryGetComponent<TargetToStrike>(out targetToStrike))
            {
                var missileImpactParticle = ObjectPool.Instance.GetFromPool<ParticleSystem>("MissileImpactParticle");
                missileImpactParticle.transform.position = transform.position;

                if(collision.transform.TryGetComponent<CharacterBase>(out CharacterBase characterBase))
                {
                    characterBase.Damage(damagePoint, transform.position, transform.position);
                }
                StartCoroutine(ParticleEffect_ReturnToPool(missileImpactParticle));
                ObjectPool.Instance.ReturnToPool("MissilePrefab", this);
            }
        }

        private IEnumerator ParticleEffect_ReturnToPool(ParticleSystem effect)
        {
            yield return new WaitForSecondsRealtime(5.5f);

            ObjectPool.Instance.ReturnToPool("MissileImpactParticle", effect);
            Debug.Log("미사일 충돌 파티클 효과 복귀 완료");
        }

        

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, standardPrediction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(standardPrediction, deviatedPrediction);
        }
    }
}
