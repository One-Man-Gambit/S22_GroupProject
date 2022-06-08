using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using Fusion;

public class Fusion_Test_Manager : MonoBehaviour//, INetworkRunnerCallbacks
{   
    // /////////////////////////////////////////////////////////////////////////////////////
    // // FUSION
    // /////////////////////////////////////////////////////////////////////////////////////

    // string gameVersion = "v0.0.4";

    // NetworkRunner _runner;
    

    // [SerializeField] private NetworkPrefabRef _playerPrefab;
    // private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    // /////////////////////////////////////////////////////////////////////////////////////

    
    // #region MonoBehaviour

    // private void OnGUI() 
    // {
    //     if (_runner == null) 
    //     {
    //         if (GUI.Button(new Rect(0, 0, 200, 40), "HOST"))
    //         {
    //             StartGame(GameMode.Host);
    //         }

    //         if (GUI.Button(new Rect(0, 40, 200, 40), "JOIN"))
    //         {
    //             StartGame(GameMode.Client);
    //         }
    //     }
    // }

    // #endregion

    // #region Fusion Methods

    // async void StartGame(GameMode mode)
    // {
    //     // Create the Fusion runner and let it know that we will be providing user input
    //     _runner = gameObject.AddComponent<NetworkRunner>();
    //     _runner.ProvideInput = true;

    //     // Start or join (depends on gamemode) a session with a specific name
    //     await _runner.StartGame(new StartGameArgs()
    //     {
    //         GameMode = mode,
    //         SessionName = "Test Room",
    //         Scene = SceneManager.GetActiveScene().buildIndex,
    //         SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
    //     });
    // }

    // #endregion

    // #region Fusion Callbacks

    // // Callback from a NetworkRunner when a new player has joined
    // public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    // {
    //     // Create a unique position for the player
    //     Vector3 spawnPosition = new Vector3((player.RawEncoded%runner.Config.Simulation.DefaultPlayers)*3, 1, 0);
    //     NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity);

    //     // Keep track of the player avatars so we can remove it when they disconnect
    //     _spawnedCharacters.Add(player, networkPlayerObject);
    // }

    // // Callback from a NetworkRunner when a player has disconnected.
    // public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    // {
    //     // Find and remove the players avatar
    //     if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
    //     {
    //         runner.Despawn(networkObject);
    //         _spawnedCharacters.Remove(player);
    //     }
    // }

    // // Callback from NetworkRunner that polls for user inputs. 
    // public void OnInput(NetworkRunner runner, NetworkInput input)
    // {
    //     var data = new NetworkInputData();

    //     if (Input.GetKey(KeyCode.W)) data.direction += Vector3.forward;
    //     if (Input.GetKey(KeyCode.S)) data.direction += Vector3.back;
    //     if (Input.GetKey(KeyCode.A)) data.direction += Vector3.left;
    //     if (Input.GetKey(KeyCode.D)) data.direction += Vector3.right;

    //     input.Set(data);

    // }

    // public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    // {

    // }

    // // Callback when the runner is shutdown.
    // public void OnShutdown(NetworkRunner runner, ShutdownReason shotdownReason) 
    // {

    // }

    // // Callback when a NetworkRunner successfully connects to a server or host.
    // public void OnConnectedToServer(NetworkRunner runner) 
    // {

    // }

    // // Callback when a NetworkRunner disconnects from a server or host.
    // public void OnDisconnectedFromServer(NetworkRunner runner) 
    // {

    // }

    // // Callback for when NetworkRunner receives a Connection Request from a Remote Client
    // public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) 
    // {

    // }

    // // Callback for when NetworkRunner fails to connect to a server or host.
    // public void OnConnectFailed(NetworkRunner runner, Fusion.Sockets.NetAddress remoteAddress, Fusion.Sockets.NetConnectFailedReason reason)
    // {
        
    // }

    // // This callback is invoked when a manually dispatched simulation message is received from a remote peer.
    // public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    // {

    // }

    // // This callback is invoked when a new List of Sessions is received from the Photon Cloud
    // public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    // {

    // }

    // // Callback is invoked when the Authentication procedure returns a response from the Authentication Server
    // public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    // {

    // }

    // // Callback is invoked when the Host Migration process has started
    // public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    // {

    // }

    // public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data)
    // {
        
    // }

    // public void OnSceneLoadDone(NetworkRunner runner) 
    // {

    // }

    // public void OnSceneLoadStart(NetworkRunner runner)
    // {

    // }

    // #endregion

    
}
