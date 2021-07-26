using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToServet : MonoBehaviourPunCallbacks
{
    void Start()
    {
        GameObject Audio = GameObject.Find("Audio");
        DontDestroyOnLoad(Audio);
        PhotonNetwork.ConnectUsingSettings();
        GameObject.Find("AConnect").GetComponent<AudioSource>().Play();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Menu");
    }
}
