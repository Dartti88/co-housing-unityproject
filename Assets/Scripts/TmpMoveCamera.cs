using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpMoveCamera : MonoBehaviour
{
    public float cameraSpeed;

    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");

        gameObject.transform.Translate(new Vector3(xAxisValue * cameraSpeed, 0.0f, zAxisValue * cameraSpeed));
    }
}
