using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
public class MainMenuController : MonoBehaviourPunCallbacks
{
    public Animator animator;
    public TMP_InputField inputField;
    public GameObject OnlineMenu;
    public JoinOnlineGame joinOnlineGame;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape))
            OnEscapeButton();
    }
    public void OnStartGameClick()
    {
        StartCoroutine(LoadNextLevel());
    }
    public void OnExitGameClick()
    {
        Application.Quit();
    }

    public IEnumerator LoadNextLevel()
    {
        animator.SetTrigger("StartLoading");
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("EndLoading");
        yield return new WaitForSeconds(1.6f);
        SceneManager.LoadScene("Game");
    }

    public IEnumerator LoadOnlineLevel()
    {
        animator.SetTrigger("StartLoading");
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("EndLoading");
        yield return new WaitForSeconds(1.6f);
        PhotonNetwork.LoadLevel(2);
    }
    public void OnCreateButton()
    {
        PhotonNetwork.CreateRoom(inputField.text);
    }
    public void OnJoinButton()
    {
        PhotonNetwork.JoinRoom(inputField.text);
    }
    public override void OnCreatedRoom()
    {
        StartCoroutine(LoadOnlineLevel());
    }
    public override void OnJoinedRoom()
    {
        StartCoroutine(LoadOnlineLevel());
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }
    public void OnOnlineButtton()
    {
        OnlineMenu.SetActive(true);
        joinOnlineGame.OnConnectToServer();
    }
    public void OnOnlineCloseButton()
    {
        OnlineMenu.SetActive(false);
    }
    public void OnEscapeButton()
    {
        if(OnlineMenu.activeSelf)
            OnOnlineCloseButton();
    }
}
