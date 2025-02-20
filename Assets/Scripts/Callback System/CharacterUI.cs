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

        //linkedCharacter.onDamageCallback += RefreshHpBar;  // "��������Ʈ�� chain�� �Ǵ�" ��� ǥ��
        // linkedCharacter.onDamagedAction += RefreshHpBar;

    }

    // �ǽð����� ��ȯ�Ǵ� ĳ���� ����
    private void Update()
    {
        currentCharacter = GameManager.Instance.currentActiveCharacter;
    }

    // �� ĳ���ͷ� ��ȯ�Ǿ��� ��, ��������Ʈ�� �޼��忡 Chain�ɱ�
    private void OnEnable()
    {
        GameManager.Instance.OnCharacterSwitch += HandleCharacterSwitch;
    }

    // �ٸ� ĳ���ͷ� ��ȯ�Ǿ��� ��, ���� ĳ������ Chain ����
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
        // ���⼭�� currentCharacter �� ��ȯ�ϱ� ���� ĳ����
        if (currentCharacter != null)
        {
            var currentCharacterBase = currentCharacter.GetComponent<CharacterBase>();
            currentCharacterBase.OnDamaged -= RefreshHpBar;
            currentCharacterBase.OnChangedHP -= RefreshHpBar;
            currentCharacterBase.OnChangedMP -= RefreshMPBar;
        }

        currentCharacter = newCharacter.gameObject;

        // ���⼭�� currentCharacter �� ��ȯ�� ���� ĳ����
        if (currentCharacter != null)
        {
            var currentCharacterBase = currentCharacter.GetComponent<CharacterBase>();
            currentCharacterBase.OnDamaged += RefreshHpBar;
            currentCharacterBase.OnChangedHP += RefreshHpBar;
            currentCharacterBase.OnChangedMP += RefreshMPBar;

            RefreshHpBar(currentCharacterBase.currentHP, currentCharacterBase.maxHP);
            RefreshMPBar(currentCharacterBase.currentMP, currentCharacterBase.maxHP);
            // ���� MP�� �����ϰ� �ִٸ� MP ���ŵ� ���⼭ �߰��� �� �ֽ��ϴ�.
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
