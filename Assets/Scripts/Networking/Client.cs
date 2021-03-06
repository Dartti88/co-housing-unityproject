using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;


/*

*Kaikki n?m? pyynn?t ei mene aina ensimm?isell? kerralla l?pi, koska tuo serveri on paska:D
V?lill? sielt? tulee onCompletionCallback:iin jotakin, miss? lukee esim. "access violation 
or syntax error" ja yleens? jossakin kohtaa error koodi 1064, tai joku "server timed out".
N?iss? tilanteissa mik??n ei yleens? ole rikki, vaan tuo pyynt? pit?? vain laittaa menem??n uudestaan.

Taskeihin liittyv?t:
    BeginRequest_CompleteTask(profileID, taskID, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Merkkaa serverille, ett? t?m? profiili on suorittanut taskin
        *Ei kuitenkaan koske mihink??n paikallisiin listoihin / systeemeihin
        (*Jos halutaan p?ivitt?? paikallisia listoja ottamaan t?m?n muutoksen huomioon, 
        pit?? kutsua GetAcceptedTasks, tai GetAvailableTasks, riippuen, mit? halutaan tehd?)
        
    BeginRequest_AcceptTask(profileID, taskID, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Merkkaa serverille, ett? profiili on hyv?ksynyt taskin
        *Ei kuitenkaan koske mihink??n paikallisiin listoihin / systeemeihin
        (*Jos halutaan p?ivitt?? paikallisia listoja ottamaan t?m?n muutoksen huomioon, 
        pit?? kutsua GetAcceptedTasks, tai GetAvailableTasks, riippuen, mit? halutaan tehd?)
    
    BeginRequest_GetAvailableTasks(onCompletionCallback);
        *P?ivitt?? Client.Instance.task_list - dictionaryn

    BeginRequest_GetAcceptedTasks(profileID, onCompletionCallback);
        *P?ivitt?? Client.Instance.acceptedTasks_list - dictionaryn
    
    BeginRequest_GetCreatedTasks(profileID, onCompletionCallback);
        *Hakee kaikki t?m?n profiilin tekem?t taskit
        *P?ivitt?? Client.Instance.createdTasks_list - dictionaryn

    BeginRequest_AddNewTask(Task, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Lis?? uuden taskin serverille
        *T?m?k??n ei koske mihink??n paikallisiin listoihin / systeemeihin
        (*Jos halutaan p?ivitt?? paikallisia listoja ottamaan t?m?n muutoksen huomioon, 
        pit?? kutsua GetAcceptedTasks, tai GetAvailableTasks, riippuen, mit? halutaan tehd?)
        
    BeginRequest_RemoveTask(userName, password, taskID, onCompletionCallback)
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Poistaa taskin serverilt?
        (*T?m?k??n ei koske mihink??n paikallisiin listoihin)

Profiileihin liittyv?t:
    BeginRequest_GetAllProfiles(onCompletionCallback);
        *P?ivitt?? Client.Instance.profile_list.profiles
        *HUOM!: tuo Client.Instance.profile_list on ProfilesContainer - objekti, joka pit?? sis?ll??n taulukon profiileista
        
    BeginRequest_AddNewProfile(Profile);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Lis?? uuden profiilin serverille
        (*Ei koske mihink??n paikalliseen dataan. Pit?? kutsua GetAllProfiles, jos halutaan, ett? muutos n?kyy t?m?n j?lkeen paikallisesti)

    BeginRequest_UpdateProfile(Profile, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *P?ivitt?? profiilin tietoja serverille.
        *T?ll? sis??n sy?tett?v?ll? profiililla on oltava oikea userName ja password, ett? t?m? muutos hyv?ksyt??n serverill?
        (*Ei koske mihink??n paikalliseen dataan)

    BeginRequest_ValidatePassword(userName, password, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Testaa l?ytyyk? serverilt? userName - password matchi?
        (*Ei koske mihink??n paikalliseen dataan)
*/

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }

    // This is used for the "pseudo-real time communications hacking" thing
    //private RealTimeController realTimeController;

    [Serializable]
    public class ProfilesContainer
    {
        public Profile[] profiles;
    }
    
    public ProfilesContainer profile_list = new ProfilesContainer();

    public Dictionary<int, Task> task_list =            new Dictionary<int, Task>();
    public Dictionary<int, Task> acceptedTasks_list =   new Dictionary<int, Task>();
    public Dictionary<int, Task> createdTasks_list =    new Dictionary<int, Task>();

    // Prefab for all the other players except for the local player (Need this to spawn other players)
    public GameObject profileHandler;
    public ProfileHandler pHandler;

    public bool isLoggedIn = false;
    public int floor = 0;
    // omg this so fukin dumb.. but necessary..
    public Vector3 initLocalPlayerPos = new Vector3(0, 0, 0);

    const string MESSAGE_ERROR_IDENTIFIER = "Error";
    const char MESSAGE_CUSTOM_DATA_SEPARATOR = ';';

    public enum RequestName
    {
        GetAllProfiles
    }
    public Dictionary<RequestName, bool> RequestInProgress { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            pHandler = profileHandler.GetComponent<ProfileHandler>();
            RequestInProgress = new Dictionary<RequestName, bool>();
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this);
        }
    }

    Profile testProfile;
    Task newTestTask;
    Task newTestTask2;
    Task newTestTask3;

    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame

    void Update()
    {

        /*if (roomBookingTest)
        {
            BeginRequest_GetRoomBookings("ConcertDanceHall", null);
            BeginRequest_MakeRoomBooking("Bookings_ConcertDanceHall", 6, "2021-12-24", "IsStillWorkingTest", null);
            roomBookingTest = false;
        }
        */
    }
    
    public string GetDisplayNameById(int id)
    {
        for (int i=0, list_size = profile_list.profiles.Length; i < list_size; ++i)
            {
            if (profile_list.profiles[i].profileID == id)
                {
                return profile_list.profiles[i].displayName;
                }
            }
        return null;
    }

    public int GetIDByDisplayName(string displayName)
    {
        return Array.Find(profile_list.profiles, e => e.displayName == displayName).profileID;
    }
    
    
    // PUBLIC PROFILE STUFF ------------------------- PUBLIC PROFILE STUFF ------------------------- PUBLIC PROFILE STUFF
    public void BeginRequest_GetAllProfiles(System.Action<string> onCompletionCallback)
    {
        RequestInProgress[RequestName.GetAllProfiles] = true;

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
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_ValidatePasswordComplete));
    }
    public void BeginRequest_LogOut(System.Action<string> onCompletionCallback)
    {
        // Need handle to the local player's transform, to save its' last pos..
        Vector3 lastPlayerPos = FindObjectOfType<RealTimeController>().localPlayer.transform.position;
        
        string lastX_str = lastPlayerPos.x.ToString().Replace(',', '.');
        string lastY_str = lastPlayerPos.y.ToString().Replace(',', '.');
        string lastZ_str = lastPlayerPos.z.ToString().Replace(',', '.');

        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", pHandler.userProfile.profileID.ToString()));
        form.Add(new MultipartFormDataSection("key_x", lastX_str));
        form.Add(new MultipartFormDataSection("key_y", lastY_str));
        form.Add(new MultipartFormDataSection("key_z", lastZ_str));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_LogOut, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_LogOutComplete));
    }

    public void BeginRequest_UpdateProfile(Profile profileToUpdate, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", "\"" + profileToUpdate.profileID.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_password", "\"" + profileToUpdate.password + "\""));
        form.Add(new MultipartFormDataSection("key_avatarID", "\"" + profileToUpdate.avatarID.ToString() + "\""));
        form.Add(new MultipartFormDataSection("key_description", "\"" + profileToUpdate.description + "\""));
        form.Add(new MultipartFormDataSection("key_displayName", "\"" + profileToUpdate.displayName + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_UpdateProfile, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_UpdateProfileComplete));
    }

    // Updates currently logged in user's data from server (gets social score(xp), credits, etc to match values from server)
    // *NOTE! Unlike most of the other "BeginRequests" here, this returns the data from server as plain text, formatted as following(';' as entry separator):
    //      "credits(float);socialScore(float)"
    public void BeginRequest_UpdateLocalProfileData(System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", pHandler.userProfile.profileID.ToString()));
        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_UpdateLocalProfileData, form, "text/plain");
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_UpdateLocalProfileDataComplete));
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

    public void BeginRequest_GetCreatedTasks(int profileID, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", "\"" + profileID.ToString() + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_GetCreatedTasks, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_UpdateCreatedTasksFromDatabase));
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
        form.Add(new MultipartFormDataSection("key_cost", task.cost.ToString())); // If adding "\"" -> we actually get double double quotes in php.. like: ""key_name""
        form.Add(new MultipartFormDataSection("key_quantity", task.quantity.ToString()));
        form.Add(new MultipartFormDataSection("key_uniqueQuantity", "\"" + task.uniqueQuantity.ToString() + "\""));

        // NOTE*: Make the date be in sql format (YYYY-MM-DD)
        form.Add(new MultipartFormDataSection("key_expirationDate", "\"" + task.expirationDate + "\""));

        form.Add(new MultipartFormDataSection("key_description", "\"" + task.description + "\""));
        form.Add(new MultipartFormDataSection("key_name", "\"" + task.taskName + "\""));

        form.Add(new MultipartFormDataSection("key_points", "\"" + task.points + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_CreateNewTask, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_AddedNewTaskComplete));
    }

    public void BeginRequest_RemoveTask(string creatorUserName, string creatorPassword, int toRemoveTaskID, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_userName", "\"" + creatorUserName + "\""));
        form.Add(new MultipartFormDataSection("key_password", "\"" + creatorPassword + "\""));
        form.Add(new MultipartFormDataSection("key_taskID", "\"" + toRemoveTaskID.ToString() + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_RemoveTask, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_RemovedTaskComplete));
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
    

    // PUBLIC ROOM BOOKING STUFF ------------------------- PUBLIC ROOM BOOKING STUFF ------------------------- PUBLIC ROOM BOOKING STUFF
    public void BeginRequest_GetRoomBookings(string roomName, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_roomName", roomName));
        
        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_GetRoomBookings, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, null));
    }
    public void BeginRequest_MakeRoomBooking(string roomName, int startTime, string date, string bookerName, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_roomName", roomName));
        form.Add(new MultipartFormDataSection("key_startTime", startTime.ToString()));
        form.Add(new MultipartFormDataSection("key_date", "\"" + date + "\""));
        form.Add(new MultipartFormDataSection("key_bookerName", "\"" + bookerName + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_MakeRoomBooking, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_MakeRoomBookingComplete));
    }
    public void BeginRequest_CancelRoomBooking(string roomName, int startTime, string date, string bookerName, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_roomName", roomName));
        form.Add(new MultipartFormDataSection("key_startTime", startTime.ToString()));
        form.Add(new MultipartFormDataSection("key_date", "\"" + date + "\""));
        form.Add(new MultipartFormDataSection("key_bookerName", "\"" + bookerName + "\""));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_CancelRoomBooking, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_CancelRoomBookingComplete));
    }

    // PUBLIC REAL TIME STUFF ------------------------- PUBLIC REAL TIME STUFF ------------------------- PUBLIC REAL TIME STUFF
    public void BeginRequest_GetProfileStatuses(System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_floor", floor.ToString()));
        
        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_GetProfileStatuses, form);
        
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_GetProfileStatuses));
    }
    public void BeginRequest_UpdateProfileStatus(int profileID, int status, Vector3 destination, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_profileID", profileID.ToString()));
        form.Add(new MultipartFormDataSection("key_status", status.ToString()));

        string destX_str = destination.x.ToString().Replace(',', '.');
        string destY_str = destination.y.ToString().Replace(',', '.');
        string destZ_str = destination.z.ToString().Replace(',', '.');

        form.Add(new MultipartFormDataSection("key_x", destX_str));
        form.Add(new MultipartFormDataSection("key_y", destY_str));
        form.Add(new MultipartFormDataSection("key_z", destZ_str));

        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_UpdateProfileStatus, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_UpdateProfileStatus));
    }

    // PUBLIC CHAT STUFF ------------------------- PUBLIC CHAT STUFF ------------------------- PUBLIC CHAT STUFF
    public void BeginRequest_SubmitChatMessage(string displayName, string message, System.Action<string> onCompletionCallback)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        form.Add(new MultipartFormDataSection("key_displayName", "\"" + displayName + "\""));
        form.Add(new MultipartFormDataSection("key_message", "\"" + message + "\""));
        
        UnityWebRequest req = WebRequests.CreateWebRequest_POST_FORM(WebRequests.URL_POST_SubmitChatMessage, form);
        StartCoroutine(SendWebRequest(req, onCompletionCallback, null));
    }
    public void BeginRequest_GetChatMessages(System.Action<string> onCompletionCallback)
    {
        UnityWebRequest req = WebRequests.CreateWebRequest_GET(WebRequests.URL_GET_GetChatMessages, "application/json");
        StartCoroutine(SendWebRequest(req, onCompletionCallback, Internal_OnCompletion_GetChatMessages));
    }

    // ALL INTERNAL FUNCS ->

    // INTERNAL PROFILES STUFF ------------------------- INTERNAL PROFILES STUFF ------------------------- INTERNAL PROFILES STUFF
    void Internal_OnCompletion_UpdateProfilesFromDatabase(UnityWebRequest req)
    {
        string json = "{\"profiles\": " + req.downloadHandler.text + "}";
        try
        {
            profile_list = JsonUtility.FromJson<ProfilesContainer>(json);
        }
        catch (ArgumentException e)
        {
            Debug.LogError(e.Message);
        }
        RequestInProgress[RequestName.GetAllProfiles] = false;
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
    // *QUITE DUMB ATM! This happens when logging in or reqistering new user, so we use this to determine, is client logged in successfully..
    void Internal_OnCompletion_ValidatePasswordComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_ValidatePasswordComplete(UnityWebRequest req)");
        isLoggedIn = !(req.downloadHandler.text == "Failed" || req.downloadHandler.text.Contains("Error"));
    }
    void Internal_OnCompletion_LogOutComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_LogOutComplete(UnityWebRequest req) " + req.downloadHandler.text);
        isLoggedIn = !(req.downloadHandler.text == "Success");
    }

    void Internal_OnCompletion_UpdateProfileComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_UpdateProfileComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
        // Quite hard to notice, but we also trigger avatar change for all other players through chat

        GameObject chatObj = GameObject.Find("Chat");
        if (chatObj != null)
        {
            Chat chatScript = chatObj.GetComponent<Chat>();
            chatScript.SubmitMessage("func_changeAvatar(" + pHandler.userProfile.avatarID.ToString() + ")");
        }
    }

    void Internal_OnCompletion_UpdateLocalProfileDataComplete(UnityWebRequest req)
    {
        string response = req.downloadHandler.text;
        // Check, if error..
        if (response.Contains(MESSAGE_ERROR_IDENTIFIER))
        {
            Debug.Log("Internal_OnCompletion_UpdateLocalProfileDataComplete(UnityWebRequest req)\n" + response);
        }
        else // If no error..
        {
            Debug.Log("Internal_OnCompletion_UpdateLocalProfileDataComplete(UnityWebRequest req)");
            string[] data = response.Split(MESSAGE_CUSTOM_DATA_SEPARATOR);
            if (data.Length >= 2)
            {
                pHandler.userProfile.credits = float.Parse(data[0]);
                pHandler.userProfile.socialScore = float.Parse(data[1]);

                GameObject profile = GameObject.FindGameObjectWithTag("Profile");
                if (profile != null)
                    {
                    profile.GetComponent<LevelManager>().UpdateLevels();
                    }
                else
                    {
                    Debug.Log("ERROR >> Profile not ready for level update");
                    }
            }
            else
            {
                Debug.Log("     ERROR >> invalid data length! Response from server was: " + response);
            }
        }

    }

    // INTERNAL TASKS STUFF ------------------------- INTERNAL TASKS STUFF ------------------------- INTERNAL TASKS STUFF
    class TempTaskList { public Task[] tasks; }
    void Internal_OnCompletion_UpdateAvailableTasksFromDatabase(UnityWebRequest req)
    {
        string json = "{\"tasks\": " + req.downloadHandler.text + "}";
        try
        {
            task_list = JsonUtility.FromJson<TempTaskList>(json).tasks.ToDictionary(t => t.taskID);
        }
        catch (Exception e)
        {
            task_list = new Dictionary<int, Task>(); // init to empty list if not found
            Debug.Log("No available tasks found!");
        }
        Debug.Log("Internal_OnCompletion_UpdateAvailableTasksFromDatabase(UnityWebRequest req)");
    }
    void Internal_OnCompletion_UpdateAcceptedTasksFromDatabase(UnityWebRequest req) // !!! NOT TESTED YET !!!
    {
        string json = "{\"tasks\": " + req.downloadHandler.text + "}";

        try
        {
            acceptedTasks_list = JsonUtility.FromJson<TempTaskList>(json).tasks.ToDictionary(t => t.taskID);
        }
        catch (Exception e)
        {
            acceptedTasks_list = new Dictionary<int, Task>(); // init to empty list if not found
            Debug.Log("No accepted tasks found!");
        }
        Debug.Log("Internal_OnCompletion_UpdateAcceptedTasksFromDatabase(UnityWebRequest req)");
    }
    void Internal_OnCompletion_UpdateCreatedTasksFromDatabase(UnityWebRequest req)
    {
        string json = "{\"tasks\": " + req.downloadHandler.text + "}";
        try
        {
            createdTasks_list = JsonUtility.FromJson<TempTaskList>(json).tasks.ToDictionary(t => t.taskID);
        }
        catch (Exception e)
        {
            createdTasks_list = new Dictionary<int, Task>(); // init to empty list if not found
            Debug.Log("No created tasks found!");
        }
        Debug.Log("Internal_OnCompletion_UpdateCreatedTasksFromDatabase(UnityWebRequest req)\n"+req.downloadHandler.text);
    }
    void Internal_OnCompletion_AddedNewTaskComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_AddedNewTaskComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }
    void Internal_OnCompletion_RemovedTaskComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_RemovedTaskComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }
    void Internal_OnCompletion_AcceptTaskComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_AcceptTaskComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }
    void Internal_OnCompletion_CompleteTaskComplete(UnityWebRequest req)
    {
        GameObject profile = GameObject.FindGameObjectWithTag("Profile");
        if (profile != null)
        {
            BeginRequest_UpdateLocalProfileData(null);
            profile.GetComponent<LevelManager>().UpdateLevels();
        }
        else
        {
            Debug.Log("ERROR >> Profile not ready for level update");
        }

        Debug.Log("Internal_OnCompletion_CompleteTaskComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }

    // INTERNAL ROOM BOOKING STUFF ------------------------- INTERNAL ROOM BOOKING STUFF ------------------------- INTERNAL ROOM BOOKING STUFF
    // *NOTE! copy this into the calendar system, so we can actually store the booking list somewhere..
    /*void Custom_OnCompletion_GetRoomBookings(string response)
    {
        List<TempBookingData> testList;

        string json = "{\"bookings\": " + response + "}";
        TempBookingsContainer bookingContainer = new TempBookingsContainer();
        try
        {
            bookingContainer = JsonUtility.FromJson<TempBookingsContainer>(json);
            testList = new List<TempBookingData>(bookingContainer.bookings);
        }
        catch (Exception e)
        {
            Debug.Log("No bookings found for requested room");
        }
    }*/

    void Internal_OnCompletion_MakeRoomBookingComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_MakeRoomBookingComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }
    void Internal_OnCompletion_CancelRoomBookingComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_CancelRoomBookingComplete(UnityWebRequest req)\n" + req.downloadHandler.text);
    }

    // INTERNAL REAL TIME STUFF ------------------------- INTERNAL REAL TIME STUFF ------------------------- INTERNAL REAL TIME STUFF
    void Internal_OnCompletion_GetProfileStatuses(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_GetProfileStatuses(UnityWebRequest req)");
    }
    void Internal_OnCompletion_UpdateProfileStatus(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_UpadteProfileStatus(UnityWebRequest req)");
    }

    // INTERNAL CHAT STUFF ------------------------- INTERNAL CHAT STUFF ------------------------- INTERNAL CHAT STUFF
    void Internal_OnCompletion_GetChatMessages(UnityWebRequest req)
    {
        //Debug.Log("Internal_OnCompletion_GetChatMessages(UnityWebRequest req)");
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