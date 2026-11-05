using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using WebSocketSharp;
using System;
using UnityEngine.UIElements;
using Photon.Realtime;
using System.Linq.Expressions;

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
    public InputField RoomNameInputField;
    public InputField MaxPlayerInputField;
    
    [Header("Inside Room Panel")]
    public GameObject InsideRoom_UI_Panel;

    [Header("Room List Panel")]
    public GameObject RoomList_UI_Panel;
    public GameObject roomListEntryPrefab;
    public GameObject roomListParentGameObject;

    [Header("Join Random RoomPanel")]
    public GameObject JoinRandomRoom_UI_Panel;
    private Dictionary<string,RoomInfo> catchedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(Login_UI_Panel.name);

        catchedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
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

    public void OnRoomCreateButtonClicked()
    {
        string roomName = RoomNameInputField.text;
        if(String.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + UnityEngine.Random.Range(1000,10000);
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(MaxPlayerInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public void OnShowRoomButtonsClicked()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(RoomList_UI_Panel.name);

    }

    public void OnBackButtonClicked()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(GameOptions_UI_Panel.name);
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
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(InsideRoom_UI_Panel.name);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomList();
        foreach(RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if(catchedRoomList.ContainsKey(room.Name))
                {
                    catchedRoomList.Remove(room.Name);
                }
            }
            else
            {
                if(catchedRoomList.ContainsKey(room.Name))
                {
                    catchedRoomList[room.Name]= room;
                }
                else
                    catchedRoomList.Add(room.Name, room);
            }
        }
        foreach(RoomInfo room in catchedRoomList.Values)
        {
            GameObject roomListEntryGameObject = Instantiate(roomListEntryPrefab);
            roomListEntryGameObject.transform.SetParent(roomListParentGameObject.transform);
            roomListEntryGameObject.transform.localScale = Vector3.one;
            roomListEntryGameObject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListEntryGameObject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount + " / " + room.MaxPlayers;
            roomListEntryGameObject.transform.Find("JoinRoomButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name));
            roomListGameObjects.Add(room.Name, roomListEntryGameObject);
        }
    }
    public override void OnLeftLobby()
    {
        ClearRoomList();
        catchedRoomList.Clear();
    }
    #endregion

    #region  Private Methods
    void OnJoinRoomButtonClicked(string _roomName)
    {
        if(PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(_roomName);
        
    }

    void ClearRoomList()
    {
        foreach(var roomListGameObject in roomListGameObjects.Values)
        {
            Destroy(roomListGameObject);
        }
        roomListGameObjects.Clear();
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
