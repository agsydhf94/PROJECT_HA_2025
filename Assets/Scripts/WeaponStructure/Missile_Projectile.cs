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
        [SerializeField] private float acceleration = 50; // ���ӵ� �߰�
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


        // ���� �̻��� ���: Ÿ�� ����
        public void SetTarget(TargetToStrike newTarget)
        {
            targetToStrike = newTarget;
            isStraightMode = false;
        }

        // ���� ���: Ư�� �������� �߻�
        public void SetStraightMode(Vector3 direction)
        {
            isStraightMode = true;
            straightDirection = direction.normalized; // ������ ����ȭ
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

                AdjustSpeedBasedOnRotation(); // ���� ���� �� ���� �߰�

                RotateRocket();
            }
            
        }

        private void PredictMovement(float leadTimePercentage)
        {
            var predictionTime = Mathf.Lerp(0, maxTimePrediction, leadTimePercentage);
            var target = WeaponManager.Instance.targetToStrike;

            // ��ǥ ����� ���� ��ġ�� �����Ͽ� �� ������ ��ǥ ��ó�� ���ϵ��� ����
            Vector3 futurePosition = target.Rb.position + target.Rb.velocity * predictionTime;
            standardPrediction = Vector3.Lerp(target.Rb.position, futurePosition, 0.7f); // ����: 1.0 �� 0.7
        }

        private void AddDeviation(float leadTimePercentage)
        {
            var deviation = new Vector3(Mathf.Cos(Time.time * deviationSpeed), 0, 0);

            var predictionOffset = transform.TransformDirection(deviation) * deviationAmount * leadTimePercentage;

            deviatedPrediction = standardPrediction + predictionOffset;
        }

        private void AdjustSpeedBasedOnRotation()
        {
            var heading = deviatedPrediction - transform.position; // ��ǥ ����
            float angleDifference = Vector3.Angle(transform.forward, heading); // ���� ����� ��ǥ ������ ���� ���� ���

            float rotationFactor = Mathf.Clamp01(angleDifference / 90f); // �ִ� 90������ ���� ���� (0~1 ����ȭ)
            float adjustedSpeed = Mathf.Lerp(missileSpeed * 0.6f, missileSpeed, 1 - rotationFactor); // ȸ������ Ŭ���� ����

            rb.velocity = transform.forward * adjustedSpeed; // ���� ����
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
            Debug.Log("�̻��� �浹 ��ƼŬ ȿ�� ���� �Ϸ�");
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
