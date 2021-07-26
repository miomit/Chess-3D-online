using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        if (!GameObject.Find("AMusic").GetComponent<AudioSource>().isPlaying)
            GameObject.Find("AMusic").GetComponent<AudioSource>().Play(1);
    }

    public void Play()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
