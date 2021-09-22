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
