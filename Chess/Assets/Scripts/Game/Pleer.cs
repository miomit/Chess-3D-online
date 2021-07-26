using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pleer : MonoBehaviour, IPunObservable
{
    //для отправки данных
    PhotonView View;
    bool SEND = false;
    string SendCoordinates;
    string ReadCoordinates;

    private bool bKillFigure = false;

    private ManagerGame managerGame;
    private Box box;
    private BoxBoardCubes boxBoardCubes;
    private BoxFigurs boxFigurs;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (SEND)
            {
                Debug.LogError("SEND");
                stream.SendNext(SendCoordinates);
                stream.SendNext(bKillFigure);
                SEND = false;
                bKillFigure = false;
            }
        }
        else
        {
            string ReadCoordinates = stream.ReceiveNext().ToString();
            Debug.LogError("Read: " + ReadCoordinates);
            bool ReadKillFigure = (bool)stream.ReceiveNext();
            managerGame.ProcessTheCoordinates(ReadCoordinates, ReadKillFigure);
            if (ReadKillFigure)
                GameObject.Find("AKill").GetComponent<AudioSource>().Play();
            else
                GameObject.Find("AMove").GetComponent<AudioSource>().Play();

            string error = managerGame.MoveOptionFigure(ReadCoordinates);
            if (error != "OK")
                Debug.LogError(error);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        View = GetComponent<PhotonView>();
        managerGame = GameObject.Find("ManagerGame").GetComponent<ManagerGame>();
        box = GameObject.Find("Box").GetComponent<Box>();
        boxBoardCubes = GameObject.Find("Box").GetComponent<BoxBoardCubes>();
        boxFigurs = GameObject.Find("Box").GetComponent<BoxFigurs>();

        print("ZeroCube x: " + boxBoardCubes.x + "; y: " + boxBoardCubes.y);
        print("ZeroCube up: " + boxBoardCubes.up + "; right: " + boxBoardCubes.right);
        print("---------------------------------------------------------------------");
        print("ZeroFigurs x: " + boxBoardCubes.x + "; y: " + boxBoardCubes.y);
        print("ZeroFigurs up: " + boxBoardCubes.up + "; right: " + boxBoardCubes.right);
    }

    // Update is called once per frame
    void Update()
    {
        if (View.IsMine)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        int x = managerGame.ConvertToPlayerCoordinates((int)hit.collider.gameObject.transform.position.x + 1);
                        int y = managerGame.ConvertToPlayerCoordinates((int)hit.collider.gameObject.transform.position.z + 1);
                        print(hit.collider.gameObject.name);
                        print("x: " + managerGame.game.ToCharAH(x) + "; y: " + y);

                        if (managerGame.game.CheckWhiteFigure(hit.collider.gameObject.name[0]) == box.Pleer && hit.collider.gameObject.tag == "Figure")
                        {
                            managerGame.Figure = hit.collider.gameObject;
                            string error = managerGame.MovePossible(managerGame.game.ToCharAH(x).ToString() + y.ToString());
                            GameObject.Find("ASelect").GetComponent<AudioSource>().Play();
                            if (error != "OK")
                                Debug.LogError(error);
                            else
                                SendCoordinates = managerGame.game.ToCharAH(x).ToString() + y.ToString();
                        }

                        bool checkKillFigure = managerGame.CheckFigureOnKill(hit.collider.gameObject.transform) && hit.collider.gameObject.tag == "Figure";

                        if (hit.collider.gameObject.tag == "MoveOption" || hit.collider.gameObject.tag == "MoveKill" || checkKillFigure)
                        {
                            if (hit.collider.gameObject.tag == "MoveKill" || checkKillFigure)
                            {
                                GameObject.Find("AKill").GetComponent<AudioSource>().Play();
                                foreach (GameObject figure in GameObject.FindGameObjectsWithTag("Figure"))
                                {
                                    if (managerGame.ConvertToPlayerCoordinatesServer((int)figure.transform.position.x) == x - 1)
                                        if (managerGame.ConvertToPlayerCoordinatesServer((int)figure.transform.position.z) == y - 1)
                                        {
                                            Destroy(figure);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                GameObject.Find("AMove").GetComponent<AudioSource>().Play();
                            }

                            string error = managerGame.MoveOptionFigure(managerGame.game.ToCharAH(x).ToString() + y.ToString(), x - 1, y - 1);
                            if (error != "OK")
                                Debug.LogError(error);
                            else
                            {
                                if (hit.collider.gameObject.tag == "MoveKill" || checkKillFigure)
                                    bKillFigure = true;
                                SendCoordinates += managerGame.game.ToCharAH(x).ToString() + y.ToString();
                                SEND = true;
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                SEND = true;
            }
        }

    }
}
