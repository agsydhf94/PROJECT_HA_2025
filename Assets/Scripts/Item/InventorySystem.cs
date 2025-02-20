using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [Serializable]
    public class InventorySystem : MonoBehaviour
    {
        [SerializeField]
        private List<WeaponItemData> weapons = new List<WeaponItemData>();
        [SerializeField]
        private List<ArmourItemData> armour = new List<ArmourItemData>();
        [SerializeField]
        private List<HealthItemData> health = new List<HealthItemData>();
        [SerializeField]
        private List<PotionItemData> potion = new List<PotionItemData>();

        private InventoryItem selectedWeapon;
        private InventoryItem selectedArmor;

        public InventoryItem Selected_Weapon
        {
            get { return selectedWeapon; }
            set { selectedWeapon = value; }
        }

        public InventoryItem Selected_Armor
        {
            get { return selectedArmor; }
            set { selectedArmor = value; }
        }

        /*
        void SampleFunction()
        {
            // GameMaster 스크립트를 사용하고자 하는데
            // 그러려면 해당 스크립트가 들어있는 게임오브젝트 찾고
            // 오브젝트 찾았으면 안에 들어있는 GameMaster 스크립트에 접근해서
            // GameMaster 스크립트 안에 있는 것들을 막 이용하고 싶은 건데 이러면 너무 귀찮다
            GameObject gameMasterObject =  GameObject.Find("HA.GameMaster");
            GameMaster gameMasterComp = gameMasterObject.GetComponent<GameMaster>();
            gameMasterComp.fx_Level = 1;
            gameMasterComp.MasterVolume(100);

            // GameMaster 스크립트 내에서 이미 GameMaster 타입의 static 변수 instance 가 생성되어 있고
            // 이를 바로 접근할 수 있다
            GameMaster.instance.fx_Level = 1;
            GameMaster.instance.MasterVolume(100);
        }
        */

        public InventorySystem()
        {
            ClearInventory();
        }

        public void ClearInventory()
        {
            weapons.Clear();
            armour.Clear();
            health.Clear();
            potion.Clear();
        }


        // 인벤토리에 아이템을 추가하는 함수
        public void AddItem(InventoryItem item)
        {
            switch (item.Category)
            {
                case ItemCategory.WEAPON:
                    weapons.Add(new WeaponItemData()
                    {
                        itemID = "",
                        itemName = item.Name,
                        category = ItemCategory.WEAPON,
                        remainAmmo = 10,
                    });
                    break;
                //case ItemCategory.ARMOUR:
                //    armour.Add(item);
                //    break;

                //case ItemCategory.HEALTH:
                //    health.Add(item);
                //    break;
                case ItemCategory.POTION:
                    potion.Add(new PotionItemData()
                    {
                        itemName = item.Name,
                        itemDescription = item.Description,
                        category = ItemCategory.POTION,
                    });
                    break;
            }
        }


        // 인벤토리에서 아이템을 제거하는 함수
        public void DeleteItem(InventoryItem item)
        {
            //switch (item.Category)
            //{
            //    case ItemCategory.WEAPON:
            //        weapons.Remove(item);
            //        break;

            //    case ItemCategory.ARMOUR:
            //        armour.Remove(item);
            //        break;

            //    case ItemCategory.CLOTHING:
            //        clothing.Remove(item);
            //        break;

            //    case ItemCategory.HEALTH:
            //        health.Remove(item);
            //        break;

            //    case ItemCategory.POTION:
            //        potion.Remove(item);
            //        break;
            //}
        }
    }

    /*
     done :    라이플 재장전 애니메이션(상체 레이어 마스크 적용, 애니메이션 이벤트 타이밍 델리게이트) , 
        보유 탄수-현재 장전수 UI, 스크립트 구현 
        사격시 총 발사 음원 재생
        총알 맞아을 때 재질별 이펙트

        아이템 및 무기 인벤토리 베이스 스크립트 작업 중
        인벤토리 UI 임시 작업 중(그리드 레이아웃, 마우스 스크롤)

    to do until next week : 아이템 인벤토리 완성(UI및 아이템 설명 표시까지)
                            시간되면 무기전환 시스템 만들기(라이플, 권총, 카타나 등)

    앞으로 하고 싶은 것 : 스킬 구현(애니메이션, 스킬 발동 시 데미지 구역 만들기, 쿨타임)
                         적 스폰 및 AI
                         보스 패턴
                         캐릭터 경험치 시스템
                         상점 시스템(인벤토리와 연계)

    wepapon currenwea 넣고 꺼내는

     => currentWeapon
     => weaponToEquip

    1번 무기에서 2번무기로 스왑
    currnetWeapon (1번) => holster => holsterFinished? => weaponToEquip != null => Equip하는 모션실행
                        => (weaponToEquip = 2번 무기)			                => 2번 무기 꺼내기
     */

}
