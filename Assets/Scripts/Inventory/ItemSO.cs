using HA;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HA
{
    [CreateAssetMenu]
    public class ItemSO : ScriptableObject
    {
        public int itemID;
        public string itemName;
        public Sprite itemSprite;
        public string itemDescription;
        public ItemType itemType;
        

        public AttributesToChange statToChange = new AttributesToChange();
        public int amountToChangeStat;

        public StatToChange attributesToChange = new StatToChange();
        public int amountToChangeAttribute;

        public CharacterBase characterBase => PlayerController.Instance.PlayerCharacterBase;


        

        public bool UseItem()
        {
            switch(statToChange)
            {
                case AttributesToChange.HEALTH:
                    {
                        if (characterBase.currentHP == characterBase.maxHP)
                        {
                            return false;
                        }
                        else
                        {
                            characterBase.IncreaseHP(amountToChangeStat);
                            return true;
                        }
                    }
                case AttributesToChange.MANA:
                    {
                        if(characterBase.currentMP == characterBase.maxMP)
                        {
                            return false;
                        }
                        else
                        {
                            characterBase.IncreaseMP(amountToChangeStat);
                            return true;
                        }
                    }
                case AttributesToChange.RIFLE_BULLET:
                    {
                        var currentWeapon = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerController>().currentWeapon;
                        currentWeapon.GetComponent<Weapon>().bulletTotal += amountToChangeAttribute;
                        /*
                        for(int i = 0; i < weaponArray.Length; i++)
                        {
                            if(weaponArray[i].GetComponent<Weapon>().weaponCategory == WeaponCategory.Rifle)
                            {
                                weaponArray[i].GetComponent<Weapon>().bulletTotal += amountToChangeStat;
                            }
                        }
                        */
                    }
                    break;
                case AttributesToChange.HANDGUN_BULLET:
                    {
                        var currentWeapon = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerController>().currentWeapon;
                        currentWeapon.GetComponent<Weapon>().bulletTotal += amountToChangeAttribute;

                        /*
                        for (int i = 0; i < weaponArray.Length; i++)
                        {
                            if (weaponArray[i].GetComponent<Weapon>().weaponCategory == WeaponCategory.Pistol)
                            {
                                weaponArray[i].GetComponent<Weapon>().bulletTotal += amountToChangeStat;
                            }
                        }
                        */
                    }
                    break;
            }
            /*
            if (statToChange == StatToChange.HEALTH)
            {
                //PlayerController.Instance.PlayerCharacterBase.AddHP(amountToChangeStat);
                //PlayerController.Instance.PlayerCharacterBase.currentHP += amountToChangeStat;
                
                
            }
            */
            return false;

            

            if (statToChange == AttributesToChange.MANA)
            {
                //PlayerController.Instance.PlayerCharacterBase.AddMP(amountToChangeStat);
                PlayerController.Instance.PlayerCharacterBase.currentMP += amountToChangeStat;
            }

            
        }



        
    }
}
