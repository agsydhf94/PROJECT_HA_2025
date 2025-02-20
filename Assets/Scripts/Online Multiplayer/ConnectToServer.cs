using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace HA
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        public TMP_InputField usernameInput;
        public TMP_Text buttonText;

        public void OnClickConnect()
        {
            if (usernameInput.text.Length >= 1)
            {
                PhotonNetwork.NickName = usernameInput.text;
                buttonText.text = "Connecting...";
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        // ���� ���� ���ӿ� �����ϸ� ȣ�� ��
        public override void OnConnectedToMaster()
        {
            SceneManager.LoadScene("Online Multiplayer_Lobby");
        }
    }
}
