using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUI: MonoBehaviour
{
    public Image hpBar;
    public Image mpBar;

    public GameObject currentCharacter;

    [SerializeField]
    public TMP_Text hpText;
    public TMP_Text mpText;

    //public CharacterBase linkedCharacter;



    private void Start()
    {
       
        SwitchCharacter(GameManager.Instance.currentActiveCharacter.GetComponent<CharacterBase>());

        GameManager.Instance.currentActiveCharacter.GetComponent<CharacterBase>().OnDamaged += RefreshHpBar;
        GameManager.Instance.currentActiveCharacter.GetComponent<CharacterBase>().OnChangedHP += RefreshHpBar;
        GameManager.Instance.currentActiveCharacter.GetComponent<CharacterBase>().OnChangedMP += RefreshMPBar;

        //PlayerController.Instance.PlayerCharacterBase.OnDamaged += RefreshHpBar;
        //PlayerController.Instance.PlayerCharacterBase.OnChangedHP += RefreshHpBar;

        //linkedCharacter.onDamageCallback += RefreshHpBar;  // "델리게이트에 chain을 건다" 라고 표현
        // linkedCharacter.onDamagedAction += RefreshHpBar;

    }

    // 실시간으로 전환되는 캐릭터 감지
    private void Update()
    {
        currentCharacter = GameManager.Instance.currentActiveCharacter;
    }

    // 새 캐릭터로 전환되었을 때, 델리게이트에 메서드에 Chain걸기
    private void OnEnable()
    {
        GameManager.Instance.OnCharacterSwitch += HandleCharacterSwitch;
    }

    // 다른 캐릭터로 전환되었을 때, 기존 캐릭터의 Chain 해제
    private void OnDisable()
    {
        GameManager.Instance.OnCharacterSwitch -= HandleCharacterSwitch;
    }

    private void HandleCharacterSwitch(CharacterBase newCharacter)
    {
        SwitchCharacter(newCharacter);
    }

    private void SwitchCharacter(CharacterBase newCharacter)
    {
        // 여기서의 currentCharacter 는 전환하기 전의 캐릭터
        if (currentCharacter != null)
        {
            var currentCharacterBase = currentCharacter.GetComponent<CharacterBase>();
            currentCharacterBase.OnDamaged -= RefreshHpBar;
            currentCharacterBase.OnChangedHP -= RefreshHpBar;
            currentCharacterBase.OnChangedMP -= RefreshMPBar;
        }

        currentCharacter = newCharacter.gameObject;

        // 여기서의 currentCharacter 는 전환한 후의 캐릭터
        if (currentCharacter != null)
        {
            var currentCharacterBase = currentCharacter.GetComponent<CharacterBase>();
            currentCharacterBase.OnDamaged += RefreshHpBar;
            currentCharacterBase.OnChangedHP += RefreshHpBar;
            currentCharacterBase.OnChangedMP += RefreshMPBar;

            RefreshHpBar(currentCharacterBase.currentHP, currentCharacterBase.maxHP);
            RefreshMPBar(currentCharacterBase.currentMP, currentCharacterBase.maxHP);
            // 만약 MP도 관리하고 있다면 MP 갱신도 여기서 추가할 수 있습니다.
        }
    }


    public void RefreshHpBar(float currentHp, float maxHp)
    {
        hpBar.fillAmount = currentHp / maxHp;
        hpText.text = $"{currentHp:0} / {maxHp:0}";
        // InventoryManager.Instance.hpStatText.text = $"{currentHp:0} / {maxHp:0}";
    }

    
    public void RefreshMPBar(float currentMp, float maxMp)
    {
        mpBar.fillAmount = currentMp / maxMp;
        mpText.text = $"{currentMp:0} / {maxMp:0}";
        // InventoryManager.Instance.mpStatText.text = $"{currentMp:0} / {maxMp:0}";

    }
    
}
