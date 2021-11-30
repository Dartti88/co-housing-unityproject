using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public GameObject popup;

    public void Show(string msg)
    {
        popup.SetActive(true);
        popup.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = msg;
    }

    public void Close()
    {
        popup.SetActive(false);
    }
}
