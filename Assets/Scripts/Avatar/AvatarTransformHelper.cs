using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarTransformHelper : MonoBehaviour
{
    // rotations
    public float frontRotateX;
    public float frontRotateY;

    public float rightRotateX;
    public float rightRotateY;

    public float backRotateX;
    public float backRotateY;

    public float leftRotateX;
    public float leftRotateY;

    // positions
    public float frontPositionX;
    public float frontPositionZ;

    public float rightPositionX;
    public float rightPositionZ;

    public float backPositionX;
    public float backPositionZ;

    public float leftPositionX;
    public float leftPositionZ;

    public void SetTransforms(int camDir)
    {
        switch (camDir)
        {
            case 0:
                gameObject.transform.localRotation = Quaternion.Euler(frontRotateX, frontRotateY, 0);
                gameObject.transform.localPosition = new Vector3(frontPositionX, 0, frontPositionZ);
                break;
            case 1:
                gameObject.transform.localRotation = Quaternion.Euler(rightRotateX, rightRotateY, 0);
                gameObject.transform.localPosition = new Vector3(rightPositionX, 0, rightPositionZ);
                break;
            case 2:
                gameObject.transform.localRotation = Quaternion.Euler(backRotateX, backRotateY, 0);
                gameObject.transform.localPosition = new Vector3(backPositionX, 0, backPositionZ);
                break;
            case 3:
                gameObject.transform.localRotation = Quaternion.Euler(leftRotateX, leftRotateY, 0);
                gameObject.transform.localPosition = new Vector3(leftPositionX, 0, leftPositionZ);
                break;
            default:
                gameObject.transform.localRotation = Quaternion.Euler(frontRotateX, frontRotateY, 0);
                gameObject.transform.localPosition = new Vector3(frontPositionX, 0, frontPositionZ);
                break;
        }
    }
}
