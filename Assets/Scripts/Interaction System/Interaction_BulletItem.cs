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

            // ȭ��󿡼� ������ ���� �� UI���� ����
            Destroy(gameObject);
            InteractionUI.Instance.RemoveInteractionData(this);
        }
    }
}
