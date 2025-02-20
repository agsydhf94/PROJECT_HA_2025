using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


namespace HA
{
    public class SkillSlot : MonoBehaviour, IPointerClickHandler
    {
        public ISkillState skillState;
        public TMP_Text skillName;
        public float addAttack;
        public float addRange;
        // public TMP_Text skillDescription;
        public float coolTime;
        public bool isCooled = true;
        public TMP_Text cooltimeCountdown;

        public Image skillSlotImage;
        public Sprite skillSprite;
        public GameObject skillSelectedImage;
        public GameObject skillActivatedImage;
        public GameObject skillLockedImage;
        public Image coolTimeImage;
        public Color cooltimeColor;
        public Color characterColor;

        public SkillSO skillSO;
        public bool isThisSlotSelected;
        public bool isUnlocked;
        public int requiredForUnlock;

        public bool isUsedForSkillTree;
        public bool isActivated;

        public AudioSource audioSource;
        public AudioClip skillSelected_SE;
        public AudioClip skillActivated_SE;
        public AudioClip skillIsFull_SE;
        public AudioClip skillDeactivated_SE;



        public SkillManager skillManager;
        public PlayerData currentPlayerData;

        
        // ������ ä�� Instantiate �Ǵ� �����̹Ƿ�
        // Instantiate �Ǿ Ȱ��ȭ�Ǵ� ���� ������ �ҷ�����
        private void Awake()
        {            
            if(isUsedForSkillTree)
            {
                skillName.text = skillSO.name;
                addAttack = skillSO.attackPoint;

                skillSprite = skillSO.skillSprite;
                skillSlotImage.sprite = skillSprite;
            }  
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left && isUsedForSkillTree)
            {
                OnClickLeft();
            }
        }

        public void OnClickLeft()
        {
            if(!isThisSlotSelected && isUsedForSkillTree)
            {
                PlaySE(skillSelected_SE);
                SkillManager.Instance.DeselectAllSkillSlots();
                isThisSlotSelected = true;
                skillSelectedImage.SetActive(true);
                SkillManager.Instance.selected_SkillDescription.text = skillSO.skillDescription;
            }
            else if(isThisSlotSelected && isUsedForSkillTree)
            {
                if(!isUnlocked)
                {
                    // ���� ĳ������ ��ų ����Ʈ�� �����;� ��
                    // ���� ĳ���Ϳ� ��� ��������?

                    if(SkillManager.Instance.SkillPointCheck() >= requiredForUnlock)
                    {
                        // ��ų�� ����ϱ⿡ ����� ����Ʈ�� ������
                        // ����ϰ�
                        // ������ ��ŭ ��ų����Ʈ�� ����
                        
                        isUnlocked = true;
                        skillLockedImage.SetActive(false);
                        GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().skillPoint -= requiredForUnlock;
                        
                    }
                }
                else if(isUnlocked)
                {
                    if(!isActivated && GameManager.Instance.currentcharacterActiveSkills.Count < 2)
                    {
                        PlaySE(skillActivated_SE);
                        isActivated = true;
                        skillActivatedImage.SetActive(true);

                        // Activate �� ��ų�� ������
                        // current Character�� ���־����.
                        GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().currentActiveSkills.Add(this);
                        SkillManager.Instance.CurrentActivatedSkill_DataTransfer(this);
                    }
                    else if(!isActivated && GameManager.Instance.currentcharacterActiveSkills.Count == 2)
                    {
                        PlaySE(skillIsFull_SE);
                    }
                    else if(isActivated)
                    {
                        PlaySE(skillDeactivated_SE);
                        isActivated = false;
                        skillActivatedImage.SetActive(false);
                        GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().currentActiveSkills.Remove(this);
                        SkillManager.Instance.CurrentActivatedSkill_DataRemove(this);
                    }
                }
            }
            
        }



        /*
        public void SkillDataLoader(string skill_name)
        {           
            foreach(var skillSlot in SkillManager.Instance.skillSlots)
            {
                if(skillSlot.name == skill_name)
                {
                    skillSO = skillSlot.skillSO;
                }
            }

            if(!isUsedForSkillTree)
            {
                // ��ųƮ���� �ƴ� ���� ����ϰ� �ִ� ��ų â�� ��쿡��
                // ���� ������ ��ų�� skillSO �� �ε��ϴ� ���� ����

                skillName.text = skillSO.name;
                addAttack = skillSO.addAttack;
                addRange = skillSO.addRange;

                skillSprite = skillSO.skillSprite;
                skillSlotImage.sprite = skillSprite;
            }
            else if(isUsedForSkillTree)
            {
                skillActivatedImage.SetActive(true);
            }
            
        }
        */
        

        public void SkillDataReset()
        {
            skillName.text = "";
            addAttack = 0;
            addRange = 0;

            skillSlotImage.sprite = null;
        }

        public void PlaySE(AudioClip audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }


  

}
