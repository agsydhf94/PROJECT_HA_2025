using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class TransformUtility
    {
        public static GameObject FindGameObjectWithTag(GameObject root, string tag)
        {
            return FindFirstChildWithTag(root, tag);
        }

        // 특정 태그를 가진 첫 번째 자식 GameObject를 찾는 함수
        private static GameObject FindFirstChildWithTag(GameObject parent, string tag)
        {
            // 모든 자식 검색
            foreach (Transform child in parent.transform)
            {
                // 자식이 태그와 일치하는 경우 해당 객체 반환
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }

                // 자식의 자식도 검색 (재귀 호출)
                GameObject foundChild = FindFirstChildWithTag(child.gameObject, tag);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            // 일치하는 태그를 가진 자식이 없는 경우 null 반환
            return null;
        }
    }
}
