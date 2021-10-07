using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;



public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }
    
    [Serializable]
    public class ProfilesContainer
    {
        public Profile[] profiles;
    }

    public ProfilesContainer profile_list = new ProfilesContainer();
    public Dictionary<int, Task> task_list = new Dictionary<int, Task>();
    public Dictionary<int, Task> acceptedTasks_list = new Dictionary<int, Task>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this);
        }
    }

    Profile testProfile;
    Task newTestTask;

    // Start is called before the first frame update
    void Start()
    {
        testProfile = new Profile(0, 0, "TestUser", "Jorma", "perse", "This is the first test user test update testing update", 3, 420, 0, Profile.ProfileType.Resident, DateTime.Now);
        testProfile.id = 1;

        // description changes..
        // avatarID: 1 => 3

        newTestTask = new Task();
        newTestTask.taskID = 69;
        newTestTask.creatorID = 3;
        newTestTask.taskName = "Another Task";
        newTestTask.targetID = 2;
        newTestTask.description = "Testing does task names and points work..";
        newTestTask.cost = 1;
        newTestTask.points = 420;
        newTestTask.quantity = 5;
        newTestTask.uniqueQuantity = 5;
        newTestTask.expirationDate = "2025-09-12";
    }

    bool exec = true;
    // Update is called once per frame
    void Update()
    {
        if (exec)
        {
            // profileID = 1 -> username = TestUser, displayName = Jorma

            //BeginRequest_UpdateProfile(testProfile, null);
            //BeginRequest_CompleteTask(2, 9, null);
            //BeginRequest_AcceptTask(2, 9, null);
            //BeginRequest_GetAcceptedTasks(1, null);
            //BeginRequest_GetAvailableTasks(null);
            //BeginRequest_AddNewTask(newTestTask, null);

            //BeginRequest_GetAllProfiles(null);
            //BeginRequest_AddNewProfile(newTestProfile);
            BeginRequest_ValidatePassword("Joel", "Peruna", Test);
            exec = false;
        }
    }
    
    public string GetDisplayNameById(int id)
    {
        for (int i=0, list_size = profile_list.profiles.Length; i < list_size; ++i)
            {
            if (profile_list.profiles[i].id == id)
                {
                return profile_list.profiles[i].displayName;
                }
            }
        return null;
    }

    void Test(string a)
    {
        Debug.Log("Login TEST: " + a);
    }
    
    // PUBLIC PROFILE STUFF ------------------------- PUBLIC PROFILE STUFF ------------------------- PUBLIC PROFILE STUFF
    public void BeginRequest_GetAllProfiles(System.Action<string> onCompletionCallback)
    {
        UnityWebRequest req = WebRequests.CreateWebRequest_GET(WebRequests.URL_GET_Profiles, "application/json");
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_UpdateProfilesFromDatabase));
    }

    public void BeginRequest_AddNewProfile(Profile newProfile, System.Action<string> onCompletionCallback)
    {
        if (newProfile.userName.Length <= 0)
        {
            Debug.Log("ERROR: BeginRequest_AddNewProfile(Profile newProfile, System.Action<string> onCompletionCallback)\n>>userName was empty");
            return;
        }
        else if (newProfile.displayName.Length <= 0)
        {
            Debug.Log("ERROR: BeginRequest_AddNewProfile(Profile newProfile, System.Action<string> onCompletionCallback)\n>>displayName was empty");
            return;
        }
        else if (newProfile.password.Length <= 0)
        {
            Debug.Log("ERROR: BeginRequest_AddNewProfile(Profile newProfile, System.Action<string> onCompletionCallback)\n>>password was empty");
            return;
        }

        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_userName", "\"" + newProfile.userName + "\""));
        form.Add(new MultipartFormDataSection("key_displayName", "\"" + newProfile.displayName + "\""));
        
        form.Add(new MultipartFormDataSection("key_apartmentID", newProfile.apartmentID.ToString()));
        form.Add(new MultipartFormDataSection("key_communityID", newProfile.communityID.ToString()));
        form.Add(new MultipartFormDataSection("key_avatarID", newProfile.avatarID.ToString()));

        form.Add(new MultipartFormDataSection("key_password", "\"" + newProfile.password + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_CreateNewProfile, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_AddedNewProfileComplete));
    }

    public void BeginRequest_ValidatePassword(string username, string password, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_userName", "\"" + username + "\""));
        form.Add(new MultipartFormDataSection("key_password", "\"" + password + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_ValidatePassword, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, null));
    }

    public void BeginRequest_UpdateProfile(Profile profileToUpdate, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", "\"" + profileToUpdate.id.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_password", "\"" + profileToUpdate.password + "\""));
        form.Add(new MultipartFormDataSection("key_avatarID", "\"" + profileToUpdate.avatarID.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_description", "\"" + profileToUpdate.description + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_UpdateProfile, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_UpdateProfileComplete));
    }

    // PUBLIC TASKS STUFF ------------------------- PUBLIC TASKS STUFF ------------------------- PUBLIC TASKS STUFF
    public void BeginRequest_GetAvailableTasks(System.Action<string> onCompletionCallback)
    {
        UnityWebRequest req = WebRequests.CreateWebRequest_GET(WebRequests.URL_GET_AvailableTasks, "application/json");
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_UpdateAvailableTasksFromDatabase));
    }
    public void BeginRequest_GetAcceptedTasks(int profileID, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", "\"" + profileID.ToString() + "\""));
        
        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_GetAcceptedTasks, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_UpdateAcceptedTasksFromDatabase));
    }

    public void BeginRequest_AddNewTask(Task task, System.Action<string> onCompletionCallback)
    {
        if (task.description.Length <= 0)
        {
            Debug.Log("ERROR: BeginRequest_AddNewTask(Task task, System.Action<string> onCompletionCallback)\n>>Task description was empty");
            return;
        }

        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", "\"" + task.creatorID.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_targetID", "\"" + task.targetID.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_cost", "\"" + task.cost.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_quantity", "\"" + task.quantity.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_uniqueQuantity", "\"" + task.uniqueQuantity.ToString() + "\""));

        // NOTE*: Make the date be in sql format (YYYY-MM-DD)
        form.Add(new MultipartFormDataSection("key_expirationDate", "\"" + task.expirationDate + "\""));

        form.Add(new MultipartFormDataSection("key_description", "\"" + task.description + "\""));
        form.Add(new MultipartFormDataSection("key_name", "\"" + task.taskName + "\""));

        form.Add(new MultipartFormDataSection("key_points", "\"" + task.points + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_CreateNewTask, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_AddedNewTaskComplete));
    }

    public void BeginRequest_AcceptTask(int profileID, int taskID, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", "\"" + profileID.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_taskID", "\"" + taskID.ToString() + "\""));
        
        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_AcceptTask, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_AcceptTaskComplete));
    }

    public void BeginRequest_CompleteTask(int profileID, int taskID, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", "\"" + profileID.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_taskID", "\"" + taskID.ToString() + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_CompleteTask, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_CompleteTaskComplete));
    }


    // ALL INTERNAL FUNCS ->

    // INTERNAL PROFILES STUFF ------------------------- INTERNAL PROFILES STUFF ------------------------- INTERNAL PROFILES STUFF
    void Internal_OnCompletion_UpdateProfilesFromDatabase(UnityWebRequest req)
    {
        string json = "{\"profiles\": " + req.downloadHandler.text + "}";
        profile_list = JsonUtility.FromJson<ProfilesContainer>(json);
    }

    // This is called when server responds to post new profile request
    void Internal_OnCompletion_AddedNewProfileComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_AddedNewProfileComplete(UnityWebRequest req)");
    }

    void Internal_OnCompletion_PasswordValidationComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_PasswordValidationComplete(UnityWebRequest req)");
    }

    void Internal_OnCompletion_UpdateProfileComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_UpdateProfileComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }

    // INTERNAL TASKS STUFF ------------------------- INTERNAL TASKS STUFF ------------------------- INTERNAL TASKS STUFF
    class TempTaskList
    {
        public Task[] tasks;
    }
    void Internal_OnCompletion_UpdateAvailableTasksFromDatabase(UnityWebRequest req)
    {
        string json = "{\"tasks\": " + req.downloadHandler.text + "}";
        task_list = JsonUtility.FromJson<TempTaskList>(json).tasks.ToDictionary(t => t.creatorID);
        Debug.Log("Internal_OnCompletion_UpdateAvailableTasksFromDatabase(UnityWebRequest req)");
    }
    void Internal_OnCompletion_UpdateAcceptedTasksFromDatabase(UnityWebRequest req) // !!! NOT TESTED YET !!!
    {
        string json = "{\"tasks\": " + req.downloadHandler.text + "}";
        acceptedTasks_list = JsonUtility.FromJson<TempTaskList>(json).tasks.ToDictionary(t => t.creatorID);
        Debug.Log("Internal_OnCompletion_UpdateAcceptedTasksFromDatabase(UnityWebRequest req)");
    }
    void Internal_OnCompletion_AddedNewTaskComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_AddedNewTaskComplete(UnityWebRequest req)");
    }

    void Internal_OnCompletion_AcceptTaskComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_AcceptTaskComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }

    void Internal_OnCompletion_CompleteTaskComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_CompleteTaskComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }

    // COMMON ->
    IEnumerator SendWebRequest(UnityWebRequest req, System.Action<string> onCompletionCallback, System.Action<UnityWebRequest> internalCallback)
    {
        Debug.Log("Sending web request to the server...");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(req.error);
        else
        {
            if (internalCallback != null)
                internalCallback.Invoke(req);

            if (onCompletionCallback != null)
                onCompletionCallback.Invoke(req.downloadHandler.text);
        }
    }
}