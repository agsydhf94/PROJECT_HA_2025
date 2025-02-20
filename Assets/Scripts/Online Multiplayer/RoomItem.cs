using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HA;

namespace HA
{
    public class RoomItem : MonoBehaviour
    {
        public TMP_Text roomName;
        LobbyManager lobbyManager;

        private void Start()
        {
            lobbyManager = FindObjectOfType<LobbyManager>();
        }

        public void SetRoomName(string _roomName)
        {
            roomName.text = _roomName;
        }

        public void OnClickItem()
        {
            lobbyManager.JoinRoom(roomName.text);
        }
    }
}
