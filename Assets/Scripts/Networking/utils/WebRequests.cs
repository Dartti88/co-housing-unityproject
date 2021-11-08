using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequests
{
    public const string URL_test = "https://gcdata.000webhostapp.com/DatabaseTest.php";
    
    public const string URL_GET_Profiles = "https://gcdata.000webhostapp.com/GetProfiles.php";
    public const string URL_POST_CreateNewProfile = "https://gcdata.000webhostapp.com/AddNewProfileTEST.php";
    public const string URL_POST_ValidatePassword = "https://gcdata.000webhostapp.com/ValidatePassword.php";
    public const string URL_POST_UpdateProfile = "https://gcdata.000webhostapp.com/UpdateProfile.php";

    public const string URL_POST_CreateNewTask = "https://gcdata.000webhostapp.com/AddNewTask.php";
    public const string URL_POST_RemoveTask = "https://gcdata.000webhostapp.com/RemoveTask.php";
    public const string URL_GET_AvailableTasks = "https://gcdata.000webhostapp.com/GetAvailableTasks.php";
    public const string URL_POST_GetAcceptedTasks = "https://gcdata.000webhostapp.com/GetAcceptedTasks.php";
    public const string URL_POST_GetCreatedTasks= "https://gcdata.000webhostapp.com/GetCreatedTasks.php";

    public const string URL_POST_AcceptTask = "https://gcdata.000webhostapp.com/AcceptTask.php";
    public const string URL_POST_CompleteTask = "https://gcdata.000webhostapp.com/CompleteTask.php";

    public const string URL_GET_GetCharacterDestinations = "https://gcdata.000webhostapp.com/GetCharacterDestinations.php";
    public const string URL_POST_SendCharacterDestination = "https://gcdata.000webhostapp.com/SendCharacterDestination.php";



    public static UnityWebRequest CreateWebRequest_GET(string url, string contentType = "text/plain")
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Content-Type", contentType);
        req.SetRequestHeader("User-Agent", "DefaultBrowser");
        req.SetRequestHeader("Cookie", string.Format("DummyCookie"));
        return req;
    }

    public static UnityWebRequest CreateWebRequest_POST_FORM(string url, List<IMultipartFormSection> form)
    {
        UnityWebRequest req = UnityWebRequest.Post(url, form);
        /*req.SetRequestHeader("Content-Type", "multipart/form-data");
        req.SetRequestHeader("User-Agent", "DefaultBrowser");
        req.SetRequestHeader("Cookie", string.Format("DummyCookie"));*/
        return req;
    }
}
