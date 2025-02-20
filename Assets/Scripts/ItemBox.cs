using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class ItemBox : MonoBehaviour, IInteractable
    {
        public string Key => throw new System.NotImplementedException();
        public string Message => throw new System.NotImplementedException();


        public void Interact()
        {
            // To do : 아이템 박스를 없애고, 아이템을 플레이어의 인벤토리에 넣어준다.

            // Player Character 의 줍기 모션을 트리거한다.
            // SingleTon 패턴으로 아이템 저장하는 클래스를 따로 만듬(playercontroller 에 넣으면 너무 커짐 
        }
    }
}
