using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequests
{
    
    public const string URL_GET_Profiles = "https://gamifiedcohousingapp.000webhostapp.com/GetProfiles.php";
    public const string URL_POST_CreateNewProfile = "https://gamifiedcohousingapp.000webhostapp.com/AddNewProfileTEST.php";
    public const string URL_POST_ValidatePassword = "https://gamifiedcohousingapp.000webhostapp.com/ValidatePassword.php";
    public const string URL_POST_LogOut = "https://gamifiedcohousingapp.000webhostapp.com/LogOut.php";
    public const string URL_POST_UpdateProfile = "https://gamifiedcohousingapp.000webhostapp.com/UpdateProfile.php";
    public const string URL_POST_UpdateLocalProfileData = "https://gamifiedcohousingapp.000webhostapp.com/GetProfileData.php";

    public const string URL_POST_CreateNewTask = "https://gamifiedcohousingapp.000webhostapp.com/AddNewTask.php";
    public const string URL_POST_RemoveTask = "https://gamifiedcohousingapp.000webhostapp.com/RemoveTask.php";
    public const string URL_GET_AvailableTasks = "https://gamifiedcohousingapp.000webhostapp.com/GetAvailableTasks.php";
    public const string URL_POST_GetAcceptedTasks = "https://gamifiedcohousingapp.000webhostapp.com/GetAcceptedTasks.php";
    public const string URL_POST_GetCreatedTasks= "https://gamifiedcohousingapp.000webhostapp.com/GetCreatedTasks.php";

    public const string URL_POST_AcceptTask = "https://gamifiedcohousingapp.000webhostapp.com/AcceptTask.php";
    public const string URL_POST_CompleteTask = "https://gamifiedcohousingapp.000webhostapp.com/CompleteTask.php";
    
    public const string URL_POST_GetProfileStatuses = "https://gamifiedcohousingapp.000webhostapp.com/GetProfileStatuses.php";
    public const string URL_POST_UpdateProfileStatus = "https://gamifiedcohousingapp.000webhostapp.com/UpdateProfileStatus.php";

    public const string URL_GET_GetChatMessages = "https://gamifiedcohousingapp.000webhostapp.com/GetChatMessages.php";
    public const string URL_POST_SubmitChatMessage = "https://gamifiedcohousingapp.000webhostapp.com/SubmitChatMessage.php";

    public const string URL_POST_GetRoomBookings = "https://gamifiedcohousingapp.000webhostapp.com/GetRoomBookings.php";
    public const string URL_POST_MakeRoomBooking = "https://gamifiedcohousingapp.000webhostapp.com/MakeRoomBooking.php";
    public const string URL_POST_CancelRoomBooking = "https://gamifiedcohousingapp.000webhostapp.com/CancelRoomBooking.php";


    public static UnityWebRequest CreateWebRequest_GET(string url, string contentType = "text/plain")
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Content-Type", contentType);
        req.SetRequestHeader("User-Agent", "DefaultBrowser");
        req.SetRequestHeader("Cookie", string.Format("DummyCookie"));
        return req;
    }

    public static UnityWebRequest CreateWebRequest_POST_FORM(string url, List<IMultipartFormSection> form, string contentType = "text/plain")
    {
        UnityWebRequest req = UnityWebRequest.Post(url, form);
        /*req.SetRequestHeader("Content-Type", "multipart/form-data");
        req.SetRequestHeader("User-Agent", "DefaultBrowser");
        req.SetRequestHeader("Cookie", string.Format("DummyCookie"));*/
        return req;
    }
}
