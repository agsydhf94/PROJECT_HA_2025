using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;

namespace HA
{
    public class CharacterBase : MonoBehaviourPun, IDamagable
    {
        public float initialHealth;
        public float initialMp;

        public float currentHP;
        public float maxHP;

        public float currentMP;
        public float maxMP;


        public bool Dead { get; protected set; }
        public event Action OnDeath;

        public delegate void OnDamage(float currentHP, float maxHP);
        public OnDamage onDamageCallback;

        public System.Action<float, float> OnDamaged;
        public System.Action<float, float> OnChangedHP;
        public System.Action<float, float> OnChangedMP;

        private Renderer[] characterRenderers;

        private void Awake()
        {
            characterRenderers = GetComponentsInChildren<Renderer>();
        }

        // 생명체의 상태를 리셋
        protected virtual void OnEnable()
        {
            Dead = false;
            currentHP = initialHealth;
        }

        [PunRPC]
        public virtual void Damage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if(!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
            {
                currentHP -= damage;

                OnDamaged?.Invoke(currentHP, maxHP);

                if(PhotonNetwork.IsConnected)
                {
                    photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, currentHP, Dead);

                    photonView.RPC("Damage", RpcTarget.Others, damage, hitPoint, hitNormal);
                }
                

            }
            

            if (currentHP <= 0 && !Dead)
            {
                Die();
            }
        }


        [PunRPC]
        public virtual void IncreaseHP(float amount)
        {
            if(Dead)
            {
                return;
            }

            // 온라인 멀티플레이어 시, 클라이언트만 체력 회복 가능
            if(!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
            {
                currentHP += amount;

                currentHP = Mathf.Clamp(currentHP, 0, maxHP);

                OnChangedHP?.Invoke(currentHP, maxHP);

                // 서버 클라이언트로 정보 동기화
                photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, currentHP, Dead);

                // 다른 클라이언트에 있는 로컬의 리모트들도 체력 회복
                photonView.RPC("IncreaseHP", RpcTarget.Others, amount);
            }
            
            
        }

        [PunRPC]
        public virtual void IncreaseMP(float amount)
        {
            if (Dead)
            {
                return;
            }

            // 온라인 멀티플레이어 시, 클라이언트만 체력 회복 가능
            if (!PhotonNetwork.IsConnected || PhotonNetwork.IsMasterClient)
            {
                currentMP += amount;

                currentMP = Mathf.Clamp(currentMP, 0, maxMP);

                OnChangedMP?.Invoke(currentMP, maxMP);

                // 서버 클라이언트로 정보 동기화
                photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, currentMP, Dead);

                // 다른 클라이언트에 있는 로컬의 리모트들도 체력 회복
                photonView.RPC("IncreaseMP", RpcTarget.Others, amount);
            }


        }

        [PunRPC]
        public void ApplyUpdateHealth(float newHealth, float newMana, bool newDead)
        {
            currentHP = newHealth;
            currentMP = newMana;
            Dead = newDead;
        }

        public virtual void Die()
        {
            if(OnDeath != null)
            {
                OnDeath();
            }

            Dead = true;
        }

        

        
    }
    
    public enum CharacterMode
    {
        Shooter,
        Kunoichi
    }

}
