using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

namespace HA
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        public TMP_InputField roomInputField;
        public GameObject lobbyPanel;
        public GameObject roomPanel;
        public TMP_Text roomName;

        public RoomItem roomItemPrefab;
        List<RoomItem> roomItemList = new List<RoomItem>();
        public Transform contentObject;

        public float timeBetweenUpdates = 1.5f;
        private float nextUpdateTime;

        public List<PlayerItem> playerItemsList = new List<PlayerItem>();
        public PlayerItem playerItemPrefab;
        public Transform playerItemParent;

        public GameObject playButton;


        private void Start()
        {
            PhotonNetwork.JoinLobby();
        }

        

        public void OnClickCreate()
        {
            if(roomInputField.text.Length >= 1)
            {
                PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 4});
            }
        }

        // 방에 들어갔을 때 호출되는 함수
        public override void OnJoinedRoom()
        {
            lobbyPanel.SetActive(false);
            roomPanel.SetActive(true);
            roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
            UpdatePlayerList();
        }

        private void UpdatePlayerList()
        {
            // 플레이어 리스트 초기화
            foreach(PlayerItem item in playerItemsList)
            {
                Destroy(item.gameObject);
            }
            playerItemsList.Clear();

            // 아직 방에 입장하지 않았다면
            // 방에 입장했을 때만 실행되어야 함
            if(PhotonNetwork.CurrentRoom == null)
            {
                return;
            }

            foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
                newPlayerItem.SetPlayerInfo(player.Value);
                playerItemsList.Add(newPlayerItem);
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if(Time.time >= nextUpdateTime + timeBetweenUpdates)
            {
                UpdateRoomList(roomList);
                nextUpdateTime = Time.time;
            }

            
        }

        private void UpdateRoomList(List<RoomInfo> list)
        {
            foreach(RoomItem item in roomItemList)
            {
                Destroy(item.gameObject);
            }
            roomItemList.Clear();

            foreach(RoomInfo room in list)
            {
                var newRoom = Instantiate(roomItemPrefab, contentObject);
                newRoom.SetRoomName(room.Name);
                roomItemList.Add(newRoom);
            }
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        // 방 나가기 버튼 클릭했을 때 호출
        public void OnClickLeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        // 방을 나가면 자동으로 호출
        public override void OnLeftRoom()
        {
            roomPanel.SetActive(false);
            lobbyPanel.SetActive(true);
        }

        // 방을 만들었다가 나갔을 때, 로비에 만들었던 방이 계속 뜨게 해줌
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdatePlayerList();
        }

        private void Update()
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                playButton.SetActive(true);
            }
            else
            {
                playButton.SetActive(false);
            }
        }

        public void OnClickPlayButton()
        {
            PhotonNetwork.LoadLevel("TPS Shooting_Zombie_Multiplayer");
        }
    }
}
