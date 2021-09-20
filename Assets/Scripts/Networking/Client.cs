using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;


public abstract class DatabaseEntry
{
    public bool RequestInProgress { get; protected set; } = false;
    public bool IsAvailable { get; protected set; } = false;
    public abstract void Init();
    public abstract void SetString(int index, string data);
}

public class Profile : DatabaseEntry
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

    Profile profile;

    // Start is called before the first frame update
    void Start()
    {
        profile = new Profile();
    }


    bool exec = true;
    // Update is called once per frame
    void Update()
    {
        if (exec)
        {
            BeginRequest_GetProfile(profile);
            exec = false;
        }

        if (profile.RequestInProgress)
            Debug.Log("Request in progress...");

        if (profile.IsAvailable)
            Debug.Log("Server >> " + profile.GetUsername());
    }


    public void BeginRequest_GetProfile(Profile profile)
    {
        if (!profile.RequestInProgress)
            StartCoroutine(GetFromDatabase(0, profile));
    }

    IEnumerator GetFromDatabase(int index, DatabaseEntry outEntry)
    {
        outEntry.Init();

        UnityWebRequest testRequest = WebRequests.CreateWebRequest(WebRequests.URL_test);
        Debug.Log("Sending GET request to the server...");

        yield return testRequest.SendWebRequest();

        if (testRequest.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(testRequest.error);
        else
        {
            //str = testRequest.downloadHandler.text;
            outEntry.SetString(index, testRequest.downloadHandler.text);

            //Debug.Log("Data fetched successfully: <" + www.downloadHandler.text + ">");
        }
    }


    /*
    IEnumerator GetFromDatabase()
    {
        UnityWebRequest testRequest = WebRequests.CreateWebRequest(WebRequests.URL_test);
        Debug.Log("Sending GET request to the server...");

        yield return testRequest.SendWebRequest();

        if (testRequest.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(testRequest.error);
        else
        {
            stringFromDatabase = testRequest.downloadHandler.text;
            //Debug.Log("Data fetched successfully: <" + www.downloadHandler.text + ">");
        }
    }
    */

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
