using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class BulletItem : MonoBehaviour
    {

        public int extrabullet = 40;
        public InventoryItem inventoryItem;
        public Weapon weapon;

        private void Awake()
        {
            weapon = GameObject.Find("ScifiRifleWLT78Receiver").GetComponent<Weapon>();
        }

        private void Update()
        {
            // 특정 함수가 실행되면? ItemFunction() 실행
        }


        public void ItemFunction()
        {
            weapon.bulletTotal += extrabullet;
        }
    }
}
