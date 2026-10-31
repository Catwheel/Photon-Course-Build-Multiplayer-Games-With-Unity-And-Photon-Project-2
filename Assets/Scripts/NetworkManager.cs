using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using WebSocketSharp;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status")]
    public Text connectionStatusText;

    [Header("UI Login Panel")]
    public InputField PlayerNameInput;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        
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
    }
    #endregion
}
