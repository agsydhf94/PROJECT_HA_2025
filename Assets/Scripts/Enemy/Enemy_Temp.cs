using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Temp : CharacterBase, IDamagable
{
    //public float enemyHp;
    public bool isDead = false;

    public GameObject projectile;
    public Transform projectilePoint;
    public float bulletPower = 300f;

    public RagdollController ragdollController;

    private void Awake()
    {
        ragdollController = GetComponent<RagdollController>();
        //enemyHp = currentHP;
    }

    private void Update()
    {
        if (currentHP > 0)
        {
            isDead = false;
        }
        else
        {
            isDead = true;
            ragdollController.ActiveRagdoll();
            // gameObject.SetActive(!isDead);
        }
    }

    public void Shoot()
    {
        Rigidbody rb = Instantiate(projectile, projectilePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletPower, ForceMode.Impulse);
    }


    public override void Damage(float damagePoint, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(isDead)
        {
            // 적이 죽었을 때 래그돌 활성화 및 힘 적용
            ragdollController.ForceActiveRagdollWithPower(HumanBodyBones.Spine, hitNormal, 50f);  // 스파인에 힘을 가함
        }
        base.Damage(damagePoint, hitPoint, hitNormal);
    }

    /*
    public override void Damage(float damagePoint, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!isDead)
        {
            enemyHp -= damagePoint;
        }
        else
        {
            // 적이 죽었을 때 래그돌 활성화 및 힘 적용
            ragdollController.ForceActiveRagdollWithPower(HumanBodyBones.Spine, hitNormal, 50f);  // 스파인에 힘을 가함
        }
    }
    */


}
