using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Linq;


/*

*Kaikki n‰m‰ pyynnˆt ei mene aina ensimm‰isell‰ kerralla l‰pi, koska tuo serveri on paska:D
V‰lill‰ sielt‰ tulee onCompletionCallback:iin jotakin, miss‰ lukee esim. "access violation 
or syntax error" ja yleens‰ jossakin kohtaa error koodi 1064, tai joku "server timed out".
N‰iss‰ tilanteissa mik‰‰n ei yleens‰ ole rikki, vaan tuo pyyntˆ pit‰‰ vain laittaa menem‰‰n uudestaan.

Taskeihin liittyv‰t:
    BeginRequest_CompleteTask(profileID, taskID, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Merkkaa serverille, ett‰ t‰m‰ profiili on suorittanut taskin
        *Ei kuitenkaan koske mihink‰‰n paikallisiin listoihin / systeemeihin
        (*Jos halutaan p‰ivitt‰‰ paikallisia listoja ottamaan t‰m‰n muutoksen huomioon, 
        pit‰‰ kutsua GetAcceptedTasks, tai GetAvailableTasks, riippuen, mit‰ halutaan tehd‰)
        
    BeginRequest_AcceptTask(profileID, taskID, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Merkkaa serverille, ett‰ profiili on hyv‰ksynyt taskin
        *Ei kuitenkaan koske mihink‰‰n paikallisiin listoihin / systeemeihin
        (*Jos halutaan p‰ivitt‰‰ paikallisia listoja ottamaan t‰m‰n muutoksen huomioon, 
        pit‰‰ kutsua GetAcceptedTasks, tai GetAvailableTasks, riippuen, mit‰ halutaan tehd‰)
    
    BeginRequest_GetAvailableTasks(onCompletionCallback);
        *P‰ivitt‰‰ Client.Instance.task_list - dictionaryn

    BeginRequest_GetAcceptedTasks(profileID, onCompletionCallback);
        *P‰ivitt‰‰ Client.Instance.acceptedTasks_list - dictionaryn
    
    BeginRequest_GetCreatedTasks(profileID, onCompletionCallback);
        *Hakee kaikki t‰m‰n profiilin tekem‰t taskit
        *P‰ivitt‰‰ Client.Instance.createdTasks_list - dictionaryn

    BeginRequest_AddNewTask(Task, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Lis‰‰ uuden taskin serverille
        *T‰m‰k‰‰n ei koske mihink‰‰n paikallisiin listoihin / systeemeihin
        (*Jos halutaan p‰ivitt‰‰ paikallisia listoja ottamaan t‰m‰n muutoksen huomioon, 
        pit‰‰ kutsua GetAcceptedTasks, tai GetAvailableTasks, riippuen, mit‰ halutaan tehd‰)
        
    BeginRequest_RemoveTask(userName, password, taskID, onCompletionCallback)
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Poistaa taskin serverilt‰
        (*T‰m‰k‰‰n ei koske mihink‰‰n paikallisiin listoihin)

Profiileihin liittyv‰t:
    BeginRequest_GetAllProfiles(onCompletionCallback);
        *P‰ivitt‰‰ Client.Instance.profile_list.profiles
        *HUOM!: tuo Client.Instance.profile_list on ProfilesContainer - objekti, joka pit‰‰ sis‰ll‰‰n taulukon profiileista
        
    BeginRequest_AddNewProfile(Profile);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Lis‰‰ uuden profiilin serverille
        (*Ei koske mihink‰‰n paikalliseen dataan. Pit‰‰ kutsua GetAllProfiles, jos halutaan, ett‰ muutos n‰kyy t‰m‰n j‰lkeen paikallisesti)

    BeginRequest_UpdateProfile(Profile, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *P‰ivitt‰‰ profiilin tietoja serverille.
        *T‰ll‰ sis‰‰n syˆtett‰v‰ll‰ profiililla on oltava oikea userName ja password, ett‰ t‰m‰ muutos hyv‰ksyt‰‰n serverill‰
        (*Ei koske mihink‰‰n paikalliseen dataan)

    BeginRequest_ValidatePassword(userName, password, onCompletionCallback);
        *Palauttaa "Success" - onCompletionCallback:iin, jos onnistuu
        *Testaa lˆytyykˆ serverilt‰ userName - password matchi‰
        (*Ei koske mihink‰‰n paikalliseen dataan)
*/

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }
    
    [Serializable]
    public class ProfilesContainer
    {
        public Profile[] profiles;
    }

    public ProfilesContainer profile_list = new ProfilesContainer();

    public Dictionary<int, Task> task_list =            new Dictionary<int, Task>();
    public Dictionary<int, Task> acceptedTasks_list =   new Dictionary<int, Task>();
    public Dictionary<int, Task> createdTasks_list =    new Dictionary<int, Task>();

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
    Task newTestTask2;
    Task newTestTask3;

    // Start is called before the first frame update
    void Start()
    {
        /*
        testProfile = new Profile(0, 0, "UnityTestUser", "UrpoPetteri", "1234", "Test user", 1, 0, 0, Profile.ProfileType.Resident, DateTime.Now);
        
        newTestTask = new Task();
        newTestTask.creatorID = 22;
        newTestTask.taskName = "Another Task";
        newTestTask.targetID = 2;
        newTestTask.description = "Testing does task names and points work..";
        newTestTask.cost = 1;
        newTestTask.points = 420;
        newTestTask.quantity = 5;
        newTestTask.uniqueQuantity = 5;
        newTestTask.expirationDate = "2025-09-12";

        newTestTask2 = new Task();
        newTestTask2.creatorID = 22;
        newTestTask2.taskName = "Testingtask2";
        newTestTask2.targetID = 2;
        newTestTask2.description = "Testing GetCreatedTasks.php";
        newTestTask2.cost = 1;
        newTestTask2.points = 1;
        newTestTask2.quantity = 2;
        newTestTask2.uniqueQuantity = 0;
        newTestTask2.expirationDate = "2025-09-12";
        

        newTestTask3 = new Task();
        newTestTask3.creatorID = 23;
        newTestTask3.taskName = "TESTINGTESTINGASD123";
        newTestTask3.targetID = 0;
        newTestTask3.description = "tesging";
        newTestTask3.cost = 1;
        newTestTask3.points = 1;
        newTestTask3.quantity = 1;
        newTestTask3.uniqueQuantity = 0;
        newTestTask3.expirationDate = "2025-09-12";
        */
    }

    bool exec = true;
    // Update is called once per frame
    void Update()
    {
        if (exec)
        {

            //BeginRequest_AddNewTask(newTestTask3, null);
            //BeginRequest_RemoveTask("UnityTestUser", "1234", 11, null);

            //BeginRequest_GetCreatedTasks(22, null);

            //BeginRequest_GetAvailableTasks(null);

            exec = false;
        }
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
        form.Add(new MultipartFormDataSection("key_profileID", "\"" + profileToUpdate.profileID.ToString() + "\""));
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
        task_list = JsonUtility.FromJson<TempTaskList>(json).tasks.ToDictionary(t => t.taskID);
        Debug.Log("Internal_OnCompletion_UpdateAvailableTasksFromDatabase(UnityWebRequest req)");
    }
    void Internal_OnCompletion_UpdateAcceptedTasksFromDatabase(UnityWebRequest req) // !!! NOT TESTED YET !!!
    {
        string json = "{\"tasks\": " + req.downloadHandler.text + "}";
        acceptedTasks_list = JsonUtility.FromJson<TempTaskList>(json).tasks.ToDictionary(t => t.taskID);
        Debug.Log("Internal_OnCompletion_UpdateAcceptedTasksFromDatabase(UnityWebRequest req)");
    }
    void Internal_OnCompletion_UpdateCreatedTasksFromDatabase(UnityWebRequest req)
    {
        string json = "{\"tasks\": " + req.downloadHandler.text + "}";
        createdTasks_list = JsonUtility.FromJson<TempTaskList>(json).tasks.ToDictionary(t => t.taskID);
        Debug.Log("Internal_OnCompletion_UpdateCreatedTasksFromDatabase(UnityWebRequest req)\n"+req.downloadHandler.text);

        Debug.Log("\nCreated tasks list:");
        foreach (KeyValuePair<int, Task> t in createdTasks_list)
        {
            Debug.Log("taskID : " + t.Key.ToString() + " Creator : " + t.Value.creatorID.ToString());
        }
    }
    void Internal_OnCompletion_AddedNewTaskComplete(UnityWebRequest req)
    {
        Debug.Log("Internal_OnCompletion_AddedNewTaskComplete(UnityWebRequest req)");
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