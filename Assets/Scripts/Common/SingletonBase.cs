using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace HA
{
    public class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
    {
        private static T m_Instance = null;
        private static object _syncobj = new object();
        private static bool appIsClosing = false;

        public static T Instance
        {
            get
            {
                if (appIsClosing)
                    return null;

                lock (_syncobj)
                {
                    if (m_Instance == null)
                    {
                        T[] objs = FindObjectsOfType<T>();

                        if (objs.Length > 0)
                            m_Instance = objs[0];

                        if (objs.Length > 1)
                            Debug.Log("There is more than one " + typeof(T).Name + " in the scene.");

                        if (m_Instance == null)
                        {
                            //이쪽이 첫번째로 들어오고...
                            string goName = typeof(T).ToString();
                            GameObject a_go = GameObject.Find(goName);
                            if (a_go == null)
                                a_go = new GameObject(goName);
                            m_Instance = a_go.AddComponent<T>();  //Awake()가 이쪽에서 발생된다.
                        }
                        else
                        {
                            m_Instance.Init();
                        }
                    }

                    return m_Instance;
                }//lock (_syncobj)
            } // get
        }//public static T Instance

        public virtual void Awake()
        {
            //이쪽이 두번째로 들어온다.
            Init();
        }

        protected virtual void Init()
        {
            if (m_Instance == null)
            {
                m_Instance = this as T;
                DontDestroyOnLoad(base.gameObject);
            }
            else
            {
                if (m_Instance != this)
                {
                    DestroyImmediate(base.gameObject);
                }
            }
        } // 초기화를 상속을 통해 구현   

        private void OnApplicationQuit()  //앱 강제 종료시 발생되는 함수
        {
            m_Instance = null;
            appIsClosing = true;
        }
    }
}
