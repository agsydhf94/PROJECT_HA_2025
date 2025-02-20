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

        
        // 프리팹 채로 Instantiate 되는 유형이므로
        // Instantiate 되어서 활성화되는 순간 정보를 불러오기
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
                    // 닌자 캐릭터의 스킬 포인트를 가져와야 댐
                    // 닌자 캐릭터에 어떻게 접근하지?

                    if(SkillManager.Instance.SkillPointCheck() >= requiredForUnlock)
                    {
                        // 스킬을 언락하기에 충분한 포인트가 있으면
                        // 언락하고
                        // 차감된 만큼 스킬포인트를 갱신
                        
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

                        // Activate 된 스킬의 정보를
                        // current Character로 쏴주어야함.
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
                // 스킬트리가 아닌 현재 사용하고 있는 스킬 창일 경우에는
                // 내가 선택한 스킬의 skillSO 를 로딩하는 역할 수행

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
