using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Fusion_Test_Gameplay_Manager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public List<Transform> SpawnPoint = new List<Transform>();

    public static GameObject LocalPlayerInstance;
    public List<GameObject> Players = new List<GameObject>();

    PhotonView pv;

    private void Start()
    {
        pv = PhotonView.Get(this);

        if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait() 
    {
        // Make sure everyone is in
        yield return new WaitForSeconds(3);
        pv.RPC("SpawnPlayer", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC] private void SpawnPlayer(string name)
    {
        int id = PhotonNetwork.LocalPlayer.ActorNumber;
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", this.PlayerPrefab.name), SpawnPoint[(id == -1) ? 0 : id % SpawnPoint.Count].position, Quaternion.identity, 0);         
    }  
}
