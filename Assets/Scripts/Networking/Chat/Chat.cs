using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{

    public InputField inputField_enterMessage;
    ProfileHandler profileHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        var se = new InputField.SubmitEvent();
        se.AddListener(SubmitMessage);
        inputField_enterMessage.onEndEdit = se;

        profileHandler = Client.Instance.profileHandler.GetComponent<ProfileHandler>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    

    // When submitting "directly" from input field
    public void SubmitMessage(string message)
    {
        if (message.Length > 0)
        {
            Debug.Log("Attempting to send: " + message);
            Client.Instance.BeginRequest_SubmitChatMessage(profileHandler.userProfile.displayName, message, OnCompletionCallback_SubmitMessage);
        }
    }

    void OnCompletionCallback_SubmitMessage(string response)
    {
        Debug.Log("OnCompletionCallback_SubmitMessage : Server response: " + response);
    }
}
