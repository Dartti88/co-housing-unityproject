using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public delegate void ClickActionDelegate();
    public static event ClickActionDelegate OnRotateAvatarRight;
    public static event ClickActionDelegate OnRotateAvatarLeft;

    public Button rotateCameraRight;
    public Button rotateCameraLeft;

    private void Start()
    {
        rotateCameraRight.onClick.AddListener(RotateCameraRight);
        rotateCameraLeft.onClick.AddListener(RotateCameraLeft);
    }

    private void RotateCameraRight()
    {
        if (OnRotateAvatarRight != null) { OnRotateAvatarRight(); }
    }

    private void RotateCameraLeft()
    {
        if (OnRotateAvatarLeft != null) { OnRotateAvatarLeft(); }
    }
}
