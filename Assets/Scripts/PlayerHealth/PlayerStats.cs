using HA;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public int HP;
    public int MP;
    public float attack;
    public float defense;

    public TMP_Text HPText;
    public TMP_Text MPText;
    public TMP_Text attackText;
    public TMP_Text defenseText;

    public TMP_Text attackPreviewText;
    public TMP_Text defensePreviewText;
    public Image previewImage;

    public GameObject selectedEquipmentImage;
    public GameObject selectedEquipmentStat;

    public CharacterBase currentCharacterBase;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    private void Start()
    {
        currentCharacterBase = GameManager.Instance.GetComponent<CharacterBase>();
        UpdateEquipmentStats();   
    }

    private void Update()
    {
        
    }

    public void LifeRelatedStatUpdate()
    {
        HPText.text = currentCharacterBase.currentHP.ToString();
        MPText.text = currentCharacterBase.currentMP.ToString();

    }

    public void UpdateEquipmentStats()
    {
        attackText.text = attack.ToString();
        defenseText.text = defense.ToString();
    }

    public void EquipmentPreview(float attack, float defense, Sprite itemSprite)
    {
        attackPreviewText.text = attack.ToString();
        defensePreviewText.text = defense.ToString();

        previewImage.sprite = itemSprite;

        selectedEquipmentImage.SetActive(true);
        selectedEquipmentStat.SetActive(true);
    }

    public void ActivePreview()
    {
        selectedEquipmentImage.SetActive(false);
        selectedEquipmentStat.SetActive(false);
    }
}
