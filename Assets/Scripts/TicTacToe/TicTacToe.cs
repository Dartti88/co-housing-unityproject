using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{

    public int boardWidth = 3;
    public int boardHeight = 3;

    public float cellWidth = 1f;
    public float cellHeight = 1f;

    public float lineThickness = .1f;

    public int win = 3;

    public GameObject cellPrefab;
    public GameObject linePrefab;

    public Transform lines;

    CellState turn;

    Cell[] winCells;

    bool gameRunning;


    void Start()
    {
        gameRunning = true;
        turn = CellState.cross;

        winCells = new Cell[win];

        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {

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


    void CreateBoard()
    {
        float bottomLeftX = transform.position.x - boardWidth * cellWidth / 2f + cellWidth / 2f;
        float bottomLeftY = transform.position.y - boardHeight * cellHeight / 2f + cellHeight / 2f;

        // Creates grid of cells
        for (int y = 0; y < boardHeight; y++)
        {
            GameObject row = new GameObject();
            row.transform.SetParent(transform);
            row.name = "Row " + y;
            for (int x = 0; x < boardWidth; x++)
            {
                float cellX = bottomLeftX + x * cellWidth;
                float cellY = bottomLeftY + y * cellHeight;
                GameObject go = Instantiate(cellPrefab);

                go.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth, cellHeight);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(cellX, cellY);

                go.transform.SetParent(row.transform);
            }
        }

        // Creates vertical lines
        for (int x = 0; x < boardWidth - 1; x++)
        {
            float lineX = bottomLeftX + x * cellWidth + cellWidth / 2f;
            float lineY = bottomLeftY + boardHeight * cellHeight / 2f - cellHeight / 2f;
            GameObject go = Instantiate(linePrefab);

            go.GetComponent<RectTransform>().sizeDelta = new Vector2(lineThickness, cellHeight * boardHeight);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(lineX, lineY);
            go.GetComponent<Image>().color = new Color(0, 0, 0);
            go.transform.SetParent(lines);
        }

        // Creates horizontal lines
        for (int y = 0; y < boardHeight - 1; y++)
        {
            float lineX = bottomLeftX + boardWidth * cellWidth / 2f - cellWidth / 2f;
            float lineY = bottomLeftY + y * cellHeight + cellHeight / 2f;
            GameObject go = Instantiate(linePrefab);

            go.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth * boardWidth, lineThickness);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(lineX, lineY);
            go.GetComponent<Image>().color = new Color(0, 0, 0);
            go.transform.SetParent(lines);
        }
    }

    public bool IsWin()
    {
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
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

    bool FindChainHorizontal(Cell startCell, int x, int y)
    {
        int chain = 1;
        for (int i = x + 1; i < boardWidth; i++)
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
        for (int i = y + 1; i < boardHeight; i++)
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
        for (int i = 1; i < boardWidth; i++)
        {
            if (x + i > boardWidth - 1 || y + i > boardHeight - 1)
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
        for (int i = 1; i < boardWidth; i++)
        {
            if (x + i > boardWidth - 1 || y - i < 0)
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
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Cell cell = transform.GetChild(y).GetChild(x).GetComponent<Cell>();
                cell.Reset();
            }
        }
    }

    public void InvokeReset()
    {
        Invoke("Reset", 1f);
    }
}
