using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class ManagerGame : MonoBehaviourPunCallbacks
{
    public Game game;
    private Box box;

    public GameObject MoveOption;
    public GameObject MoveKill;

    [SerializeField] public GameObject Figure;
    [SerializeField] private GameObject[] Figurs;

    public GameObject[] QueenFigurs = new GameObject[2];

    int MoveX = 0, MoveY = 3;

    void Start()
    {
        box = GameObject.Find("Box").GetComponent<Box>();
        game = new Game();
    }

    public void Menu()
    {
        PhotonNetwork.LeaveRoom();
        Destroy(box.gameObject);
        PhotonNetwork.LoadLevel("menu");
    }

    public void ProcessTheCoordinates(string placementFigure, bool kill)
    {
        Figurs = GameObject.FindGameObjectsWithTag("Figure");

        placementFigure = placementFigure.ToLower();
        print(placementFigure);
        int x = game.GetNumber(placementFigure[0]), y = game.GetNumber(placementFigure[1]);

        MoveX = game.GetNumber(placementFigure[2]);
        MoveY = game.GetNumber(placementFigure[3]);

        print("x: " + x + "; y: " + y + "; MoveX: " + MoveX + "; MoveY: " + MoveY);

        foreach (GameObject figure in Figurs)
        {
            if (ConvertToPlayerCoordinatesServer((int)figure.transform.position.x) == x)
                if (ConvertToPlayerCoordinatesServer((int)figure.transform.position.z) == y)
                {
                    print(figure.name);
                    Figure = figure;
                    break;
                }
        }
        
        if (kill)
            foreach (GameObject figure in Figurs)
            {
                if (ConvertToPlayerCoordinatesServer((int)figure.transform.position.x) == MoveX)
                    if (ConvertToPlayerCoordinatesServer((int)figure.transform.position.z) == MoveY)
                    {
                        Destroy(figure);
                        break;
                    }
            }
    }

    public string MovePossible(string placementFigure)
    {
        string error = game.MovePossible(placementFigure);

        DestroyAll("MoveOption");
        DestroyAll("MoveKill");

        GameObject moveOjc;
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                int i = box.Pleer ? -1 : 1;
                int j = box.Pleer ? 7 : 0;

                if (game.Possible[y, x] == '·')
                {
                    moveOjc = Instantiate(MoveOption, new Vector3(7 - x * i - j, MoveOption.transform.position.y, 7 - y * i - j), MoveOption.transform.rotation);
                    moveOjc.name = "MoveOption";

                }

                if (game.Possible[y, x] == 'x')
                {
                    moveOjc = Instantiate(MoveKill, new Vector3(7 - x * i - j, MoveOption.transform.position.y, 7 - y * i - j), MoveOption.transform.rotation);
                    moveOjc.name = "MoveKill";

                }
            }

        return error;
    }

    public string MoveOptionFigure(string placementFigure)
    {
        string error = game.Move(placementFigure, true);



        if (error == "OK" || error == "UPDATE")
        {
            int i = box.Pleer ? -1 : 1;
            int j = box.Pleer ? 7 : 0;
            Vector3 local = new Vector3(7 - MoveX * i - j, Figure.transform.position.y, 7 - MoveY * i - j);
            if (error == "UPDATE")
            {
                Destroy(Figure);
                local = new Vector3(7 - MoveX * i - j, QueenFigurs[0].transform.position.y, 7 - MoveY * i - j);
                if (!box.Pleer)
                    Figure = Instantiate(QueenFigurs[0], local, Quaternion.identity);
                else
                    Figure = Instantiate(QueenFigurs[1], local, Quaternion.identity);
                Figure.name = Figure.name[0].ToString();
            }
            else {
                Figure.transform.position = local;
            }
        }

        if (error == "UPDATE") error = "OK";

        return error;
    }

    public string MoveOptionFigure(string placementFigure, int x, int y)
    {
        string error = game.Move(placementFigure, false);

        DestroyAll("MoveOption");
        DestroyAll("MoveKill");

        if (error == "OK" || error == "UPDATE")
        {
            int i = box.Pleer ? -1 : 1;
            int j = box.Pleer ? 7 : 0;
            Vector3 local = new Vector3(7 - x * i - j, Figure.transform.position.y, 7 - y * i - j);
            if (error == "UPDATE")
            {
                Destroy(Figure);
                if (box.Pleer)
                    Figure = Instantiate(QueenFigurs[0], local, Quaternion.identity);
                else
                    Figure = Instantiate(QueenFigurs[1], local, Quaternion.identity);
                Figure.name = Figure.name[0].ToString();
            }
            else
            {
                Figure.transform.position = local;
            }
        }
        if (error == "UPDATE") error = "OK";

        return error;
    }

    public bool CheckFigureOnKill(Transform figure)
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
            {
                int i = box.Pleer ? -1 : 1;
                int j = box.Pleer ? 7 : 0;

                if (game.Possible[y, x] == 'x')
                {
                    if (x == 7 - (int)figure.position.x * i - j && y == 7 - (int)figure.position.z * i - j)
                        return true;
                }
            }
        return false;
    }

    public void DestroyAll(string tag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }
    }

    public int ConvertToPlayerCoordinates(int c)
    {
        if (box.Pleer)
            return c;
        else
            return 9 - c;
    }

    public int ConvertToPlayerCoordinatesServer(int c)
    {
        if (box.Pleer)
            return c;
        else
            return 7 - c;
    }
}
