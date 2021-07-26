using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    public GameObject box;

    public void CreateRoom()
    {
        box.GetComponent<Box>().Pleer = true;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createInput.text, options);
    }

    public void JoinRoom()
    {
        box.GetComponent<Box>().Pleer = false;
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        DontDestroyOnLoad(box);
        GameObject.Find("AMusic").GetComponent<AudioSource>().Stop();
        PhotonNetwork.LoadLevel("Game");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
