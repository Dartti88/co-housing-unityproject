using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PicChange : MonoBehaviour
{
    public Image currentProfilePic;
    public GameObject thisButton;
    public GameObject ProfilePics;
    public void Click()
    {
        //this is not the best way to execute this but I cannot bother 
        Image pic = thisButton.gameObject.GetComponent<Image>();
        currentProfilePic.gameObject.GetComponent<Image>().sprite = pic.sprite;
        Debug.Log(currentProfilePic.sprite);
        ProfilePics.SetActive(false);
    }
}
