using UnityEngine;
using Photon.Pun;

public class Spawn : MonoBehaviour
{
    [Header("Доска")]
    public GameObject[] BoardCubes = new GameObject[2];
    private float BoardCubeSize;

    [Header("Фигуры")]
    public GameObject[] Figurs = new GameObject[12];
    //public GameObject Manager;
    public ManagerGame managerGame;

    [Header("Игрок")]
    public GameObject Pleer;

    private Box box;
    private BoxBoardCubes boxBoardCubes;
    private BoxFigurs boxFigurs;

    private void Start()
    {
        box = GameObject.Find("Box").GetComponent<Box>();
        boxBoardCubes = GameObject.Find("Box").GetComponent<BoxBoardCubes>();
        boxFigurs = GameObject.Find("Box").GetComponent<BoxFigurs>();
        ///managerGame = Manager.GetComponent<ManagerGame>();
        BoardCubeSize = BoardCubes[0].transform.localScale.x;
        
        //bool switchCubeColor = box.Pleer;
        bool switchCubeColor = true;
        int switchCubeColorBoolInt;

        GameObject boardCube;
        for (int y = 0; y < 8; y++)
        {
            if (switchCubeColor) switchCubeColorBoolInt = 0;
            else switchCubeColorBoolInt = 1;

            GameObject figureInstantiate;
            for (int x = 0; x < 8; x++)
            {
                Vector3 local = new Vector3(x, 0, y);
                if (x % 2 == switchCubeColorBoolInt)
                    boardCube = Instantiate(BoardCubes[0], local, Quaternion.identity);
                else
                    boardCube = Instantiate(BoardCubes[1], local, Quaternion.identity);
                boardCube.transform.localScale = new Vector3(boardCube.transform.localScale.x, boardCube.transform.localScale.y, boardCube.transform.localScale.z);

                if (x == 0 && y == 0)
                {
                    boxBoardCubes.x = (int)boardCube.transform.position.x;
                    boxBoardCubes.y = (int)boardCube.transform.position.z;
                }

                if (x == 0 && y == 1)
                {
                    boxBoardCubes.up = (int)boardCube.transform.position.z - boxBoardCubes.y;
                }

                if (x == 1 && y == 0)
                {
                    boxBoardCubes.right = (int)boardCube.transform.position.x - boxBoardCubes.x;
                }

                int i = box.Pleer ? -1 : 1;
                int j = box.Pleer ? 7 : 0;
                foreach (GameObject figure in Figurs)
                    if (managerGame.game.Placement[7 - y * i - j, 7 - x * i - j] == figure.name[0])
                    {
                        
                        //figureInstantiate = Instantiate(figure, new Vector3(ZeroCoordinate.position.x + x * Xk, figure.transform.position.y, ZeroCoordinate.position.z + y * Yk), figure.transform.rotation);
                        figureInstantiate = Instantiate(figure, new Vector3(x, figure.transform.position.y, y), figure.transform.rotation);
                        figureInstantiate.name = figure.name[0].ToString();
                        if (box.Pleer)
                        {
                            if (figure.name[0] == 'n')
                            {
                                figureInstantiate.transform.rotation = Quaternion.identity;
                            }
                        }
                        else
                        {
                            if (figure.name[0] == 'N')
                            {
                                figureInstantiate.transform.rotation = Quaternion.identity;
                            }
                        }
                        if (box.Pleer)
                        {
                            if (x == 0 && y == 0)
                            {
                                boxFigurs.x = (int)figureInstantiate.transform.position.x;
                                boxFigurs.y = (int)figureInstantiate.transform.position.z;
                            }

                            if (x == 0 && y == 1)
                            {
                                boxFigurs.up = (int)figureInstantiate.transform.position.z - boxFigurs.y;
                            }

                            if (x == 1 && y == 0)
                            {
                                boxFigurs.right = (int)figureInstantiate.transform.position.x - boxFigurs.x;
                            }
                        }
                        else
                        {
                            if (x == 7 && y == 7)
                            {
                                boxFigurs.x = (int)figureInstantiate.transform.position.x;
                                boxFigurs.y = (int)figureInstantiate.transform.position.z;
                            }

                            if (x == 7 && y == 6)
                            {
                                boxFigurs.up = (int)figureInstantiate.transform.position.z - boxFigurs.y;
                            }

                            if (x == 6 && y == 7)
                            {
                                boxFigurs.right = (int)figureInstantiate.transform.position.x - boxFigurs.x;
                            }
                        }

                        break;
                    }

            }
            switchCubeColor = !switchCubeColor;

        }

        PhotonNetwork.Instantiate(Pleer.name, new Vector3(0,0,0), Quaternion.identity);
    }
}
