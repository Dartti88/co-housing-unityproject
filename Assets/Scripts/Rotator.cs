using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton controller to rotate the camera around the house
public class Rotator : Singleton<Rotator>
{
    public float RotationDuration = 0.5f;

    // current camera viewing direction
    public ViewDirection CurrentDirection;

    private Vector3 initialRotation;
    private Vector3 targetRotation;

    // camera starting position on scene load
    private Vector3 initialPosition;

    private bool Rotating = false;

    public int Step = 0;

    private void Start()
    {
        initialPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
    }

    public void RotateScene(int direction)
    {
        if (!Rotating)
        {
            StartCoroutine(LerpRotation(direction));
            Rotating = true;
        }
    }

    // slowly rotate camera towards the new specified direction
    IEnumerator LerpRotation(int direction)
    {
        initialRotation = transform.rotation.eulerAngles;
        targetRotation = initialRotation;
        targetRotation.y += 90 * direction;
        int roundedRotation = (int)Mathf.Ceil(targetRotation.y + (targetRotation.y < 0 ? -1 : 1));
        Step = roundedRotation / 90;

        switch (Step) // set the direction variable based on the rotation degrees
        {
            case -4:
            case 0:
            case 4:
                {
                    CurrentDirection = ViewDirection.FRONT;
                    break;
                }
            case 1:
            case -3:
                {
                    CurrentDirection = ViewDirection.LEFT;
                    break;
                }
            case 2:
            case -2:
                {
                    CurrentDirection = ViewDirection.BACK;
                    break;
                }
            case -1:
            case 3:
                {
                    CurrentDirection = ViewDirection.RIGHT;
                    break;
                }
        }

        float t = 0.0f;
        while (t < 1.0f) // lerp the camera rotation from initial to target rotation over RotationDuration seconds
        {
            t += Time.deltaTime / RotationDuration;
            transform.rotation = Quaternion.Euler(Vector3.Lerp(initialRotation, targetRotation, t));
            yield return null;
        }
        Rotating = false;
        WallController.Instance.RefreshWalls();
    }

}

// enumerable to make camera direction into a more human readable format
public enum ViewDirection
{
    FRONT,
    BACK,
    LEFT,
    RIGHT
}
