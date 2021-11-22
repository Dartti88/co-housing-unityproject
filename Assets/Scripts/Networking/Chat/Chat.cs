using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class ChatMessage
{
    public string displayName;
    public string message;
}
[Serializable]
public class ChatMessagesContainer
{
    public ChatMessage[] messages;
}
public class Chat : MonoBehaviour
{

    public InputField inputField_enterMessage;
    public GameObject obj_chatBoxContent;
    public Scrollbar verticalScrollbar;

    public List<GameObject> objs_messages; // Each message's gameobject

    public ChatMessagesContainer messagesContainer; // Container holding each message's data (similar to profilesContainer)

    RectTransform rect_chatBoxContent;

    ProfileHandler profileHandler;
    Font font;

    int messageCount = 0;
    float messageFontSize = 14.0f;
    const float chatBoxWidth = 0.0f;

    float updateCooldown = 0.0f;
    const float maxUpdateCooldown = 3.0f;

    // This is true for one frame -> if added so many messages, we need to scroll to the bottom, to view the latest ones
    bool forceScrolling = false;

    // Start is called before the first frame update
    void Start()
    {
        rect_chatBoxContent = obj_chatBoxContent.GetComponent<RectTransform>();
        font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        var se = new InputField.SubmitEvent();
        se.AddListener(SubmitMessage);
        inputField_enterMessage.onEndEdit = se;

        profileHandler = Client.Instance.profileHandler.GetComponent<ProfileHandler>();

        /*AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        AddMessageToChatBox("TestUser", "Test spam message");
        */
    }

    void Update()
    {
        if (Client.Instance.isLoggedIn)
        {
            if (updateCooldown <= 0)
            {
                updateCooldown = maxUpdateCooldown;
                Client.Instance.BeginRequest_GetChatMessages(null);
                Debug.Log("Chat update triggered...");
            }
            else
            {
                updateCooldown -= 1.0f * Time.deltaTime;
            }

            // Force to scroll to the bottom of the chat messages, if new messages added..
            if (forceScrolling && verticalScrollbar.value > 0.0f)
            {
                verticalScrollbar.value = 0;
                forceScrolling = false;
            }
        }
    }

    
    

    // When submitting "directly" from input field
    public void SubmitMessage(string message)
    {
        if (message.Length > 0)
        {
            Debug.Log("Attempting to send: " + message);
            Client.Instance.BeginRequest_SubmitChatMessage(profileHandler.userProfile.displayName, message, OnCompletionCallback_SubmitMessage);
            // Clear prev text from the input field..
            inputField_enterMessage.text = "";
        }
    }

    void OnCompletionCallback_SubmitMessage(string response)
    {
        Debug.Log("OnCompletionCallback_SubmitMessage : Server response: " + response);
    }

    // Adds a new message to chatbox and resizes its' "content"
    void AddMessageToChatBox(string displayName, string message)
    {
        GameObject newTextObj = new GameObject("Text_message");
        newTextObj.transform.parent = obj_chatBoxContent.transform;

        Vector3 position = newTextObj.transform.localPosition;
        float newTextPos = messageCount * messageFontSize;
        position.y = -newTextPos;
        position.x = 0;

        newTextObj.transform.localPosition = position;

        Text textComponent = newTextObj.AddComponent<Text>();

        textComponent.font = font;
        textComponent.text = displayName + ": " + message;
        textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;

        RectTransform textRectTransform = newTextObj.GetComponent<RectTransform>();
        textRectTransform.anchorMin = new Vector2(0, 1);
        textRectTransform.anchorMax = new Vector2(0, 1);
        textRectTransform.pivot = new Vector2(0, 1);

        // remember to strech the content thing..
        Vector2 contentSize = rect_chatBoxContent.sizeDelta;
        contentSize.y += messageFontSize;
        rect_chatBoxContent.sizeDelta = contentSize;

        objs_messages.Add(newTextObj);

        messageCount++;
    }

    // Clears all text in the chat box..
    public void ResetChatMessageBox()
    {
        rect_chatBoxContent.sizeDelta = new Vector2(0, messageFontSize);

        foreach (GameObject textObj in objs_messages)
        {
            GameObject.Destroy(textObj);
        }
        objs_messages.Clear();

        messageCount = 0;
    }
    // Updates all text in the whole chat box (fucking inefficient and dumb..)
    public void UpdateChatBoxMessages()
    {
        // Preceed only if we detect new messages in the "possibly updated" messagesContainer..
        if (!(objs_messages.Count < messagesContainer.messages.Length))
        {
            Debug.Log("Chat update rejected by client!");
            return;
        }

        ResetChatMessageBox();

        foreach (ChatMessage m in messagesContainer.messages)
        {
            AddMessageToChatBox(m.displayName, m.message);
        }

        forceScrolling = true;
    }
}
