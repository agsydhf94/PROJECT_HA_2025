using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace HA
{
    public class SkillManager : Singleton<SkillManager>
    {
        public GameManager gameManager;


        public Dictionary<string, SkillSlot> currentActivateSkills = new Dictionary<string, SkillSlot>();
        public GameObject[] activeSkillSlot;
        public SkillSlot[] skillSlots;
        public bool isSkillUsing;
        public TMP_Text selected_SkillDescription;

        [Header("Skill VFX Prefabs - Asuna")]
        public ParticleSystem asunaSkill_A_VFX;
        public ParticleSystem asunaSkill_B1_VFX;
        public ParticleSystem asunaSkill_B2_VFX;
        public ParticleSystem asunaSkill_B3_VFX;
        public ParticleSystem asunaSkill_C1_VFX;
        public ParticleSystem asunaSkill_C2_VFX;

        [Header("Initial Transform")]
        public Transform inintialTransform;

        [Header("Asuna SkillUI")]
        public GameObject asunaSkillTree_UI;
        public SkillSlot[] asunaSkillSlots_UI;

        [Header("Kunoichi Skill Slot")]
        public GameObject kunoichiSkillTree_UI;
        public SkillSlot[] kunoichiSkillSlots_UI;



        private void Awake()
        {
            SkillVFX_ObjectPool_Initiate();
        }

        public SkillSlot[] SkillSlotUIInitialize(CharacterMode characterMode)
        {
            if(characterMode == CharacterMode.Shooter)
            {
                return asunaSkillSlots_UI;
            }
            else if(characterMode == CharacterMode.Kunoichi)
            {
                return kunoichiSkillSlots_UI;
            }

            return null;
        }

        public GameObject GetSkillTreeUI(CharacterMode characterMode)
        {
            if (characterMode == CharacterMode.Shooter)
            {
                return asunaSkillTree_UI;
            }
            else if (characterMode == CharacterMode.Kunoichi)
            {
                return kunoichiSkillTree_UI;
            }

            return null;
        }


        public void DeselectAllSkillSlots()
        {
            //skillSlots = GameManager.Instance.skillSlots;
            skillSlots = SkillSlotUIInitialize(GameManager.Instance.currentCharacterMode);

            for (int i = 0; i < skillSlots.Length; i++)
            {
                skillSlots[i].isThisSlotSelected = false;
                skillSlots[i].skillSelectedImage.SetActive(false);
                selected_SkillDescription.text = "";
            }
        }

        public string Detect_SelectedSlot()
        {
            string output = "";

            for(int i = 0; i < skillSlots.Length; i++)
            {
                if (skillSlots[i].isThisSlotSelected)
                {
                    output = skillSlots[i].skillSO.skillDescription;
                    break;
                }
                else
                {
                    output = "";
                }
            }
            return output;
        }

        public bool SkillInUseCheck()
        {
            bool skillinUse = false;

            for(int i = 0; i < activeSkillSlot.Length; i++)
            {
                // for ���鼭 �� ��ų�� ��� �Ϸ�Ǿ����� ���ٵ�
                // ��� �Ϸ��� ������
                // ��Ÿ���� �� á�°�? �� �ƴ϶� ������ �Ϸ�Ǿ���? �� �� ����
            }

            return skillinUse;
        }

        public int SkillPointCheck()
        {
            int currentSkillPoint = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().skillPoint;

            return currentSkillPoint;
        }

        public void SkillExecute(int index)
        {
            var currentCharacter = GameManager.Instance.currentActiveCharacter;
            currentCharacter.GetComponentInChildren<Animator>().SetTrigger(activeSkillSlot[index].GetComponentInChildren<SkillSlot>().skillSO.skillName);

            inintialTransform = GameManager.Instance.currentActiveCharacter.gameObject.transform;
            StartCoroutine(SkillCoolTime(activeSkillSlot[index].GetComponentInChildren<SkillSlot>()));
            activeSkillSlot[index].GetComponentInChildren<ISkillState>().ExecuteSkill();
        }

        public IEnumerator SkillCoolTime(SkillSlot skillSlot)
        {
            float progress = 0f;
            float coolTime = skillSlot.coolTime;
            float countdown = skillSlot.coolTime;
            skillSlot.cooltimeCountdown.gameObject.SetActive(true);
            skillSlot.isCooled = false;
            skillSlot.GetComponent<Image>().color = skillSlot.cooltimeColor;

            while(progress <= 1)
            {
                skillSlot.coolTimeImage.fillAmount = Mathf.Lerp(1, 0, progress);
                progress += Time.deltaTime / coolTime;  // 0�� 1 ������ ������ ����ȭ �ϴ� ���� i.e. ũ��� ������
                
                countdown -= Time.deltaTime;
                skillSlot.cooltimeCountdown.text = countdown.ToString("F3");
                yield return null;
            }
            skillSlot.isCooled = true;            
            skillSlot.GetComponent<Image>().color = skillSlot.characterColor;
            skillSlot.cooltimeCountdown.gameObject.SetActive(false);            
            Debug.Log(skillSlot.isCooled);
            
        }

        // Ȱ��ȭ�� ��ų ������ ���ֱ�
        // �̸� Ȱ��ȭ�� â�� �ִ� �� ���ٴ� 
        // ���� ���� "No Active Skills" ����
        // ��ų Ȱ���� ������ UI������ �ν��Ͻ�ȭ

        public void CurrentActivatedSkill_DataTransfer(SkillSlot skillSlot)
        {                     
            for(int i = 0; i < activeSkillSlot.Length; i++)
            {
                Debug.Log(activeSkillSlot[i].name);
                if (activeSkillSlot[i].transform.childCount == 0)
                {
                    var activatedSkillSlot = Instantiate(skillSlot);
                    activatedSkillSlot.transform.SetParent(activeSkillSlot[i].transform);
                    activatedSkillSlot.skillActivatedImage.SetActive(false);

                    RectTransform rectTransform = activatedSkillSlot.GetComponent<RectTransform>();
                    rectTransform.localPosition = new Vector3(0, 0, 0);
                    rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    rectTransform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    Debug.Log(activatedSkillSlot.skillName.text);
                    currentActivateSkills.Add(activatedSkillSlot.skillName.text, activatedSkillSlot);

                    break;
                }
            }
            
            // SetParent �� ���ش���, RectTransfrom ���� (���� �߿�)
        }

        public void CurrentActivatedSkill_DataRemove(SkillSlot skillSlot)
        {
            string skillNameToRemove = skillSlot.skillName.text;

            if(currentActivateSkills.ContainsKey(skillNameToRemove))
            {
                // Destroy ��, key�� �ش��ϴ� ������Ʈ.gameObject
                Destroy(currentActivateSkills[skillNameToRemove].gameObject);
                currentActivateSkills.Remove(skillNameToRemove);
            }
        }

        public void SkillVFX_ObjectPool_Initiate()
        {
            ObjectPool.Instance.CreatePool<ParticleSystem>("Asuna_SkillA", asunaSkill_A_VFX, 3);

            ObjectPool.Instance.CreatePool<ParticleSystem>("Asuna_SkillB1", asunaSkill_B1_VFX, 3);
            ObjectPool.Instance.CreatePool<ParticleSystem>("Asuna_SkillB2", asunaSkill_B2_VFX, 3);
            ObjectPool.Instance.CreatePool<ParticleSystem>("Asuna_SkillB3", asunaSkill_B3_VFX, 3);

            ObjectPool.Instance.CreatePool<ParticleSystem>("Asuna_SkillC1", asunaSkill_C1_VFX, 3);
            ObjectPool.Instance.CreatePool<ParticleSystem>("Asuna_SkillC2", asunaSkill_C2_VFX, 3);
        }

        public List<float> VFX_PlayTimeDelayCalculator(float[] animationTime, float[] vfxRT_ByAction)
        {
            List<float> vfxDelayTime = new List<float>();

            for(int i = 0; i < animationTime.Length; i++)
            {
                if (animationTime.Length == 1)
                {
                    float c = animationTime[0] - vfxRT_ByAction[0];
                    vfxDelayTime.Add(c);
                }
                else
                {
                    float c = (animationTime[i] - vfxRT_ByAction[i]) - animationTime[i-1];
                    vfxDelayTime.Add(c);
                }

            }

            return vfxDelayTime;
        }
    }




    
}
