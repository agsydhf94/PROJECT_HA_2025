using HA;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace HA
{
    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<UIList, GameObject> uiInstances = new Dictionary<UIList, GameObject>();

        public T Show<T>(UIList uiType) where T : UIBase
        {
            var uiInstance = GetUI<T>(uiType);
            uiInstance?.Show();

            return uiInstance;
        }

        public T Hide<T>(UIList uiType) where T : UIBase
        {
            var uiInstance = GetUI<T>(uiType);
            uiInstance?.Hide();

            return uiInstance;
        }

        private Transform panelRoot;
        private Transform popupRoot;

        private Dictionary<UIList, UIBase> panels_dic = new Dictionary<UIList, UIBase>();
        private Dictionary<UIList, UIBase> popups_dic = new Dictionary<UIList, UIBase>();

        private bool isInitialized = false;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }



        // 이 스크립트가 붙어있는 HA.UIManager 오브젝트의 자식에
        // Panel Root, Popup Root 오브젝트를 생성
        // panel,popup 딕셔너리 초기화
        public void Initialize()
        {
            if (isInitialized)
                return;

            isInitialized = true;



            if (panelRoot == null)
            {
                GameObject goPanelRoot = new GameObject("Panel Root");
                panelRoot = goPanelRoot.transform;
                panelRoot.SetParent(transform);
                panelRoot.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                panelRoot.localScale = Vector3.one;
            }

            if (popupRoot == null)
            {
                GameObject goPopupRoot = new GameObject("Popup Root");
                popupRoot = goPopupRoot.transform;
                popupRoot.SetParent(transform);
                popupRoot.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                popupRoot.localScale = Vector3.one;
            }

            for (int index = (int)UIList.PANEL_START; index < (int)UIList.PANEL_END; index++)
            {
                panels_dic.Add((UIList)index, null);
            }

            for (int index = (int)UIList.POPUP_START; index < (int)UIList.POPUP_END; index++)
            {
                popups_dic.Add((UIList)index, null);
            }
        }

        // 위쪽에 선언 된 Dictionary 2개 panels/popups에 담겨져있는, UIList에 해당하는 UI 객체를 반환하는 함수
        // 만약에 Dictionary에 선언 된 panels/popups에 해당하는 UI 객체가 없다면
        // Resources.Load 를 틍해서 해당 UI Prefab을 복제 생성한다.

        public T GetUI<T>(UIList uiType, bool reload = false) where T : UIBase
        {
            if (UIList.PANEL_START < uiType && uiType < UIList.PANEL_END)
            {
                if (panels_dic.ContainsKey(uiType))
                {
                    if (reload && panels_dic[uiType] != null)
                    {
                        Destroy(panels_dic[uiType].gameObject);
                        panels_dic[uiType] = null;
                    }

                    if (panels_dic[uiType] == null)
                    {
                        string path = $"UI/UI.{uiType}";
                        UIBase loadedPrefab = Resources.Load<UIBase>(path);
                        if (loadedPrefab == null)
                            return null;

                        T result = loadedPrefab.GetComponent<T>();
                        if (result == null)
                            return null;

                        panels_dic[uiType] = Instantiate(loadedPrefab, panelRoot);
                        panels_dic[uiType].gameObject.SetActive(false);

                        return panels_dic[uiType].GetComponent<T>();
                    }
                    else
                    {
                        return panels_dic[uiType].GetComponent<T>();
                    }
                }

                if (UIList.POPUP_START < uiType && uiType < UIList.POPUP_END)
                {
                    if (popups_dic.ContainsKey(uiType))
                    {
                        if (reload && popups_dic[uiType] != null)
                        {
                            Destroy(popups_dic[uiType].gameObject);
                            popups_dic[uiType] = null;
                        }

                        if (popups_dic[uiType] == null)
                        {
                            string path = $"UI/UI.{uiType}";
                            UIBase loadedPrefab = Resources.Load<UIBase>(path);
                            if (loadedPrefab == null)
                                return null;

                            T result = loadedPrefab.GetComponent<T>();
                            if (result == null)
                                return null;

                            popups_dic[uiType] = Instantiate(loadedPrefab, popupRoot);
                            popups_dic[uiType].gameObject.SetActive(false);

                            return popups_dic[uiType].GetComponent<T>();
                        }
                        else
                        {
                            return popups_dic[uiType].GetComponent<T>();
                        }
                    }
                }
            }

            return null;
        }





        // 퀘스트 북 UI 버튼 프리팹 정보 전달
        // 정보를 

        
    }
}
    

