using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequests
{
    public const string URL_test = "https://gcdata.000webhostapp.com/DatabaseTest.php";

    public static UnityWebRequest CreateWebRequest(string url)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Content-Type", "text/plain");
        req.SetRequestHeader("User-Agent", "DefaultBrowser");
        req.SetRequestHeader("Cookie", string.Format("DummyCookie"));
        return req;
    }
}
