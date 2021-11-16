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

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeState(CellState state)
    {
        cellState = state;
        transform.GetChild(((int)cellState-1)).gameObject.SetActive(true);

        if (manager.IsWin())
        {
            manager.SetWin(true);
            manager.HighlightWin();
            manager.InvokeReset();
        }

        manager.ChangeTurn();
    }

    public CellState GetState()
    {
        return cellState;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(manager.GameIsRunning() && cellState == CellState.empty)
        {
            if (manager.GetTurn() == CellState.cross)
                ChangeState(CellState.nought);
            else if (manager.GetTurn() == CellState.nought)
                ChangeState(CellState.cross);
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
