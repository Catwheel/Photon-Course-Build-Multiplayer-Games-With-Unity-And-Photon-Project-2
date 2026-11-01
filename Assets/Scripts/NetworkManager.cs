using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using WebSocketSharp;
using System;
using UnityEngine.UIElements;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status")]
    public Text connectionStatusText;

    [Header("UI Login Panel")]
    public InputField PlayerNameInput;
    public GameObject Login_UI_Panel;

    [Header("Game Options UI Panel")]
    public GameObject GameOptions_UI_Panel;
    
    [Header("Create Room Panel")]
    public GameObject CreateRoom_UI_Panel;
    
    [Header("Inside Room Panel")]
    public GameObject InsideRoom_UI_Panel;

    [Header("Room List Panel")]
    public GameObject RoomList_UI_Panel;

    [Header("Join Random RoomPanel")]
    public GameObject JoinRandomRoom_UI_Panel;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(Login_UI_Panel.name);
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
    }
    #endregion


    #region UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;
        if(!String.IsNullOrEmpty(playerName))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("playerName is invalid!");
        }
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is Connected to Photon");
        ActivatePanel(GameOptions_UI_Panel.name);
    }
    #endregion

    #region Public Methods

    public void ActivatePanel(string panelToBeActivated)
    {
        Login_UI_Panel.SetActive(panelToBeActivated.Equals(Login_UI_Panel.name));
        GameOptions_UI_Panel.SetActive(panelToBeActivated.Equals(GameOptions_UI_Panel.name));
        CreateRoom_UI_Panel.SetActive(panelToBeActivated.Equals(CreateRoom_UI_Panel.name));
        InsideRoom_UI_Panel.SetActive(panelToBeActivated.Equals(InsideRoom_UI_Panel.name));
        RoomList_UI_Panel.SetActive(panelToBeActivated.Equals(RoomList_UI_Panel.name));
        JoinRandomRoom_UI_Panel.SetActive(panelToBeActivated.Equals(JoinRandomRoom_UI_Panel.name));
    }

    #endregion
}
