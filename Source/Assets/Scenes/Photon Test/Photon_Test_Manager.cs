using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class Photon_Test_Manager : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";

    [Tooltip("The maximum number of players per room.  When a room is full, it can't be joined by new players, and so a new room will be created.")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    const string playerNamePrefKey = "PlayerName";

    [Header("UI")]
    public GameObject ServerConnectionPanel;
    public GameObject RoomConnectionPanel;
    public TMP_InputField nameInput;
    public GameObject RoomPanel;
    public TMP_Text roomName;
    public List<TMP_Text> playerNames = new List<TMP_Text>();


    private void Awake() 
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    
        ServerConnectionPanel.SetActive(true);
        RoomConnectionPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }

    private void Start() 
    {
        string defaultName = string.Empty;
        if (nameInput != null) 
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                nameInput.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    public void SetPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value)) 
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }

        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    public void Connect() 
    {
        if (PhotonNetwork.IsConnected) 
        {
            PhotonNetwork.JoinRandomRoom();
        }    
        else 
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public void LeaveRoom() 
    {
        PhotonNetwork.LeaveRoom();
    }

	public override void OnConnectedToMaster()
	{
        Debug.Log("OnConnectedToMaster() was called by PUN");

        ServerConnectionPanel.SetActive(false);
        RoomConnectionPanel.SetActive(true);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
        ServerConnectionPanel.SetActive(true);
        RoomConnectionPanel.SetActive(false);
        RoomPanel.SetActive(false);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
        Debug.Log("OnJoinRandomFailed() was called by PUN.  No random room available, so we create one.\n Calling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });        
    }

	public override void OnJoinedRoom()
	{
        Debug.Log("OnJoinedRoom() called by PUN.  Now this client is in a room. \nUpdating Player List.");
        RoomConnectionPanel.SetActive(false);
        RoomPanel.SetActive(true);
        roomName.text = "You are now in room: \n" + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

	public override void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom() called by PUN.  This client is no longer in a room.");
        RoomPanel.SetActive(false);
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
        Debug.Log("OnPlayerEnteredRoom() called by PUN. \nUpdating Player List.");
        UpdatePlayerList();
	}

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom() called by PUN.  \nUpdating Player List.");
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        for (int i = 0; i < playerNames.Count; i++) 
        {
            if (PhotonNetwork.PlayerList.Length >= (i+1)) 
            {
                playerNames[i].text = "Player " + (i+1) + ": " + PhotonNetwork.PlayerList[i].NickName;
            }
            
            else 
            {
                playerNames[i].text = "Player " + (i+1) + ": ";
            }
            
        }
    }
}
