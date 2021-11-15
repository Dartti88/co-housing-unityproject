using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateBoard : MonoBehaviour
{
    public int boardWidth = 3;
    public int boardHeight = 3;

    public float cellWidth = 1f;
    public float cellHeight = 1f;

    public float lineThickness = .1f;

    public Transform lines;

    public GameObject cellPrefab;
    public GameObject linePrefab;

    void Start()
    {
        Create();
    }

    void Create()
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
}
