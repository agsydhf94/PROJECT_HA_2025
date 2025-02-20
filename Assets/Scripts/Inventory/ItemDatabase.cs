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
            // Resources 폴더에서 모든 ItemData ScriptableObject 로드
            ItemSO[] items = Resources.LoadAll<ItemSO>("Item ScriptableObjects");

            // 딕셔너리 초기화
            foreach(ItemSO item in items)
            {
                if (!itemDatabase.ContainsKey(item.itemID))
                {
                    itemDatabase.Add(item.itemID, item);
                    Debug.Log($"아이템 ID : {item.itemID} 데이터베이스에 추가됨");
                }
                else
                {
                    Debug.LogWarning($"중복된 ID로 인해 아이템이 추가되지 않음: {item.itemID}");
                }
            }
        }
    }
}
