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

        // 적을 공격한 모든 플레이어를 추적
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
            targetBase = null; // 타겟 초기화
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
                        // 공격 범위에 도달한 경우 공격 준비
                        navMeshAgent.isStopped = true;
                        StartCoroutine(AttackCoroutine(targetBase, targetBase.GetComponent<Collider>()));
                    }
                    else
                    {
                        // 감지 범위 안에서 공격 범위 밖일 경우 계속 이동
                        if(distance <= playerDetectionRange)
                        {
                            navMeshAgent.isStopped = false;
                            navMeshAgent.SetDestination(targetBase.transform.position);
                        }
                        else // 감지 범위 밖까지 벗어나면 정지
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
            
            // 폭발로 인한 방향 벡터 계산 (폭발 중심에서 현재 위치로 향하는 벡터)
            Vector3 explosionDirection = (transform.position - explosionPosition).normalized;

            // 폭발로 인한 데미지 처리
            Damage(damage, transform.position, explosionDirection);

            // 폭발로 인한 추가 효과: 캐릭터가 날아가는 효과
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 폭발력과 방향을 적용하여 캐릭터가 날아가게 함
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

                // 몬스터를 공격한 플레이어들을 리스트에 추가
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

            // 공격한 모든 플레이어에게 경험치를 부여
            foreach (PlayerData attacker in attackers)
            {
                attacker.GetExp(exp_Gain / attackers.Count);
            }

            // 몬스터 킬 카운트 입력
            if (!QuestManager.enemyKilled_byPlayer.ContainsKey(enemyID))
            {
                QuestManager.enemyKilled_byPlayer.Add(enemyID, new EnemyKilledData_byPlayer());
            }
            //Debug.Log(QuestManager.enemyKilled_byPlayer[enemyID].totalAmount_soFar);
            QuestManager.enemyKilled_byPlayer[enemyID].totalAmount_soFar++;
            //Debug.Log(QuestManager.enemyKilled_byPlayer[enemyID].totalAmount_soFar);

            // 적이 죽었을 때 퀘스트 진행도 업데이트
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

                    // 공격 시작
                    StartCoroutine(AttackCoroutine(aimed_target, other));
                }
            }
        }

        private IEnumerator AttackCoroutine(CharacterBase __aimed_Target, Collider __other)
        {
            // 공격 준비 단계
            isAttacking = true;
            attackStart = false;
            isEnemyStunned = true;
            navMeshAgent.isStopped = true;
            enemyAnimator.SetBool("HasTarget", false);
            enemyAnimator.SetInteger("Condition", 2);

            // 공격 애니메이션을 재생하고 1.5초 대기
            yield return new WaitForSeconds(1.5f);

            // 실제로 공격하는 시점
            float distance = Vector3.Distance(transform.position, __aimed_Target.transform.position);
            Debug.Log(distance);
            Debug.Log(distance <= attackRange);


            if (__aimed_Target != null && !__aimed_Target.Dead && distance <= attackRange)
            {
                Vector3 hitPoint = __other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - __other.transform.position;

                __aimed_Target.Damage(damage, hitPoint, hitNormal);
            }

            // 공격 종료 처리
            isAttacking = false;
            isEnemyStunned = false;
            navMeshAgent.isStopped = false;
            enemyAnimator.SetBool("HasTarget", Target);
            enemyAnimator.SetInteger("Condition", 0);
        }

    }

    
}
