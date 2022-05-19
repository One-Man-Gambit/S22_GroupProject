using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class Photon_Test_Manager : MonoBehaviourPunCallbacks
{
    string gameVersion = "v0.0.3";

    [Tooltip("The maximum number of players per room.  When a room is full, it can't be joined by new players, and so a new room will be created.")]
    [SerializeField]
    
    const string playerNamePrefKey = "PlayerName";

    [Header("Lobby Data")]
    List<RoomInfo> Lobby_RoomInfo_List = new List<RoomInfo>();

    [Header("UI Panels")]
    public GameObject Current_Panel;
    
    public GameObject Status_Panel, Login_Panel, Lobby_Panel, Creation_Panel, Room_Panel;

    [Header("Status Panel")]
    public TMP_Text Status_VersionDisplay;
    public TMP_Text Status_StatusDisplay;

    [Header("Login Panel")]
    public TMP_InputField Login_NameField;
    public TMP_Text Login_ErrorDisplay;

    [Header("Lobby Panel")]
    public TMP_Text Lobby_PlayerName;
    public TMP_Text Lobby_Name;
    public GameObject Lobby_RoomListing_Prefrab;
    public GameObject Lobby_RoomList_Content;

    [Header("Room Creation Panel")]
    public byte Creation_MaxPlayers = 6;
    public TMP_InputField Creation_NameInput;
    public TMP_Text Creation_ErrorDisplay;
    public TMP_Dropdown Creation_MaxPlayerDropdown;
    const string MAP_PROP_KEY = "map";
    const string GAME_MODE_PROP_KEY = "mode";
    

    [Header("Room Panel")]     
    public TMP_Text Room_Name;
    public List<GameObject> Room_PlayerPanels = new List<GameObject>();
    public List<TMP_Dropdown> Room_PlayerFloatySelector = new List<TMP_Dropdown>();
    public List<TMP_Dropdown> Room_PlayerColorSelector = new List<TMP_Dropdown>();
    public List<TMP_Text> Room_PlayerListings = new List<TMP_Text>();
    public List<TMP_Text> Room_PlayerReady = new List<TMP_Text>();


    #region MonoBehaviour

    private void Awake() 
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        Status_Panel.SetActive(true);
        Login_Panel.SetActive(false);
        Lobby_Panel.SetActive(false);
        Room_Panel.SetActive(false);
        Creation_Panel.SetActive(false);
        Room_Panel.SetActive(false);
    }

    private void Start() 
    {   
        // Grab the player's default name and assign it as default nickname on launch.
        string defaultName = string.Empty;
        if (Login_NameField != null) 
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                Login_NameField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;

        // Attempt to connect to server on launch.
        ConnectToPhoton();
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
	{
        Debug.Log("Connected Successfully to Photon");
        SetPanel(Login_Panel);

        // Load Player Defaults & Custom Properties
        int floatyID = PlayerPrefs.GetInt("Floaty", 0);
        int colorID = PlayerPrefs.GetInt("Color", 0);
        
        ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
        ht.Add("Floaty", floatyID);
        ht.Add("Color", colorID);
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
        Debug.LogWarningFormat("Disconnected from Photon, reason {0}", cause);
        SetPanel(Login_Panel);
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("Connected Successfully to Lobby " + ((PhotonNetwork.CurrentLobby.IsDefault) ? "Default" : PhotonNetwork.CurrentLobby.Name));

        // Refresh Room List
        RefreshLobbyRooms();
    }

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
        Lobby_RoomInfo_List = roomList;
	}

	public override void OnCreatedRoom()
	{
		RefreshLobbyRooms();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
        Debug.Log("Failed to Join Random Room.");  
    }

	public override void OnJoinedRoom()
	{
        Debug.Log("OnJoinedRoom() called by PUN.  Now this client is in a room. \nUpdating Player List.");        
        SetPanel(Room_Panel);

        // Update the player list
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdatePlayerList", RpcTarget.All);
    }

	public override void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom() called by PUN.  This client is no longer in a room.");
        //RoomPanel.SetActive(false);
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
        Debug.Log("OnPlayerEnteredRoom() called by PUN. \nUpdating Player List.");
        
        // Update the player list
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdatePlayerList", RpcTarget.All);
	}

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom() called by PUN.  \nUpdating Player List.");
        
        // Update the player list
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdatePlayerList", RpcTarget.All);
    }

    #endregion

    #region Photon Functionality

    public void ConnectToPhoton()
    {   
        // Set the App Version.
        PhotonNetwork.GameVersion = gameVersion;
        
        // Connect to Master Server
        Debug.Log("Connecting to Photon.");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void SetPlayerName(string value)
    {
        // Disable login error display if it's active.
        if (Login_ErrorDisplay.gameObject.activeInHierarchy) {
            Login_ErrorDisplay.gameObject.SetActive(false);
        }

        // Set the nickname and player prefs.
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
    
    public void Login() 
    {   
        // If name is blank, display an error to notify player
        if (string.IsNullOrEmpty(Login_NameField.text)) 
        {            
            Login_ErrorDisplay.gameObject.SetActive(true);
            Login_ErrorDisplay.text = "Name cannot be left blank.";
            return;
        }

        PhotonNetwork.JoinLobby(TypedLobby.Default);
        SetPanel(Lobby_Panel);
    }

    public void Logout() 
    {
        PhotonNetwork.LeaveLobby();
        SetPanel(Login_Panel);
    }

    public void RefreshLobbyRooms()
    {   
        //==============================================================
        // Clear Room Listings
        //==============================================================
        foreach (Transform child in Lobby_RoomList_Content.transform) 
        {
            GameObject.Destroy(child.gameObject);
        }

        // Populate Room Listings
        //==============================================================
        foreach (var room in Lobby_RoomInfo_List)
        {
            // Instantiation and Reference
            GameObject roomListing = GameObject.Instantiate(Lobby_RoomListing_Prefrab);
            Photon_Test_Room_Data roomData = roomListing.GetComponent<Photon_Test_Room_Data>();

            // Positioning
            roomListing.transform.SetParent(Lobby_RoomList_Content.transform);
            roomListing.transform.localScale = Vector3.one;

            // Button Event
            roomData.ButtonEvent.onClick.AddListener(delegate() { 
                PhotonNetwork.JoinRoom(room.Name);
            });

            // Updating Visible Information
            roomData.Identity.text = room.Name;
            roomData.Data.text = "<Game Mode> - <Map>" + " - " + room.PlayerCount.ToString() + " / " + room.MaxPlayers.ToString(); 
        
        }
    }

    public void RefreshPlayersListings()
    {
        // //==============================================================
        // // Clear Player Listings
        // //==============================================================
        // foreach (Transform child in Lobby_PlayerList_Content.transform) 
        // {
        //     GameObject.Destroy(child.gameObject);
        // }

        // // Populate Player Listings
        // //==============================================================
        // foreach (var player in PhotonNetwork.CurrentRoom.Players)
        // {   
        //     // Instantiation and Reference
        //     GameObject playerListing = GameObject.Instantiate(Lobby_PlayerListing_Prefab);

        //     // Positioning
        //     playerListing.transform.SetParent(Lobby_PlayerList_Content.transform);
        //     playerListing.transform.localScale = Vector3.one;

        //     // Updating Visible Information
        //     playerListing.GetComponent<TMP_Text>().text = player.Value.NickName;            
        // }
    }

    public void QuickMatch()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom() 
    {
        // If name is blank, display an error to notify player
        if (string.IsNullOrEmpty(Creation_NameInput.text)) 
        {            
            Creation_ErrorDisplay.gameObject.SetActive(true);
            Creation_ErrorDisplay.text = "Name cannot be left blank.";
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = Creation_MaxPlayers;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { {MAP_PROP_KEY, "Sandbox"}, { GAME_MODE_PROP_KEY, "Testing"} };
        PhotonNetwork.CreateRoom(Creation_NameInput.text, roomOptions, TypedLobby.Default);
    }

    public void LeaveRoom() 
    {
        PhotonNetwork.LeaveRoom();
    }

	public void SetPlayerColor()
    {
        // Set Player Settings to Current Selection
        int index = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players) 
        {
            // Enable the ready tag for only the local player
            if (player.Value == PhotonNetwork.LocalPlayer) 
            {    
                int newColor = Room_PlayerColorSelector[index].value;
                PlayerPrefs.SetInt("Color", newColor);                
                ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
                ht.Add("Color", newColor);
                PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
            }
            
            index++;
        } 

        // Update the player list
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdatePlayerList", RpcTarget.Others);
    }

    public void SetPlayerFloaty()
    {
        // Set Player Settings to Current Selection
        int index = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players) 
        {
            // Enable the ready tag for only the local player
            if (player.Value == PhotonNetwork.LocalPlayer) 
            {    
                
                int newFloaty = Room_PlayerFloatySelector[index].value;
                Debug.Log("New Floaty:" + newFloaty);

                PlayerPrefs.SetInt("Floaty", newFloaty);                
                ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
                ht.Add("Floaty", newFloaty);
                PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
            }

            index++;
        } 

        // Update the player list
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdatePlayerList", RpcTarget.Others);
    }

    public void SetPlayerReady()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("SetReady", RpcTarget.All);
    }    

    [PunRPC]
    private void SetReady()
    {   
        // Loop through list of players in the room.
        int index = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players) 
        {
            // Enable the ready tag for only the local player
            if (player.Value == PhotonNetwork.LocalPlayer) {
                Room_PlayerReady[index].gameObject.SetActive(!Room_PlayerReady[index].gameObject.activeInHierarchy);
            } 

            index++;
        } 

        // Update the player list
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdatePlayerList", RpcTarget.All);
    }

    [PunRPC]
    private void UpdatePlayerList()
    {
        // Toggle off all Player Panels
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            Room_PlayerPanels[i].SetActive(false);
        }


        // Loop through list of players in the room.
        int index = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players) 
        {
            // Enable Player Panels for Each Player in the room
            Room_PlayerPanels[index].SetActive(true);

            //Debug.Log((int)player.Value.CustomProperties["Floaty"]);
            Room_PlayerFloatySelector[index].value = (int)player.Value.CustomProperties["Floaty"];
            Room_PlayerColorSelector[index].value = (int)player.Value.CustomProperties["Color"];

            // Disable Load Selectors for all players except the local player.
            if (player.Value != PhotonNetwork.LocalPlayer)
            {
                Room_PlayerFloatySelector[index].interactable = false;
                Room_PlayerColorSelector[index].interactable = false;
            }

            // Make sure to write the nickname of the player as well.
            Room_PlayerListings[index].text = player.Value.NickName; 

            index++;
        } 

        Debug.Log("Player List Updated");
    }

    #endregion

    #region Client Functionality

    public void SetPanel(GameObject panel) 
    {
        // Disable current Panel
        if (Current_Panel != null) {
            Current_Panel.SetActive(false);

            if (Current_Panel.name == "Lobby Panel") {
                CloseCreationPanel();
            }
        }        

        // Assign the new panel as current and enable it.
        Current_Panel = panel;
        Current_Panel.SetActive(true);        
    }

    public void OpenCreationPanel() 
    {
        Creation_Panel.SetActive(true);
        Creation_NameInput.text = "";
        Creation_ErrorDisplay.gameObject.SetActive(false);
    }

    public void ChangeMaxPlayerCount()
    {
        TMP_Dropdown.OptionData test = Creation_MaxPlayerDropdown.options[Creation_MaxPlayerDropdown.value];
        int max_int = int.Parse(test.text);
        Creation_MaxPlayers = (byte)max_int;
    }

    public void CloseCreationPanel()
    {
        Creation_Panel.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else 
            Application.Quit();
        #endif
    }

    #endregion

    
}
