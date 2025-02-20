using HA;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HA
{
    public class Enemy : CharacterBase
    {
        public LayerMask targetLayerMask;

        private CharacterBase targetBase;
        private NavMeshAgent navMeshAgent;

        public ParticleSystem hitParticle;
        public AudioClip deathSound;
        public AudioClip hitSound;

        private int animConditionInteger;
        private bool isEnemyStunned;
        private bool isAttacking;
        public float attackRange = 1f;
        public float playerDetectionRange = 21f;
        private bool attackStart;
        private Animator enemyAnimator;
        [SerializeField]private AudioSource enemyAudioPlayer;
        private Renderer enemyRenderer;
        [SerializeField] private Canvas bossHPBar;


        public EnemySO enemySO;
        public int enemyID;
        public float damage = 20f;
        public float attackTerm = 0.5f;
        private float lastAttackTime;

        public float exp_Gain;

        // ���� ������ ��� �÷��̾ ����
        private List<PlayerData> attackers = new List<PlayerData>();

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, attackRange);
        }

        private bool Target
        {
            get
            {
                if (targetBase != null && !targetBase.Dead)
                {
                    return true;
                }
                return false;
            }
        }

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemyAnimator = GetComponent<Animator>();
            enemyAudioPlayer = GetComponent<AudioSource>();
            enemyRenderer = GetComponentInChildren<Renderer>();

            InitialSetup(enemySO);
        }

        public void InitialSetup(EnemySO enemySO)
        {
            initialHealth = enemySO.health;
            currentHP = enemySO.health;

            damage = enemySO.health;

            exp_Gain = enemySO.expGranted;

            navMeshAgent.speed = enemySO.speed;

            enemyRenderer.material.color = enemySO.skinColor;
        }

        private void Start()
        {
            StartCoroutine(UpdatePath());
        }

        private void Update()
        {
            if(!isEnemyStunned)
            {
                enemyAnimator.SetBool("HasTarget", Target);
                animConditionInteger = Target ? 1 : 0;
                enemyAnimator.SetInteger("Condition", animConditionInteger);
            }            
        }

        public void ResetTarget()
        {
            targetBase = null; // Ÿ�� �ʱ�ȭ
        }

        public IEnumerator UpdatePath()
        {
            while(!Dead)
            {
                if(Target)
                {
                    //navMeshAgent.isStopped = false;
                    //navMeshAgent.SetDestination(targetBase.transform.position);


                    float distance = Vector3.Distance(transform.position, targetBase.transform.position);

                    if (distance <= attackRange)
                    {
                        // ���� ������ ������ ��� ���� �غ�
                        navMeshAgent.isStopped = true;
                        StartCoroutine(AttackCoroutine(targetBase, targetBase.GetComponent<Collider>()));
                    }
                    else
                    {
                        // ���� ���� �ȿ��� ���� ���� ���� ��� ��� �̵�
                        if(distance <= playerDetectionRange)
                        {
                            navMeshAgent.isStopped = false;
                            navMeshAgent.SetDestination(targetBase.transform.position);
                        }
                        else // ���� ���� �۱��� ����� ����
                        {
                            navMeshAgent.isStopped = true;
                            targetBase = null;
                        }
                        
                    }


                }
                else
                {
                    navMeshAgent.isStopped = true;

                    Collider[] colliders = Physics.OverlapSphere(transform.position, playerDetectionRange, targetLayerMask);
                    for(int i = 0; i < colliders.Length; i++)
                    {
                        CharacterBase characterBase = colliders[i].GetComponent<CharacterBase>();

                        if(characterBase != null && !characterBase.Dead)
                        {
                            targetBase = characterBase;
                            break;
                        }
                    }
                }
                yield return new WaitForSeconds(0.7f);
            }
        }

        public void DamageByGrenade(float damage, Vector3 explosionPosition, float explosionForce)
        {
            
            // ���߷� ���� ���� ���� ��� (���� �߽ɿ��� ���� ��ġ�� ���ϴ� ����)
            Vector3 explosionDirection = (transform.position - explosionPosition).normalized;

            // ���߷� ���� ������ ó��
            Damage(damage, transform.position, explosionDirection);

            // ���߷� ���� �߰� ȿ��: ĳ���Ͱ� ���ư��� ȿ��
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ���߷°� ������ �����Ͽ� ĳ���Ͱ� ���ư��� ��
                rb.AddForce(explosionDirection * explosionForce, ForceMode.Impulse);
            }


            
        }

        public override void Damage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if(!Dead)
            {
                hitParticle.transform.position = hitPoint;
                hitParticle.transform.rotation = Quaternion.LookRotation(hitNormal);
                hitParticle.Play();

                enemyAnimator.SetInteger("Condition", 3);
                StartCoroutine(RecoverFromHit());

                enemyAudioPlayer.PlayOneShot(hitSound);

                // ���͸� ������ �÷��̾���� ����Ʈ�� �߰�
                PlayerData playerHealth = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>();
                if(!attackers.Contains(playerHealth))
                {
                    attackers.Add(playerHealth);
                }
            }
            
            base.Damage(damage, hitPoint, hitNormal);
        }

        public IEnumerator RecoverFromHit()
        {            
            isEnemyStunned = true;
            enemyAnimator.SetBool("HasTarget", false);
            navMeshAgent.isStopped = true;

            yield return new WaitForSeconds(0.8f);

            isEnemyStunned = false;
            enemyAnimator.SetBool("HasTarget", Target);
            enemyAnimator.SetInteger("Condition", 0);
            if(!Dead)
            {
                navMeshAgent.isStopped = false;
            }
            
        }

        public override void Die()
        {
            base.Die();
            

            Collider[] zombieColliders = GetComponents<Collider>();
            for(int i = 0; i < zombieColliders.Length; ++i)
            {
                zombieColliders[i].enabled = false;
            }

            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;

            GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().GetExp(exp_Gain);
            enemyAnimator.SetTrigger("Die");
            enemyAudioPlayer.PlayOneShot(deathSound);

            // ������ ��� �÷��̾�� ����ġ�� �ο�
            foreach (PlayerData attacker in attackers)
            {
                attacker.GetExp(exp_Gain / attackers.Count);
            }

            // ���� ų ī��Ʈ �Է�
            if (!QuestManager.enemyKilled_byPlayer.ContainsKey(enemyID))
            {
                QuestManager.enemyKilled_byPlayer.Add(enemyID, new EnemyKilledData_byPlayer());
            }
            //Debug.Log(QuestManager.enemyKilled_byPlayer[enemyID].totalAmount_soFar);
            QuestManager.enemyKilled_byPlayer[enemyID].totalAmount_soFar++;
            //Debug.Log(QuestManager.enemyKilled_byPlayer[enemyID].totalAmount_soFar);

            // ���� �׾��� �� ����Ʈ ���൵ ������Ʈ
            foreach (int questID in QuestManager.activeQuests.Keys)
            {
                QuestManager.Instance.UpdateQuestProgress_Main(questID);
            }

            
            
        }



        private void OnTriggerStay(Collider other)
        {
            if(!Dead && Time.time >= lastAttackTime + attackTerm)
            {
                CharacterBase aimed_target = other.GetComponent<CharacterBase>();

                if(aimed_target != null && aimed_target == targetBase && !isAttacking)
                {
                    lastAttackTime = Time.time;

                    // ���� ����
                    StartCoroutine(AttackCoroutine(aimed_target, other));
                }
            }
        }

        private IEnumerator AttackCoroutine(CharacterBase __aimed_Target, Collider __other)
        {
            // ���� �غ� �ܰ�
            isAttacking = true;
            attackStart = false;
            isEnemyStunned = true;
            navMeshAgent.isStopped = true;
            enemyAnimator.SetBool("HasTarget", false);
            enemyAnimator.SetInteger("Condition", 2);

            // ���� �ִϸ��̼��� ����ϰ� 1.5�� ���
            yield return new WaitForSeconds(1.5f);

            // ������ �����ϴ� ����
            float distance = Vector3.Distance(transform.position, __aimed_Target.transform.position);
            Debug.Log(distance);
            Debug.Log(distance <= attackRange);


            if (__aimed_Target != null && !__aimed_Target.Dead && distance <= attackRange)
            {
                Vector3 hitPoint = __other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - __other.transform.position;

                __aimed_Target.Damage(damage, hitPoint, hitNormal);
            }

            // ���� ���� ó��
            isAttacking = false;
            isEnemyStunned = false;
            navMeshAgent.isStopped = false;
            enemyAnimator.SetBool("HasTarget", Target);
            enemyAnimator.SetInteger("Condition", 0);
        }

    }

    
}
