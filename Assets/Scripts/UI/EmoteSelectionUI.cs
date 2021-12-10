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

    // For that weird chat command sending stuff..
    public Chat chatHandle;

    private void Start()
    {
        if (chatHandle == null)
            Debug.LogError("No Chat object specified for the EmoteSelectionUI! (You need to explicitly specify the scene's Chat object for the EmoteSelectionUI)");
        
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

        // Send emote command through chat for everyone else to see
        chatHandle.SubmitMessage("func_emote(" + i.ToString() + ")");
    }

    private void OpenSelectionScreen()
    {
        selectionUIContainer.SetActive(!selectionUIContainer.activeSelf);
    }
}
