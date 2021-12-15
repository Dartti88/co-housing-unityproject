using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequests
{
    public const string URL_BASE = "GamifiedCohousing-32854.portmap.host:32854/gcSite/";

    public const string URL_GET_Profiles =                  URL_BASE + "GetProfiles.php";
    public const string URL_POST_CreateNewProfile =         URL_BASE + "AddNewProfileTEST.php";
    public const string URL_POST_ValidatePassword =         URL_BASE + "ValidatePassword.php";
    public const string URL_POST_LogOut =                   URL_BASE + "LogOut.php";
    public const string URL_POST_UpdateProfile =            URL_BASE + "UpdateProfile.php";
    public const string URL_POST_UpdateLocalProfileData =   URL_BASE + "GetProfileData.php";

    public const string URL_POST_CreateNewTask =       URL_BASE + "AddNewTask.php";
    public const string URL_POST_RemoveTask =          URL_BASE + "RemoveTask.php";
    public const string URL_GET_AvailableTasks =       URL_BASE + "GetAvailableTasks.php";
    public const string URL_POST_GetAcceptedTasks =    URL_BASE + "GetAcceptedTasks.php";
    public const string URL_POST_GetCreatedTasks =     URL_BASE + "GetCreatedTasks.php";

    public const string URL_POST_AcceptTask =           URL_BASE + "AcceptTask.php";
    public const string URL_POST_CompleteTask =         URL_BASE + "CompleteTask.php";
    
    public const string URL_POST_GetProfileStatuses =   URL_BASE + "GetProfileStatuses.php";
    public const string URL_POST_UpdateProfileStatus =  URL_BASE + "UpdateProfileStatus.php";

    public const string URL_GET_GetChatMessages =       URL_BASE + "GetChatMessages.php";
    public const string URL_POST_SubmitChatMessage =    URL_BASE + "SubmitChatMessage.php";

    public const string URL_POST_GetRoomBookings =      URL_BASE + "GetRoomBookings.php";
    public const string URL_POST_MakeRoomBooking =      URL_BASE + "MakeRoomBooking.php";
    public const string URL_POST_CancelRoomBooking =    URL_BASE + "CancelRoomBooking.php";


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
