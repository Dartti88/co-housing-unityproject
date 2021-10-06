using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteSelectionUI : MonoBehaviour
{
    public Button selectEmoteButton;

    public GameObject selectionUIContainer;
    public GameObject selectionBackground;

    public List<Button> emoteIconButtons = new List<Button>();

    public GameObject emoteGO;

    private void Start()
    {
        selectEmoteButton.onClick.AddListener(OpenSelectionScreen);

        FillSelectionGrid();
    }

    private void FillSelectionGrid()
    {
        for (int i = 0; i < emoteIconButtons.Count; i++)
        {
            int tmp = i;

            emoteIconButtons[i].onClick.AddListener(delegate { EmoteButtonOnClick(tmp); });
        }
    }

    private void EmoteButtonOnClick(int i)
    {
        emoteGO.GetComponent<EmoteBillboard>().UseEmote(i);

        selectionUIContainer.SetActive(false);
    }

    private void OpenSelectionScreen()
    {
        selectionUIContainer.SetActive(!selectionUIContainer.activeSelf);
    }
}
