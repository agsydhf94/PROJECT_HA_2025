using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace HA
{
    public class PlayerItem : MonoBehaviour
    {
        public TMP_Text playerName;

        public void SetPlayerInfo(Player _player)
        {
            playerName.text = _player.NickName;
        }
    }
}
