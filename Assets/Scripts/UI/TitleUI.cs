using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class TitleUI : UIBase
    {
        public void OnClickGameStart()
        {
            Main.Instance.ChangeScene(SceneType.InGameScene02);
        }

        public void OnClickGameExit()
        {
            Main.Instance.SystemQuit();
        }
    }
}
