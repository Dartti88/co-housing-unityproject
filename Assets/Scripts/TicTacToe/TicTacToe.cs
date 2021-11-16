using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TicTacToe : MonoBehaviour
{
    public int win = 3;

    CellState turn;

    Cell[] winCells;

    bool gameRunning;

    int playerNumber = 0;

    public CreateBoard createBoard;

    void Start()
    {
        gameRunning = true;
        turn = CellState.cross;

        winCells = new Cell[win];
        //UpdateStateFromServer("100010020");
    }

    void Update()
    {
        /*
        if(ServerHasNewState())
        {
            UpdateStateFromServer("100010020");
        }
        */
    }

    bool ServerHasNewState()
    {
        return true;
    }

    void UpdateStateFromServer(string state)
    {
        ChangeTurn();
        for (int y = 0; y < createBoard.boardHeight; y++)
        {
            for (int x = 0; x < createBoard.boardWidth; x++)
            {
                int cellState = state[createBoard.boardWidth * y + x] - '0';
                Debug.Log(cellState);

                Cell cell = transform.GetChild(y).GetChild(x).GetComponent<Cell>();
                cell.ChangeState((CellState)cellState);
            }
        }
    }

    public int GetPLayerNumber()
    {
        return playerNumber;
    }

    public void SetPlayerNumber(int n)
    {
        playerNumber = n;
    }

    public CellState GetTurn()
    {
        return turn;
    }

    public void ChangeTurn()
    {
        if (turn == CellState.nought)
            turn = CellState.cross;
        else
            turn = CellState.nought;
    }

    public bool IsWin()
    {
        for (int y = 0; y < createBoard.boardHeight; y++)
        {
            for (int x = 0; x < createBoard.boardWidth; x++)
            {
                Cell startCell = transform.GetChild(y).GetChild(x).GetComponent<Cell>();
                winCells[0] = startCell;

                if (FindChainHorizontal(startCell, x, y) || 
                    FindChainVertical(startCell, x, y) ||
                    FindChainDiagonal1(startCell, x, y) ||
                    FindChainDiagonal2(startCell, x, y))
                    return true;
            }
        }

        return false;
    }

    public bool IsDraw()
    {
        for (int y = 0; y < createBoard.boardHeight; y++)
        {
            for (int x = 0; x < createBoard.boardWidth; x++)
            {
                Cell startCell = transform.GetChild(y).GetChild(x).GetComponent<Cell>();
                if (startCell.GetState() == CellState.empty)
                    return false;
            }
        }
        return true;
    }

    bool FindChainHorizontal(Cell startCell, int x, int y)
    {
        int chain = 1;
        for (int i = x + 1; i < createBoard.boardWidth; i++)
        {
            Cell nextCell = transform.GetChild(y).GetChild(i).GetComponent<Cell>();

            if (startCell.GetState() == nextCell.GetState() && startCell.GetState() != CellState.empty)
            {
                winCells[chain] = nextCell;
                chain++;
            }
            else break;
        }
        if (chain == win) return true;
        return false;
    }

    bool FindChainVertical(Cell startCell, int x, int y)
    {
        int chain = 1;
        for (int i = y + 1; i < createBoard.boardHeight; i++)
        {
            Cell nextCell = transform.GetChild(i).GetChild(x).GetComponent<Cell>();

            if (startCell.GetState() == nextCell.GetState() && startCell.GetState() != CellState.empty)
            {
                winCells[chain] = nextCell;
                chain++;
            }
            else break;
        }
        if (chain == win) return true;
        return false;
    }

    bool FindChainDiagonal1(Cell startCell, int x, int y)
    {
        int chain = 1;
        for (int i = 1; i < createBoard.boardWidth; i++)
        {
            if (x + i > createBoard.boardWidth - 1 || y + i > createBoard.boardHeight - 1)
                break;

            Cell nextCell = transform.GetChild(y + i).GetChild(x + i).GetComponent<Cell>();

            if (startCell.GetState() == nextCell.GetState() && startCell.GetState() != CellState.empty)
            {
                winCells[chain] = nextCell;
                chain++;
            }
            else break;
        }
        if (chain == win) return true;
        return false;
    }

    bool FindChainDiagonal2(Cell startCell, int x, int y)
    {
        int chain = 1;
        for (int i = 1; i < createBoard.boardWidth; i++)
        {
            if (x + i > createBoard.boardWidth - 1 || y - i < 0)
                break;

            Cell nextCell = transform.GetChild(y - i).GetChild(x + i).GetComponent<Cell>();

            if (startCell.GetState() == nextCell.GetState() && startCell.GetState() != CellState.empty)
            {
                winCells[chain] = nextCell;
                chain++;
            }
            else break;
        }
        if (chain == win) return true;
        return false;
    }

    public void HighlightWin()
    {
        foreach(Cell cell in winCells)
        {
            cell.WinHighlight();
        }
    }

    public void SetWin(bool winState)
    {
        gameRunning = !winState;
    }

    public bool GameIsRunning()
    {
        return gameRunning;
    }

    void Reset()
    {
        gameRunning = true;
        turn = CellState.cross;
        for (int y = 0; y < createBoard.boardHeight; y++)
        {
            for (int x = 0; x < createBoard.boardWidth; x++)
            {
                Cell cell = transform.GetChild(y).GetChild(x).GetComponent<Cell>();
                cell.Reset();
            }
        }
        // SendBoardStateToServer()
    }

    public void InvokeReset()
    {
        Invoke("Reset", 1f);
    }
}
