using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class JoinOnlineGame : MonoBehaviourPunCallbacks
{
    public GameObject waitingScreen;
    public void OnConnectToServer()
    {
        waitingScreen.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        waitingScreen.SetActive(false);
    }
}
