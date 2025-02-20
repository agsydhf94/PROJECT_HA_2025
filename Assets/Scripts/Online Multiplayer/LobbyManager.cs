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

        // �濡 ���� �� ȣ��Ǵ� �Լ�
        public override void OnJoinedRoom()
        {
            lobbyPanel.SetActive(false);
            roomPanel.SetActive(true);
            roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
            UpdatePlayerList();
        }

        private void UpdatePlayerList()
        {
            // �÷��̾� ����Ʈ �ʱ�ȭ
            foreach(PlayerItem item in playerItemsList)
            {
                Destroy(item.gameObject);
            }
            playerItemsList.Clear();

            // ���� �濡 �������� �ʾҴٸ�
            // �濡 �������� ���� ����Ǿ�� ��
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

        // �� ������ ��ư Ŭ������ �� ȣ��
        public void OnClickLeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        // ���� ������ �ڵ����� ȣ��
        public override void OnLeftRoom()
        {
            roomPanel.SetActive(false);
            lobbyPanel.SetActive(true);
        }

        // ���� ������ٰ� ������ ��, �κ� ������� ���� ��� �߰� ����
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
