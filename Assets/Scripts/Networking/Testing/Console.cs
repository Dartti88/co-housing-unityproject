using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{

    
    const float textWidth = 200;
    float linePadding = 0.0f;
    float textSize = 15.0f;
    int consoleTextCount = 0;
    
    public Scrollbar scrollBar;
    public InputField inputField;
    
    public void SubmitInput()
    {
        AddText(">> " + inputField.text, Color.cyan);
        inputField.text = "";
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    void AddText(string text, Color color)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;

        GameObject newTextObj = new GameObject("Text");
        newTextObj.transform.parent = gameObject.transform;
        float textOffsetY = textSize + linePadding;
        float totalTextOffsetY = -consoleTextCount * textOffsetY;
        newTextObj.transform.localPosition = new Vector3(-parentWidth * 0.5f + (textWidth * 0.5f), totalTextOffsetY, 0);
        Text newTextComonent = newTextObj.AddComponent<Text>();
        newTextComonent.text = text;
        newTextComonent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        newTextComonent.color = color;
        newTextComonent.fontSize = (int)textSize - 2;
        newTextComonent.alignment = TextAnchor.UpperLeft;
        RectTransform textRectTransform = newTextObj.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = new Vector2(textWidth, textSize);
        
        consoleTextCount++;

        // change the content rect's size as well..
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + (textOffsetY * 2.0f));
    }
}
