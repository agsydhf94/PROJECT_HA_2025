using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Weapon_Gun : Weapon
    {

        public override void Attack()
        {
            Shoot();
        }

        public void Shoot()
        {

            if (weaponSO.isSingleShot)
            {
                if (canShoot)
                {
                    AutoGunShooting();
                    canShoot = false; // �߻� �Ŀ��� �÷��׸� false�� ����
                }
            }
            else
            {
                AutoGunShooting(); // ���� ��忡���� ���� ���� ���� �״�� ���
            }


        }

        public void AutoGunShooting()
        {
            if (Time.time > lastShootTime + weaponSO.fireRate)
            {
                // ���� ����
                lastShootTime = Time.time;

                MotionBlurController.Instance.TriggerMotionBlur();

                PlaySE(weaponSO.fire_Sound);
                crosshair.FireAnimation();
                currentBullet_inMagazine--;

                // Muzzle ����Ʈ �����
                var newMuzzle = Instantiate(weaponSO.muzzlePrefab);                
                newMuzzle.transform.SetParent(firePosition.transform);
                newMuzzle.transform.localPosition = weaponSO.fireTransform_Position;
                newMuzzle.transform.localRotation = Quaternion.Euler(weaponSO.fireTransform_Rotation);
                newMuzzle.transform.localScale = weaponSO.fireTransform_Scale;
                newMuzzle.gameObject.SetActive(true);
                Destroy(newMuzzle, 0.12f);




                Quaternion randomQua = new Quaternion(Random.Range(0, 360.0f), Random.Range(0, 360.0f), Random.Range(0, 360.0f), 1);
                GameObject bulletCasing = Instantiate(weaponSO.bulletCasingPrefab);
                bulletCasing.transform.localRotation = randomQua;
                bulletCasing.transform.localPosition = weaponSO.bulletCasingTransform_Position;
                bulletCasing.GetComponent<Rigidbody>().AddRelativeForce(
                    new Vector3(Random.Range(50.0f, 100.0f), Random.Range(50.0f, 100.0f), Random.Range(-40.0f, 40.0f)));
                Destroy(bulletCasing, 1.0f);




                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC("ShootingInfoOnServer", RpcTarget.MasterClient);
                }
                else
                {
                    ShootingInfoOnServer();
                }

            }
        }

        [PunRPC]
        private void ShootingInfoOnServer()
        {


            Shooting_Raycast();


            // �ѱ� �ݵ� ����
            Vector3 velocity = recoilShakePattern[currentRecoilIndex];
            currentRecoilIndex++;
            if (currentRecoilIndex >= recoilShakePattern.Count)
            {
                currentRecoilIndex = currentRecoilIndex = 0;
            }
            CameraSystem.Instance.ShakeCamera(velocity, 0.2f, 1f);
        }



        private void Shooting_Raycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, weaponSO.shootingRaycastMaxRange))
            {
                var trail = Instantiate(weaponSO.bulletTrailRenderer);
                trail.time = -1.0f; // �̷��� �ϸ� ������ ������ ���� �� ����

                if (trail != null)
                {
                    if (trail.time != 1.0f)
                    {
                        trail.time = 1.0f; // �Ѿ��� �����̱� ������ ������ ������ �����Ѵ�.
                    }
                }

                trail.transform.SetParent(firePosition.transform);
                trail.transform.localPosition = weaponSO.fireTransform_Position;
                trail.transform.localRotation = Quaternion.Euler(weaponSO.fireTransform_Rotation);
                trail.transform.localScale = weaponSO.fireTransform_Scale;
                trail.transform.SetParent(null);

                if (trail != null)
                {
                    Debug.Log("Ʈ���� ������");
                }
                StartCoroutine(SpawnTrail(trail, hit));



                IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.Damage(weaponSO.damagePoint, hit.point, hit.normal);
                }


                // ���͸��� ���� ����Ʈ ǥ��
                string physicMaterialName = hit.collider.material.name;
                if (physicMaterialName.Contains("Metal"))
                {
                    Instantiate(metalImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }
                else if (physicMaterialName.Contains("Wood"))
                {
                    Instantiate(woodImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
            else
            {
                // �ƹ��͵� ���� ������ �� ������ range �Ÿ��� ����
                hit.point = ray.origin + ray.direction * weaponSO.shootingRaycastMaxRange;

            }


        }


        public IEnumerator SpawnTrail(TrailRenderer trailObject, RaycastHit hit)
        {
            float timer = 0f;
            Vector3 startPosition = trailObject.transform.position;
            Debug.Log(startPosition);

            while (timer <= 1f)
            {
                yield return new WaitForSeconds(0.01f);

                trailObject.transform.position = Vector3.Lerp(startPosition, hit.point, timer);
                timer += Time.deltaTime / weaponSO.bulletTrail_SpeedMultiplier;
            }

            trailObject.transform.position = hit.point;
            var hitParticle = Instantiate(weaponSO.bulletImpactParticle, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(trailObject.gameObject);

            yield return new WaitForSecondsRealtime(0.5f);
            ObjectPool.Instance.ReturnToPool(weaponSO.bulletImpactParticle.name, hitParticle);
        }
    }
}
