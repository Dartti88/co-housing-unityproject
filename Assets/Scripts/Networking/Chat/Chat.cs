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
    int messageFontSize = 40;
    const float chatBoxWidth = 0.0f;

    float updateCooldown = 0.0f;
    const float maxUpdateCooldown = 3.0f;

    // This is true for one frame -> if added so many messages, we need to scroll to the bottom, to view the latest ones
    bool forceScrolling = false;

    // True, if this is the first chat update (blocks triggering old emotes, etc)
    bool initialUpdate = true;

    RealTimeController realTimeController;
    Dictionary<string, System.Action<int, string>> commands;
    const string command_funcIdentifier = "func_";

    private void OnEnable()
    {
        realTimeController = FindObjectOfType<RealTimeController>();
        commands = new Dictionary<string, System.Action<int, string>>();
        commands.Add("emote", Command_Emote);
        commands.Add("changeAvatar", Command_ChangeAvatar);

        rect_chatBoxContent = obj_chatBoxContent.GetComponent<RectTransform>();
        font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        var se = new InputField.SubmitEvent();
        se.AddListener(SubmitMessage);
        inputField_enterMessage.onEndEdit = se;

        profileHandler = Client.Instance.profileHandler.GetComponent<ProfileHandler>();
    }

    private void Awake()
    {
        
    }

    void Start()
    {
        
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
                Client.Instance.BeginRequest_GetChatMessages(OnCompletion_GetChatMessages);
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

    void OnCompletion_GetChatMessages(string response)
    {
        string json = "{\"messages\": " + response + "}";
        if (response.Length > 0)
        {
            try
            {
                messagesContainer = JsonUtility.FromJson<ChatMessagesContainer>(json);
                UpdateChatBoxMessages();
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
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
        textComponent.fontSize = messageFontSize;

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
    // Updates only the newest messages to the chat box
    public void UpdateChatBoxMessages()
    {
        // Preceed only if we detect new messages in the "possibly updated" messagesContainer..
        if (!(objs_messages.Count < messagesContainer.messages.Length))
        {
            Debug.Log("Chat update rejected by client!");
            return;
        }
        
        //ResetChatMessageBox();

        // Add only the newest messages
        for (int i = objs_messages.Count; i < messagesContainer.messages.Length; ++i)
        {
            // This statement prevents old messages from displaying
            if (initialUpdate)
            {
                objs_messages.Add(null);
                continue;
            }
            ChatMessage m = messagesContainer.messages[i];
            
            if (!m.message.Contains(command_funcIdentifier))
                AddMessageToChatBox(m.displayName, m.message);
            else
                ParseAndExecuteCommandMessage(m.displayName, m.message);
        }

        forceScrolling = true;
        initialUpdate = false;
    }


    void ParseAndExecuteCommandMessage(string displayName, string message)
    {
        int profileID = Client.Instance.GetIDByDisplayName(displayName);
        // parse command name
        int start_commandName = message.IndexOf(command_funcIdentifier, 0) + command_funcIdentifier.Length;
        int end_commandName = message.IndexOf("(", start_commandName);    
        string commandName = message.Substring(start_commandName, end_commandName - start_commandName);

        // parse command parameters
        int end_params = message.IndexOf(")", end_commandName);
        string parameters = message.Substring(end_commandName + 1, (end_params - end_commandName) - 1);

        commands[commandName](profileID, parameters);
    }

    void Command_Emote(int profileID, string parameters)
    {
        // Currently this can be used only for non local users!
        //  -> ignore, if local profileID
        if (profileID == profileHandler.userProfile.profileID)
            return;

        // make sure the params are correct
        if (parameters.Contains(","))
        {
            Debug.Log("ERROR >> Chat commands @Command_Emote : invalid parameters");
            return;
        }
        realTimeController.TriggerEmote(profileID, Int32.Parse(parameters));
    }

    void Command_ChangeAvatar(int profileID, string parameters)
    {
        // Currently this can be used only for non local users!
        //  -> ignore, if local profileID
        if (profileID == profileHandler.userProfile.profileID)
            return;

        // make sure the params are correct
        if (parameters.Contains(","))
        {
            Debug.Log("ERROR >> Chat commands @Command_Emote : invalid parameters");
            return;
        }
        realTimeController.TriggerAvatarSwitch(profileID, Int32.Parse(parameters));
    }
}
