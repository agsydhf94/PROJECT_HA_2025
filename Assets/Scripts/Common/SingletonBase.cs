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
                            //������ ù��°�� ������...
                            string goName = typeof(T).ToString();
                            GameObject a_go = GameObject.Find(goName);
                            if (a_go == null)
                                a_go = new GameObject(goName);
                            m_Instance = a_go.AddComponent<T>();  //Awake()�� ���ʿ��� �߻��ȴ�.
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
            //������ �ι�°�� ���´�.
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
        } // �ʱ�ȭ�� ����� ���� ����   

        private void OnApplicationQuit()  //�� ���� ����� �߻��Ǵ� �Լ�
        {
            m_Instance = null;
            appIsClosing = true;
        }
    }
}
