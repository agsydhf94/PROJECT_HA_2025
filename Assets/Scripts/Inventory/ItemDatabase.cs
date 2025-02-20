using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class ItemDatabase : MonoBehaviour
    {
        public static Dictionary<int, ItemSO> itemDatabase = new Dictionary<int, ItemSO>();

        private void Awake()
        {
            // Resources �������� ��� ItemData ScriptableObject �ε�
            ItemSO[] items = Resources.LoadAll<ItemSO>("Item ScriptableObjects");

            // ��ųʸ� �ʱ�ȭ
            foreach(ItemSO item in items)
            {
                if (!itemDatabase.ContainsKey(item.itemID))
                {
                    itemDatabase.Add(item.itemID, item);
                    Debug.Log($"������ ID : {item.itemID} �����ͺ��̽��� �߰���");
                }
                else
                {
                    Debug.LogWarning($"�ߺ��� ID�� ���� �������� �߰����� ����: {item.itemID}");
                }
            }
        }
    }
}
