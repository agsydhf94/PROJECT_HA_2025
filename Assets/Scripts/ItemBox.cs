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
            // To do : ������ �ڽ��� ���ְ�, �������� �÷��̾��� �κ��丮�� �־��ش�.

            // Player Character �� �ݱ� ����� Ʈ�����Ѵ�.
            // SingleTon �������� ������ �����ϴ� Ŭ������ ���� ����(playercontroller �� ������ �ʹ� Ŀ�� 
        }
    }
}
