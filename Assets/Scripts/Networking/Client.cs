using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;




public abstract class DatabaseEntry
{
    public bool RequestInProgress { get; protected set; } = false;
    public bool IsAvailable { get; protected set; } = false;
    public abstract void Init();
    public abstract void SetString(int index, string data);
}

public class ProfileDataTest : DatabaseEntry
{
    public string[] data_str;

    public override void Init()
    {
        data_str = new string[1];
        RequestInProgress = true;
        IsAvailable = false;
    }

    public override void SetString(int index, string data)
    {
        data_str[index] = data;
        RequestInProgress = false;
        IsAvailable = true;
    }

    public string GetUsername()
    {
        return data_str[0];
    }
}


public class Client : MonoBehaviour
{
    Profile newTestProfile;

    // Start is called before the first frame update
    void Start()
    {
        newTestProfile = new Profile(1, 2, "UnityTestUser", "Uuser", "password", "asd123", 3, 0, Profile.ProfileType.Guest, DateTime.Now);
    }

    
    bool exec = true;
    // Update is called once per frame
    void Update()
    {
        if (exec)
        {
            BeginRequest_GetAllProfiles();
            //BeginRequest_AddNewProfile(newTestProfile);
            //BeginRequest_ValidatePassword("TestUser", "1234");
            exec = false;
        }
    }


    public void BeginRequest_GetAllProfiles()
    {
        UnityWebRequest req = WebRequests.CreateWebRequest_GET(WebRequests.URL_GET_Profiles, "application/json");
        StartCoroutine(SendWebRequest(req, UpdateProfilesFromDatabase));
    }

    public void BeginRequest_AddNewProfile(Profile newProfile)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_userName", "\"" + newProfile.userName + "\""));
        form.Add(new MultipartFormDataSection("key_displayName", "\"" + newProfile.displayName + "\""));
        form.Add(new MultipartFormDataSection("key_password", "\"" + newProfile.password + "\""));
        
        form.Add(new MultipartFormDataSection("key_apartmentID", newProfile.apartmentID.ToString()));
        form.Add(new MultipartFormDataSection("key_communityID", newProfile.communityID.ToString()));
        form.Add(new MultipartFormDataSection("key_avatarID", newProfile.avatarID.ToString()));
        
        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_CreateNewProfile, form);
        StartCoroutine(SendWebRequest(req, NotifyOn_AddedNewProfileComplete));
    }

    public void BeginRequest_ValidatePassword(string username, string password)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_userName", "\"" + username + "\""));
        form.Add(new MultipartFormDataSection("key_password", "\"" + password + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_ValidatePassword, form);
        StartCoroutine(SendWebRequest(req, NotifyOn_PasswordValidationComplete));
    }


    IEnumerator SendWebRequest(UnityWebRequest req, System.Action<UnityWebRequest> onCompletionCallback)
    {
        Debug.Log("Sending web request to the server...");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(req.error);
        else
        {
            onCompletionCallback(req);
        }
    }


    void UpdateProfilesFromDatabase(UnityWebRequest req)
    {
        string json = "{\"profiles\": " + req.downloadHandler.text + "}";

        DataController.ProfilesContainer profileList = DataController.Instance.profile_list;
        profileList = JsonUtility.FromJson<DataController.ProfilesContainer>(json);

        foreach (Profile p in profileList.profiles)
        {
            Debug.Log("====================================================\n");
            Debug.Log(p.id);
            Debug.Log(p.userName);
            Debug.Log(p.password);
            Debug.Log(p.apartmentID);
            Debug.Log(p.communityID);
            Debug.Log(p.displayName);
            Debug.Log(p.avatarID);
            Debug.Log(p.credits);
            Debug.Log(p.socialScore);
            Debug.Log(p.profileType);
            Debug.Log(p.creationDate);
            Debug.Log(p.description);
            Debug.Log("====================================================\n");
        }
    }

    // This is called when server responds to post new profile request
    void NotifyOn_AddedNewProfileComplete(UnityWebRequest req)
    {
        Debug.Log("Server >> " + req.downloadHandler.text);
    }

    void NotifyOn_PasswordValidationComplete(UnityWebRequest req)
    {
        Debug.Log("Server >> " + req.downloadHandler.text);
    }

    /*
    async Task<UnityWebRequestAsyncOperation> GetFromDatabaseAsync()
    {
        Debug.Log("Getting data from server...");

        //Task<int> asyncTask = DoGetDatabaseAsync();
        Task<UnityWebRequestAsyncOperation> asyncTask = new Task<UnityWebRequestAsyncOperation>(() =>
        {
            UnityWebRequest testRequest = WebRequests.CreateWebRequest(WebRequests.URL_test);
            return testRequest.SendWebRequest();
        });
        asyncTask.Start();
        

        return await asyncTask;
    }*/

}
