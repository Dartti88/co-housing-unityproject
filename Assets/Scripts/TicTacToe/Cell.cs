using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CellState
{
    empty,
    cross,
    nought
}

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    CellState cellState;
    Image image;
    public Color normalColor, highlightColor, winColor;

    TicTacToe manager;

    void Start()
    {
        manager = transform.parent.parent.GetComponent<TicTacToe>();

        image = GetComponent<Image>();
        cellState = CellState.empty;

        foreach(Transform child in transform)
        {
            child.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
        }

    }

    public void ChangeState(CellState state)
    {
        cellState = state;
        if(state != CellState.empty)
        {
            transform.GetChild(((int)cellState - 1)).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    void CheckForWin()
    {
        if (manager.IsWin())
        {
            manager.SetWin(true);
            manager.HighlightWin();
            manager.InvokeReset();
            return;
        }
        else if (manager.IsDraw())
        {
            manager.InvokeReset();
            return;
        }
        manager.ChangeTurn();
        // SendBoardStateToServer()
    }

    public CellState GetState()
    {
        return cellState;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(manager.GameIsRunning() && cellState == CellState.empty)
        {
            if (manager.GetTurn() == CellState.cross && manager.GetPLayerNumber() == 0)
            {
                ChangeState(CellState.nought);
                CheckForWin();
                manager.SetPlayerNumber(manager.GetPLayerNumber() == 0 ? 1 : 0);
            }

            else if (manager.GetTurn() == CellState.nought && manager.GetPLayerNumber() == 1)
            {
                ChangeState(CellState.cross);
                CheckForWin();
                manager.SetPlayerNumber(manager.GetPLayerNumber() == 0 ? 1 : 0);
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager.GameIsRunning())
        {
            image.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(manager.GameIsRunning())
        {
            image.color = normalColor;
        }
    }

    public void WinHighlight()
    {
        image.color = winColor;
    }

    public void Reset()
    {
        cellState = CellState.empty;
        image.color = normalColor;

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }
}
