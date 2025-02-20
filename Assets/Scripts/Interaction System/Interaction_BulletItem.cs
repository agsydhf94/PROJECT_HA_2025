using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Interaction_BulletItem : Interaction_Item
    {
        public int extrabullet = 40;

        public override void Interact()
        {
            Weapon weapon;
            weapon = PlayerController.Instance.currentWeapon;
            weapon.bulletTotal += extrabullet;

            // 화면상에서 아이템 제거 및 UI에서 제거
            Destroy(gameObject);
            InteractionUI.Instance.RemoveInteractionData(this);
        }
    }
}
