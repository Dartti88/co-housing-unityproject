using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelectionUI : MonoBehaviour
{
    public Button selectAvatarButton;

    public GameObject selectionUIContainer;
    public GameObject selectionBackground;
    public GameObject gridElementPrefab;

    private List<GameObject> avatarPrefabs;
    private PlayerController playerController;
    private List<GameObject> avatarIconButtons = new List<GameObject>();

    private void Start()
    {
        selectAvatarButton.onClick.AddListener(OpenSelectionScreen);
        playerController = PlayerController.Instance;
        avatarPrefabs = playerController.avatars;

        FillSelectionGrid();
    }

    private void FillSelectionGrid()
    {
        avatarIconButtons.Clear();

        for (int i = 0; i < avatarPrefabs.Count; i++)
        {
            GameObject newGridElement = Instantiate(gridElementPrefab,
                                                    gridElementPrefab.transform.position,
                                                    gridElementPrefab.transform.rotation);
            newGridElement.transform.SetParent(selectionBackground.transform, false);
            newGridElement.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = 
                avatarPrefabs[i].GetComponent<AvatarController>().fullPictureFront;

            int tmp = i;
            newGridElement.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate { AvatarButtonOnClick(tmp); });

            avatarIconButtons.Add(newGridElement.transform.GetChild(0).gameObject);
        }
    }

    private void AvatarButtonOnClick(int i)
    {
        playerController.ChangePlayerAvatar(i);
    }

    private void OpenSelectionScreen()
    {
        selectionUIContainer.SetActive(!selectionUIContainer.activeSelf);
    }
}
