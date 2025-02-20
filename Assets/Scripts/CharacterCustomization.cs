using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterCustomization : MonoBehaviour
{
    public GameObject playerCharacterAsuna;

    // 어깨 패드
    public GameObject armor_ShoulderLeft;
    public GameObject armor_UpperArmLeft;
    public GameObject armor_EllbowLeft;
    public GameObject armor_ShoulderRight;
    public GameObject armor_UpperArmRight;
    public GameObject armor_EllbowRight;

    // player 캐릭터 무기
    public GameObject handGun;
    public GameObject scifiRifle;

    // 무릎
    public GameObject armor_KneeLeft;
    public GameObject armor_KneeRight;


    private void Start()
    {
        
    }

    public bool rotateCharacter = false;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.R))
        {
            this.rotateCharacter = !rotateCharacter;
        }

        if(this.rotateCharacter)
        {
            this.playerCharacterAsuna.transform.Rotate(new Vector3(0,1,0), 33.0f * Time.deltaTime);
        }
        if(Input.GetKeyUp(KeyCode.L))
        {
            // Debug.Log(PlayerPrefs.GetString("NAME"));
        }
    }

    // id.name = 토글의 하이어라키 상에 있는 이름
    public void SetShoulderPad(Toggle id)
    {
        switch(id.name)
        {
            case "Toggle_Shoulder Armor":
                this.armor_ShoulderLeft.SetActive(id.isOn);
                this.armor_ShoulderRight.SetActive(id.isOn);                
                break;

            case "Toggle_UpperArm Armor":
                this.armor_UpperArmLeft.SetActive(id.isOn);
                this.armor_UpperArmRight.SetActive(id.isOn);
                break;

            case "Toggle_Elbow Armor":
                this.armor_EllbowLeft.SetActive(id.isOn);
                this.armor_EllbowRight.SetActive(id.isOn);               
                break;

        }
    }

    public void SetKneePad(Toggle id)
    {
        switch(id.name)
        {
            case "Toggle_Knee Armor":
                {
                    this.armor_KneeLeft.SetActive(id.isOn);
                    this.armor_KneeRight.SetActive(id.isOn);
                }
                break;
        }
    }

    public void SetRifle(Toggle id)
    {
        switch(id.name)
        {
            case "Toggle_SciFiRifle":
                {
                    this.scifiRifle.SetActive(id.isOn);
                }
                break;
        }
    }

    public void SetHandGun(Toggle id)
    {
        switch(id.name)
        {
            case "Toggle_Handgun":
                {
                    this.handGun.SetActive(id.isOn);
                }
                break;
        }
    }

    
}

