using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// singleton controller to use player input, move camera and toggle walls
public class GameController : Singleton<GameController>
{
    // layermasks to check players input against correct objects in the game
    public LayerMask RayLayers;
    public LayerMask NaviGroundLayer;
    public LayerMask NaviObjectLayer;
    public LayerMask WallLayer;

    // how long touch before it turns into drag
    public float TouchTimeThreshold = 0.8f;

    public float ScrollSpeedMultiplier = 0.1f;

    // min max camera positions so player cant scroll away from the house
    public Vector3 MinCameraPosition;
    public Vector3 MaxCameraPosition;

    // walls currently disabled
    private List<HouseWall> DisabledWalls;

    // where pointer was during last frame
    private Vector2 lastMousePos;

    // how long current input time is
    private float currentTouchTime = 0.0f;

    private bool touchStartedOnUI = false;

    // called once on scene load
    void Start()
    {
        DisabledWalls = new List<HouseWall>();
        lastMousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        // code inside UNITY_EDITOR is used on PC Unity editor for testing purposes, not on mobile build
#if UNITY_EDITOR

        // if mouse button down and touch didnt start over UI, try to drag camera or click
        if (Input.GetMouseButton(0) && !touchStartedOnUI)
        {
            if (IsPointerOverUIObject())
            {
                if (Input.GetMouseButtonDown(0))
                    touchStartedOnUI = true;
                return;
            }
            // dont move if UI was clicked
            Vector2 mousepos = Input.mousePosition;

            Vector3 newPos = Rotator.Instance.transform.position;
            var deltaX = (mousepos.x - lastMousePos.x) * ScrollSpeedMultiplier;
            var deltaY = (mousepos.y - lastMousePos.y) * ScrollSpeedMultiplier;
            Rotator.Instance.transform.position = MovePosition(deltaX, deltaY, Rotator.Instance.transform.position);
            currentTouchTime += Time.deltaTime;
        }

        else if (Input.GetMouseButtonUp(0))
        {

            if (currentTouchTime > TouchTimeThreshold)
            {
                currentTouchTime = 0.0f;
                return;
            }
            else
            {
                currentTouchTime = 0.0f;

                // Code used in Android builds
#elif UNITY_ANDROID

        // if one touch and not over UI elements
        if (Input.touchCount == 1)
        {
            if (IsPointerOverUIObject())
            {
                if (Input.GetMouseButtonDown(0))
                    touchStartedOnUI = true;
                return;
            }

            Touch touch = Input.GetTouch(0);
                // if touch moved, move camera
            if (touch.phase == TouchPhase.Moved)
            {
                var deltaX = touch.deltaPosition.x * ScrollSpeedMultiplier;
                var deltaY = touch.deltaPosition.y * ScrollSpeedMultiplier;
                Rotator.Instance.transform.position = MovePosition(deltaX, deltaY, Rotator.Instance.transform.position);
                currentTouchTime += touch.deltaTime;
            }
            else if (touch.phase == TouchPhase.Ended) // if touch ended and was short enough, go to moving player
            {
                if (currentTouchTime > TouchTimeThreshold)
                {
                    currentTouchTime = 0.0f;
                    return;
                }
                currentTouchTime = 0.0f;
                // player input
#endif

                if (touchStartedOnUI) // dont move if UI was clicked
                {
                    touchStartedOnUI = false;
                    return;
                }

                HouseObjectController.Instance.UnselectCurrent();

                // cast a ray from camera, if an object on a specified layer was hit, select it and start walking the player towards it

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f, RayLayers))
                {
                    if ((NaviGroundLayer & 1 << hit.collider.gameObject.layer) == 1 << hit.collider.gameObject.layer)
                    {
                        PlayerController.Instance.SetGoal(hit.point);
                    }
                    if ((NaviObjectLayer & 1 << hit.collider.gameObject.layer) == 1 << hit.collider.gameObject.layer)
                    {
                        ClickableObject target = hit.collider.GetComponent<ClickableObject>();
                        target.Select();
                        PlayerController.Instance.SetGoal(target.NaviTarget.position);
                    }
                }

            }
        }
        lastMousePos = Input.mousePosition;
    }

    // check if player clicked/tapped an UI element
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    // move camera position based on input and which way camera is rotated
    Vector3 MovePosition(float deltaX, float deltaY, Vector3 newPos)
    {
        switch (Rotator.Instance.CurrentDirection)
        {
            case ViewDirection.FRONT:
                {
                    newPos.x += deltaX;
                    newPos.z += deltaY;
                    break;
                }
            case ViewDirection.BACK:
                {
                    newPos.x -= deltaX;
                    newPos.z -= deltaY;
                    break;
                }
            case ViewDirection.LEFT:
                {
                    newPos.x += deltaY;
                    newPos.z -= deltaX;
                    break;
                }
            case ViewDirection.RIGHT:
                {
                    newPos.x -= deltaY;
                    newPos.z += deltaX;
                    break;
                }
        }
        newPos.x = Mathf.Clamp(newPos.x, MinCameraPosition.x, MaxCameraPosition.x);
        newPos.z = Mathf.Clamp(newPos.z, MinCameraPosition.z, MaxCameraPosition.z);
        return newPos;
    }

    public void GoDownstairs()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void GoUpstairs()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
