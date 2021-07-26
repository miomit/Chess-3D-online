using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game 
{
    public char[,] Placement { get; private set; } = new char[8, 8];
    public char[,] Possible { get; private set; } = new char[8, 8];

    private int GameMode = 0;
    public bool WhiteMove { get; private set; } = true;

    public string EatenWhite { get; private set; } = "";
    public string EatenBlack { get; private set; } = "";

    private string PlacementFigure;

    private void Swap<T>(ref T v1, ref T v2) { T v3 = v1; v1 = v2; v2 = v3; }

    public Game(string placementFenStart = "RNBQKBNR/PPPPPPPP/8/8/8/8/pppppppp/rnbqkbnr w KQkq - 0 1")
    {
        FenToArray(placementFenStart);

        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                Possible[y, x] = ' ';
    }

    private void FenToArray (string fen)
    {
        string[] fenArray = fen.Split(' ');
        string[] placementFigure = fenArray[0].Split('/');
        //Console.WriteLine(placementFigure[0]);
        for (int y = 0; y < 8; y++)
        {
            int x = 0;
            foreach (char figure in placementFigure[y])
            {
                if (!CheckNumber(figure))
                {
                    Placement[y, x] = figure;
                    x += 1;
                }
                else
                {
                    for (int i = 0; i < (int)Char.GetNumericValue(figure); i++)
                    {
                        Placement[y, x] = '·';
                        x += 1;
                    }
                }
            }
        }

        if (fenArray[1].ToLower() == "w")
            WhiteMove = true;
        else
            WhiteMove = false;

    }

    private bool CheckNumber (char n)
    {
        switch (n)
        {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8': return true;
            default: return false;
        }
    }

    public int GetNumber (char n)
    {
        switch (n)
        {
            case 'a': return 0;
            case 'b': return 1;
            case 'c': return 2;
            case 'd': return 3;
            case 'e': return 4;
            case 'f': return 5;
            case 'g': return 6;
            case 'h': return 7;

            case '1': return 0;
            case '2': return 1;
            case '3': return 2;
            case '4': return 3;
            case '5': return 4;
            case '6': return 5;
            case '7': return 6;
            case '8': return 7;

            default: return -1;
        }
    }

    public bool CheckWhiteFigure (char n)
    {
        switch (n)
        {
            case 'R':
            case 'N':
            case 'B':
            case 'K':
            case 'Q':
            case 'P': return true;
            default: return false;
        }
    }

    public string Move(string placementCoordinates, bool FullCoordinates)
    {
            
        string status = Status();

        if (status != "OK")
        {
            ResetPossible();
            return status;
        }

        if (!FullCoordinates)
            placementCoordinates = PlacementFigure + placementCoordinates;

        placementCoordinates = placementCoordinates.ToLower();

        Debug.Log(placementCoordinates);

        int x1, x2;
        int y1, y2;

        //x1 and x2
        if (GetNumber(placementCoordinates[0]) != -1) x1 = GetNumber(placementCoordinates[0]); else return "there is no such coordinate (x1 == -1)";
        if (GetNumber(placementCoordinates[2]) != -1) x2 = GetNumber(placementCoordinates[2]); else return "there is no such coordinate (x2 == -1)";

        //y1 and y2
        if (CheckNumber(placementCoordinates[1])) y1 = (int)Char.GetNumericValue(placementCoordinates[1]); else return "there is no such coordinate (y1 not in [1,8]";
        if (CheckNumber(placementCoordinates[3])) y2 = (int)Char.GetNumericValue(placementCoordinates[3]); else return "there is no such coordinate (y2 not in [1,8]";

        if (Possible[y2 - 1, x2] != '·' && Possible[y2 - 1, x2] != 'x' && !FullCoordinates)
        {
            ResetPossible();
            return "the player can't walk like this";
        }
        ResetPossible();

        /*
        Console.WriteLine("x1 = {0}, x2 = {1}", x1, x2);
        Console.WriteLine("y1 = {0}, y2 = {1}", y1, y2);
        */
        char placement1 = Placement[y1 - 1, x1];
        char placement2 = Placement[y2 - 1, x2];
        string error = "OK";
        if (Placement[y1 - 1, x1] == '·') return "the initial cell is empty";

        if (CheckWhiteFigure(Placement[y1 - 1, x1]) != WhiteMove)
        {
            if (WhiteMove)
                return "Black people don't go";
            else
                return "White people don't go";
        }

        if (Placement[y2 - 1, x2] != '·')
        {
            if (CheckWhiteFigure(Placement[y1 - 1, x1]) == CheckWhiteFigure(Placement[y2 - 1, x2])) {
                return "The player cannot kill his figure";
            }
            else {

                if (WhiteMove)
                    EatenBlack += Placement[y2 - 1, x2].ToString();
                else
                    EatenWhite += Placement[y2 - 1, x2].ToString();
                Placement[y2 - 1, x2] = '·';
            }

        }

        if (y2 == 1 && placement1 == 'p')
        {
            Placement[y1 - 1, x1] = 'q';
            error = "UPDATE";
            Debug.Log("UPDATE");
        }
        if (y2 == 8 && placement1 == 'P')
        {
            Placement[y1 - 1, x1] = 'Q';
            error = "UPDATE";
            Debug.Log("UPDATE");
        }

        Swap(ref Placement[y1 - 1, x1], ref Placement[y2 - 1, x2]);

        if (placement2 == 'k' || placement2 == 'K')
        {
            GameMode = 3;
            return Status();
        }

        WhiteMove = !WhiteMove;
        return error;
    }

    public string MovePossible (string placementFigure)
    {
        int x, y;

        placementFigure = placementFigure.ToLower();

        if (GetNumber(placementFigure[0]) != -1) x = GetNumber(placementFigure[0]); else return "there is no such coordinate (x1 == -1)";

        if (CheckNumber(placementFigure[1])) y = (int)Char.GetNumericValue(placementFigure[1]); else return "there is no such coordinate (y1 not in [1,8]";

        if (Placement[y - 1, x] == '·') return "the initial cell is empty";

        if (CheckWhiteFigure(Placement[y - 1, x]) != WhiteMove)
        {
            if (WhiteMove)
                return "You can't select a black shape";
            else
                return "You can't select a white shape";
        }

        ResetPossible();

        Possible[y - 1, x] = Placement[y - 1, x];

        switch (Placement[y - 1, x].ToString().ToLower())
        {
            case "p":
                if (WhiteMove)
                {
                    if (y - 1 + 1 <= 8) {
                        if (Placement[y - 1 + 1, x] == '·')
                        {
                            Possible[y - 1 + 1, x] = '·';
                            if (y == 2 && Placement[y - 1 + 2, x] == '·')
                                Possible[y - 1 + 2, x] = '·';
                        }
                        if (x - 1 >= 0) 
                            if (Placement[y - 1 + 1, x - 1] != '·' && CheckWhiteFigure(Placement[y - 1 + 1, x - 1]) != WhiteMove)
                                Possible[y - 1 + 1, x - 1] = 'x';
                        if (x + 1 < 8)
                            if (Placement[y - 1 + 1, x + 1] != '·' && CheckWhiteFigure(Placement[y - 1 + 1, x + 1]) != WhiteMove)
                                Possible[y - 1 + 1, x + 1] = 'x';
                    }
                }
                else
                {
                    if (y - 1 - 1 >= 0)
                    {
                        if (Placement[y - 1 - 1, x] == '·')
                        {
                            Possible[y - 1 - 1, x] = '·';
                            if (y == 7 && Placement[y - 1 - 2, x] == '·')
                                Possible[y - 1 - 2, x] = '·';
                        }

                        if (x - 1 > 0)
                            if (Placement[y - 1 - 1, x - 1] != '·' && CheckWhiteFigure(Placement[y - 1 - 1, x - 1]) != WhiteMove)
                                Possible[y - 1 - 1, x - 1] = 'x';
                        if (x + 1 < 8)
                            if (Placement[y - 1 - 1, x + 1] != '·' && CheckWhiteFigure(Placement[y - 1 - 1, x + 1]) != WhiteMove)
                                Possible[y - 1 - 1, x + 1] = 'x';

                    }
                }
                break;
            case "n":
                if (y - 1 + 1 < 8)
                {
                    if (x - 2 >= 0)
                    {
                        if (Placement[y - 1 + 1, x - 2] != '·' && CheckWhiteFigure(Placement[y - 1 + 1, x - 2]) != WhiteMove)
                            Possible[y - 1 + 1, x - 2] = 'x';

                        if (Placement[y - 1 + 1, x - 2] == '·')
                            Possible[y - 1 + 1, x - 2] = '·';
                    }
                    if (x + 2 < 8)
                    {
                        if (Placement[y - 1 + 1, x + 2] != '·' && CheckWhiteFigure(Placement[y - 1 + 1, x + 2]) != WhiteMove)
                            Possible[y - 1 + 1, x + 2] = 'x';

                        if (Placement[y - 1 + 1, x + 2] == '·')
                            Possible[y - 1 + 1, x + 2] = '·';
                    }
                }

                if (y - 1 - 1 >= 0)
                {
                    if (x - 2 >= 0)
                    {
                        if (Placement[y - 1 - 1, x - 2] != '·' && CheckWhiteFigure(Placement[y - 1 - 1, x - 2]) != WhiteMove)
                            Possible[y - 1 - 1, x - 2] = 'x';

                        if (Placement[y - 1 - 1, x - 2] == '·')
                            Possible[y - 1 - 1, x - 2] = '·';
                    }
                    if (x + 2 < 8)
                    {
                        if (Placement[y - 1 - 1, x + 2] != '·' && CheckWhiteFigure(Placement[y - 1 - 1, x + 2]) != WhiteMove)
                            Possible[y - 1 - 1, x + 2] = 'x';

                        if (Placement[y - 1 - 1, x + 2] == '·')
                            Possible[y - 1 - 1, x + 2] = '·';
                    }
                }

                if (y - 1 + 2 < 8)
                {
                    if (x - 1 >= 0)
                    {
                        if (Placement[y - 1 + 2, x - 1] != '·' && CheckWhiteFigure(Placement[y - 1 + 2, x - 1]) != WhiteMove)
                            Possible[y - 1 + 2, x - 1] = 'x';

                        if (Placement[y - 1 + 2, x - 1] == '·')
                            Possible[y - 1 + 2, x - 1] = '·';
                    }

                    if (x + 1 < 8)
                    {
                        if (Placement[y - 1 + 2, x + 1] != '·' && CheckWhiteFigure(Placement[y - 1 + 2, x + 1]) != WhiteMove)
                            Possible[y - 1 + 2, x + 1] = 'x';

                        if (Placement[y - 1 + 2, x + 1] == '·')
                            Possible[y - 1 + 2, x + 1] = '·';
                    }
                }

                if (y - 1 - 2 >= 0)
                {
                    if (x - 1 >= 0)
                    {
                        if (Placement[y - 1 - 2, x - 1] != '·' && CheckWhiteFigure(Placement[y - 1 - 2, x - 1]) != WhiteMove)
                            Possible[y - 1 - 2, x - 1] = 'x';

                        if (Placement[y - 1 - 2, x - 1] == '·')
                            Possible[y - 1 - 2, x - 1] = '·';
                    }

                    if (x + 1 < 8)
                    {
                        if (Placement[y - 1 - 2, x + 1] != '·' && CheckWhiteFigure(Placement[y - 1 - 2, x + 1]) != WhiteMove)
                            Possible[y - 1 - 2, x + 1] = 'x';

                        if (Placement[y - 1 - 2, x + 1] == '·')
                            Possible[y - 1 - 2, x + 1] = '·';
                    }
                }

                break;
            case "r":
                for (int i = 1; y - 1 - i >= 0; i++)
                {
                    if (Placement[y - 1 - i, x] == '·')
                    {
                        Possible[y - 1 - i, x] = '·';
                        continue;
                    }
                    if (CheckWhiteFigure(Placement[y - 1 - i, x]) == WhiteMove)
                        break;
                    if (Placement[y - 1 - i, x] != '·' && CheckWhiteFigure(Placement[y - 1 - i, x]) != WhiteMove)
                    {
                        Possible[y - 1 - i, x] = 'x';
                        break;
                    }
                }

                for (int i = 1; y - 1 + i < 8; i++)
                {
                    if (Placement[y - 1 + i, x] == '·')
                    {
                        Possible[y - 1 + i, x] = '·';
                        continue;
                    }
                    if (CheckWhiteFigure(Placement[y - 1 + i, x]) == WhiteMove)
                        break;
                    if (Placement[y - 1 + i, x] != '·' && CheckWhiteFigure(Placement[y - 1 + i, x]) != WhiteMove)
                    {
                        Possible[y - 1 + i, x] = 'x';
                        break;
                    }
                }

                for (int i = 1; x - i >= 0; i++)
                {
                    if (Placement[y - 1, x - i] == '·')
                    {
                        Possible[y - 1, x - i] = '·';
                        continue;
                    }
                    if (CheckWhiteFigure(Placement[y - 1, x - i]) == WhiteMove)
                        break;
                    if (Placement[y - 1, x - i] != '·' && CheckWhiteFigure(Placement[y - 1, x - i]) != WhiteMove)
                    {
                        Possible[y - 1, x - i] = 'x';
                        break;
                    }
                }

                for (int i = 1; x + i < 8; i++)
                {
                    if (Placement[y - 1, x + i] == '·')
                    {
                        Possible[y - 1, x + i] = '·';
                        continue;
                    }
                    if (CheckWhiteFigure(Placement[y - 1, x + i]) == WhiteMove)
                        break;
                    if (Placement[y - 1, x + i] != '·' && CheckWhiteFigure(Placement[y - 1, x + i]) != WhiteMove)
                    {
                        Possible[y - 1, x + i] = 'x';
                        break;
                    }
                }

                break;
            case "b":
                for (int i = 1; y - 1 - i >= 0 && x - i >= 0; i++)
                {
                    if (Placement[y - 1 - i, x - i] == '·')
                    {
                        Possible[y - 1 - i, x - i] = '·';
                        continue;
                    }

                    if (CheckWhiteFigure(Placement[y - 1 - i, x - i]) == WhiteMove) break;

                    if (Placement[y - 1 - i, x - i] != '·' && CheckWhiteFigure(Placement[y - 1 - i, x - i]) != WhiteMove)
                    {
                        Possible[y - 1 - i, x - i] = 'x';
                        break;
                    }
                    
                }

                for (int i = 1; y - 1 - i >= 0 && x + i < 8; i++)
                {
                    if (Placement[y - 1 - i, x + i] == '·')
                    {
                        Possible[y - 1 - i, x + i] = '·';
                        continue;
                    }

                    if (CheckWhiteFigure(Placement[y - 1 - i, x + i]) == WhiteMove) break;

                    if (Placement[y - 1 - i, x + i] != '·' && CheckWhiteFigure(Placement[y - 1 - i, x + i]) != WhiteMove)
                    {
                        Possible[y - 1 - i, x + i] = 'x';
                        break;
                    }
                    
                }

                for (int i = 1; y - 1 + i < 8 && x - i >= 0; i++)
                {
                    if (Placement[y - 1 + i, x - i] == '·')
                    {
                        Possible[y - 1 + i, x - i] = '·';
                        continue;
                    }

                    if (CheckWhiteFigure(Placement[y - 1 + i, x - i]) == WhiteMove) break;

                    if (Placement[y - 1 + i, x - i] != '·' && CheckWhiteFigure(Placement[y - 1 + i, x - i]) != WhiteMove)
                    {
                        Possible[y - 1 + i, x - i] = 'x';
                        break;
                    }
                  
                }

                for (int i = 1; y - 1 + i < 8 && x + i < 8; i++)
                {

                    if (Placement[y - 1 + i, x + i] == '·')
                    {
                        Possible[y - 1 + i, x + i] = '·';
                        continue;
                    }

                    if (CheckWhiteFigure(Placement[y - 1 + i, x + i]) == WhiteMove) break;

                    if (Placement[y - 1 + i, x + i] != '·' && CheckWhiteFigure(Placement[y - 1 + i, x + i]) != WhiteMove)
                    {
                        Possible[y - 1 + i, x + i] = 'x';
                        break;
                    }
           
                    
                }
                
                break;
            case "q":
                for (int i = 1; y - 1 - i >= 0 && x - i >= 0; i++)
                {
                    if (Placement[y - 1 - i, x - i] == '·')
                    {
                        Possible[y - 1 - i, x - i] = '·';
                        continue;
                    }

                    if (CheckWhiteFigure(Placement[y - 1 - i, x - i]) == WhiteMove) break;

                    if (Placement[y - 1 - i, x - i] != '·' && CheckWhiteFigure(Placement[y - 1 - i, x - i]) != WhiteMove)
                    {
                        Possible[y - 1 - i, x - i] = 'x';
                        break;
                    }

                }

                for (int i = 1; y - 1 - i >= 0 && x + i < 8; i++)
                {
                    if (Placement[y - 1 - i, x + i] == '·')
                    {
                        Possible[y - 1 - i, x + i] = '·';
                        continue;
                    }

                    if (CheckWhiteFigure(Placement[y - 1 - i, x + i]) == WhiteMove) break;

                    if (Placement[y - 1 - i, x + i] != '·' && CheckWhiteFigure(Placement[y - 1 - i, x + i]) != WhiteMove)
                    {
                        Possible[y - 1 - i, x + i] = 'x';
                        break;
                    }

                }

                for (int i = 1; y - 1 + i < 8 && x - i >= 0; i++)
                {
                    if (Placement[y - 1 + i, x - i] == '·')
                    {
                        Possible[y - 1 + i, x - i] = '·';
                        continue;
                    }

                    if (CheckWhiteFigure(Placement[y - 1 + i, x - i]) == WhiteMove) break;

                    if (Placement[y - 1 + i, x - i] != '·' && CheckWhiteFigure(Placement[y - 1 + i, x - i]) != WhiteMove)
                    {
                        Possible[y - 1 + i, x - i] = 'x';
                        break;
                    }

                }

                for (int i = 1; y - 1 + i < 8 && x + i < 8; i++)
                {

                    if (Placement[y - 1 + i, x + i] == '·')
                    {
                        Possible[y - 1 + i, x + i] = '·';
                        continue;
                    }

                    if (CheckWhiteFigure(Placement[y - 1 + i, x + i]) == WhiteMove) break;

                    if (Placement[y - 1 + i, x + i] != '·' && CheckWhiteFigure(Placement[y - 1 + i, x + i]) != WhiteMove)
                    {
                        Possible[y - 1 + i, x + i] = 'x';
                        break;
                    }


                }

                //----------------------------

                for (int i = 1; y - 1 - i >= 0; i++)
                {
                    if (Placement[y - 1 - i, x] == '·')
                    {
                        Possible[y - 1 - i, x] = '·';
                        continue;
                    }
                    if (CheckWhiteFigure(Placement[y - 1 - i, x]) == WhiteMove)
                        break;
                    if (Placement[y - 1 - i, x] != '·' && CheckWhiteFigure(Placement[y - 1 - i, x]) != WhiteMove)
                    {
                        Possible[y - 1 - i, x] = 'x';
                        break;
                    }
                }

                for (int i = 1; y - 1 + i < 8; i++)
                {
                    if (Placement[y - 1 + i, x] == '·')
                    {
                        Possible[y - 1 + i, x] = '·';
                        continue;
                    }
                    if (CheckWhiteFigure(Placement[y - 1 + i, x]) == WhiteMove)
                        break;
                    if (Placement[y - 1 + i, x] != '·' && CheckWhiteFigure(Placement[y - 1 + i, x]) != WhiteMove)
                    {
                        Possible[y - 1 + i, x] = 'x';
                        break;
                    }
                }

                for (int i = 1; x - i >= 0; i++)
                {
                    if (Placement[y - 1, x - i] == '·')
                    {
                        Possible[y - 1, x - i] = '·';
                        continue;
                    }
                    if (CheckWhiteFigure(Placement[y - 1, x - i]) == WhiteMove)
                        break;
                    if (Placement[y - 1, x - i] != '·' && CheckWhiteFigure(Placement[y - 1, x - i]) != WhiteMove)
                    {
                        Possible[y - 1, x - i] = 'x';
                        break;
                    }
                }

                for (int i = 1; x + i < 8; i++)
                {
                    if (Placement[y - 1, x + i] == '·')
                    {
                        Possible[y - 1, x + i] = '·';
                        continue;
                    }
                    if (CheckWhiteFigure(Placement[y - 1, x + i]) == WhiteMove)
                        break;
                    if (Placement[y - 1, x + i] != '·' && CheckWhiteFigure(Placement[y - 1, x + i]) != WhiteMove)
                    {
                        Possible[y - 1, x + i] = 'x';
                        break;
                    }
                }

                break;
            case "k":

                if (y - 1 - 1 >= 0 && x - 1 >= 0)
                {
                    if (Placement[y - 1 - 1, x - 1] == '·')
                        Possible[y - 1 - 1, x - 1] = '·';
                    else
                    if (Placement[y - 1 - 1, x - 1] != '·' && CheckWhiteFigure(Placement[y - 1 - 1, x - 1]) != WhiteMove)
                    {
                        Possible[y - 1 - 1, x - 1] = 'x';
                            
                    }
                }

                if (y - 1 - 1 >= 0 && x + 1 < 8)
                {
                    if (Placement[y - 1 - 1, x + 1] == '·')
                        Possible[y - 1 - 1, x + 1] = '·';
                    else
                    if (Placement[y - 1 - 1, x + 1] != '·' && CheckWhiteFigure(Placement[y - 1 - 1, x + 1]) != WhiteMove)
                    {
                        Possible[y - 1 - 1, x + 1] = 'x';
                            
                    }
                }

                if (y + 1 - 1 < 8 && x - 1 >= 0)
                {
                    if (Placement[y - 1 + 1, x - 1] == '·')
                        Possible[y - 1 + 1, x - 1] = '·';
                    else
                    if (Placement[y - 1 + 1, x - 1] != '·' && CheckWhiteFigure(Placement[y - 1 + 1, x - 1]) != WhiteMove)
                    {
                        Possible[y - 1 + 1, x - 1] = 'x';
                           
                    }
                }

                if (y + 1 - 1 < 8 && x - 1 < 8)
                {
                    if (Placement[y - 1 + 1, x + 1] == '·')
                        Possible[y - 1 + 1, x + 1] = '·';
                    else
                    if (Placement[y - 1 + 1, x + 1] != '·' && CheckWhiteFigure(Placement[y - 1 + 1, x + 1]) != WhiteMove)
                    {
                        Possible[y - 1 + 1, x + 1] = 'x';
                           
                    }

                    
                }

                //----------------------------

                if (y - 1 - 1 >= 0)
                {
                    if (Placement[y - 1 - 1, x] == '·')
                        Possible[y - 1 - 1, x] = '·';
                    else
                    if (Placement[y - 1 - 1, x] != '·' && CheckWhiteFigure(Placement[y - 1 - 1, x]) != WhiteMove)
                    {
                        Possible[y - 1 - 1, x] = 'x';
                            
                    }
                }

                if (y - 1 + 1 < 8)
                {
                    if (Placement[y - 1 + 1, x] == '·')
                        Possible[y - 1 + 1, x] = '·';
                    else
                    if (Placement[y - 1 + 1, x] != '·' && CheckWhiteFigure(Placement[y - 1 + 1, x]) != WhiteMove)
                    {
                        Possible[y - 1 + 1, x] = 'x';
                            
                    }
                }

                if (x - 1 >= 0)
                {
                    if (Placement[y - 1, x - 1] == '·')
                        Possible[y - 1, x - 1] = '·';
                    else
                    if (Placement[y - 1, x - 1] != '·' && CheckWhiteFigure(Placement[y - 1, x - 1]) != WhiteMove)
                    {
                        Possible[y - 1, x - 1] = 'x';
                            
                    }
                }

                if (x + 1 < 8)
                {
                    if (Placement[y - 1, x + 1] == '·')
                        Possible[y - 1, x + 1] = '·';
                    else
                    if (Placement[y - 1, x + 1] != '·' && CheckWhiteFigure(Placement[y - 1, x + 1]) != WhiteMove)
                    {
                        Possible[y - 1, x + 1] = 'x';
                            
                    }
                }
                break;
        }
            


        PlacementFigure = placementFigure;
        return "OK";
    }

    private void ResetPossible()
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                Possible[y, x] = ' ';
    }

    private string Status()
    {
        /* 
        if (GameMode > 1)
            return "The game cannot continue";
        */

        string striker = WhiteMove ? "White " : "Black ";

        switch (GameMode)
        {
            case 3: return striker + "win!";
        }
        return "OK";
    }

    public char ToCharAH(int n)
    {
        switch (n)
        {
            case 1: return 'A';
            case 2: return 'B';
            case 3: return 'C';
            case 4: return 'D';
            case 5: return 'E';
            case 6: return 'F';
            case 7: return 'G';
            case 8: return 'H';
            default: return '0';
        }

    }
}

