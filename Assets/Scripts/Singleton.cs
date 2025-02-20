using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    // 기존에 할당된 인스턴스가 있는지 확인
                    _instance = FindObjectOfType<T>();

                    if(_instance == null)
                    {
                        // 새로운 GameObject를 생성하여 싱글톤 인스턴스 추가
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        // 씬 전환 시 파괴되지 않도록 설정
        protected virtual void Awake()
        {
            if(_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if(_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}