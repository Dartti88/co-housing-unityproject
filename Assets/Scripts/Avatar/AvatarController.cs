using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used to keep track of which way the avatar character is looking at
public enum AvatarDirection
{
	BOT_RIGHT = 0,
	BOT_LEFT = 1,
	TOP_LEFT = 2,
	TOP_RIGHT = 3
}

public class AvatarController : MonoBehaviour
{
    // Each look direction needs its own GameObject
	public GameObject avatarBotRight;
	public GameObject avatarBotLeft;
	public GameObject avatarTopLeft;
	public GameObject avatarTopRight;

    // Lists the above GameObjects so they can be accessed using an index
    private List<GameObject> avatarList;

    private Transform cameraTransform;
    public AvatarDirection avatarDirection;

    private PlayerController playerController;

    // Delay to make avatar not change direction too often
    public float updateDelayAmount;
    private float updateDelay;

    // Avatar rotation delay to make switching directions look nicer when using camera buttons
    public float rotateAvatarDelay;

    public Sprite fullPictureFront;

    public void InitializeAvatar(int startDir)
    {
        cameraTransform = Camera.main.transform;

        playerController = PlayerController.Instance;

        MakeAvatarList();
        SetStartingDirection(startDir);

        updateDelay = updateDelayAmount;
    }

    private void Start()
    {
        InitializeAvatar(0);
    }

    private void Update()
    {
        // Check if the character is moving
        if (playerController.GetWalking())
        {
            // Check update delay to avoid running code every frame
            if (updateDelay >= updateDelayAmount)
            {
                // Get character's position
                Vector3 originalPosition = new Vector3(playerController.gameObject.transform.position.x,
                                                       0,
                                                       playerController.gameObject.transform.position.z);

                // Get a position in the direction of the character's current velocity vector
                Vector3 nextPosition = new Vector3(playerController.GetNextPosition().x, 0, playerController.GetNextPosition().z);


                // EI TARVITA ENÄÄ. Jätetty talteen varmuuden vuoksi
                //float alpha = Mathf.Atan((originalPosition.z - cameraTransform.position.z) /
                //                         (originalPosition.x - cameraTransform.position.x)) *
                //                         Mathf.Rad2Deg;
                //Debug.Log("Angle:" + alpha);

                //Debug.Log("Next:" + nextPosition + " || Camera: " + cameraTransform.position);
                //Debug.Log("X: " + (nextPosition.x - cameraTransform.position.x) + " || Z: " + (nextPosition.z - cameraTransform.position.z));


                // Check which way the character is moving
                // The int number parameter of CameraPositionFactor(int, bool) corresponds with 
                // the direction of movement when camera position is FRONT
                // bottom right = 0, bottom left = 1, top left = 2, top right = 3
                // The bool value determines whether camera position adjustments are needed
                // false = adjustments are needed, true = not needed
                if (((nextPosition.x - originalPosition.x) > 0) &&
                    ((nextPosition.z - originalPosition.z) > 0))
                {
                    CameraPositionFactor(3, false);
                    //Debug.Log("top right");
                }
                else if (((nextPosition.x - originalPosition.x) < 0) &&
                         ((nextPosition.z - originalPosition.z) > 0))
                {
                    CameraPositionFactor(2, false);
                    //Debug.Log("top left");
                }
                else if (((nextPosition.x - originalPosition.x) > 0) &&
                         ((nextPosition.z - originalPosition.z) < 0))
                {
                    CameraPositionFactor(0, false);
                    //Debug.Log("bottom right");
                }
                else if (((nextPosition.x - originalPosition.x) < 0) &&
                         ((nextPosition.z - originalPosition.z) < 0))
                {
                    CameraPositionFactor(1, false);
                    //Debug.Log("bottom left");
                }

                // Reset update delay
                updateDelay = 0;
            }
        }

        updateDelay += Time.deltaTime;
    }

    // The camera position affects which transforms are needed to display the character properly
    // int dir = direction of movement when camera position is FRONT
    // bool keepDir = keep direction or make adjustments
    private void CameraPositionFactor(int dir, bool keepDir)
    {
        ViewDirection cameraDirection = cameraTransform.parent.gameObject.GetComponent<Rotator>().CurrentDirection;

        switch (cameraDirection)
        {
            case ViewDirection.FRONT:
                avatarBotRight.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(0);
                avatarBotLeft.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(0);
                avatarTopLeft.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(0);
                avatarTopRight.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(0);
                SetDirectionActive(dir);
                break;
            case ViewDirection.RIGHT:
                avatarBotRight.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(1);
                avatarBotLeft.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(1);
                avatarTopLeft.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(1);
                avatarTopRight.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(1);
                if (!keepDir) { dir = dir - 3; }
                SetDirectionActive(dir);
                break;
            case ViewDirection.BACK:
                avatarBotRight.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(2);
                avatarBotLeft.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(2);
                avatarTopLeft.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(2);
                avatarTopRight.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(2);
                if (!keepDir) { dir = dir - 2; }
                SetDirectionActive(dir);
                break;
            case ViewDirection.LEFT:
                avatarBotRight.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(3);
                avatarBotLeft.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(3);
                avatarTopLeft.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(3);
                avatarTopRight.gameObject.GetComponent<AvatarTransformHelper>().SetTransforms(3);
                if (!keepDir) { dir = dir - 1; }
                SetDirectionActive(dir);
                break;
            default:
                Debug.Log("No camera direction");
                break;
        }
    }

    // Set character direction and correct sprites active
    private void SetDirectionActive(int direction)
    {
        if (direction == -1) { direction = 3; }
        if (direction == -2) { direction = 2; }
        if (direction == -3) { direction = 1; }

        avatarList[direction].SetActive(true);
        if (playerController.animator != avatarList[direction].GetComponentInChildren<Animator>()) playerController.animator = avatarList[direction].GetComponentInChildren<Animator>();

        for (int i = 0; i < avatarList.Count; i++)
        {
            if(i != direction) { avatarList[i].SetActive(false); }
        }

        avatarDirection = (AvatarDirection)direction;
    }

    private void OnEnable()
    {
        // Add listeners to rotate buttons
        EventManager.OnRotateAvatarRight += RotateAvatarRight;
        EventManager.OnRotateAvatarLeft += RotateAvatarLeft;
    }

    private void OnDisable()
    {
        // Remove listeners from rotate buttons
        EventManager.OnRotateAvatarRight -= RotateAvatarRight;
        EventManager.OnRotateAvatarLeft -= RotateAvatarLeft;
    }

    // Rotate avatar when right button is pressed, true for right
    private void RotateAvatarRight()
    {
        StartCoroutine(AvatarRotateDelay(true));
    }

    // Rotate avatar when left button is pressed, false for left
    private void RotateAvatarLeft()
    {
        StartCoroutine(AvatarRotateDelay(false));
    }

    // Avatar rotation with delay
    IEnumerator AvatarRotateDelay(bool dir)
    {
        int rotDir;

        yield return new WaitForSeconds(rotateAvatarDelay);

        if (dir)
        {
            if (avatarDirection == AvatarDirection.TOP_RIGHT) { rotDir = 0; }
            else { rotDir = (int)avatarDirection; rotDir += 1; }
            CameraPositionFactor(rotDir, true);
        }
        else
        {
            if (avatarDirection == AvatarDirection.BOT_RIGHT) { rotDir = 3; }
            else { rotDir = (int)avatarDirection; rotDir -= 1; }
            CameraPositionFactor(rotDir, true);
        }

        updateDelay = updateDelayAmount;
    }

    // List avatar directions for indexing
    private void MakeAvatarList()
    {
        avatarList = new List<GameObject>();
        avatarList.Clear();

        avatarList.Add(avatarBotRight);
        avatarList.Add(avatarBotLeft);
        avatarList.Add(avatarTopLeft);
        avatarList.Add(avatarTopRight);
    }

    // Make avatar face bottom right when the app is started
    private void SetStartingDirection(int dir)
    {
        avatarDirection = (AvatarDirection)dir;
        CameraPositionFactor(dir, true);
    }
}
