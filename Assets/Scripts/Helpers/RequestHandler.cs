using UnityEngine.Networking;
using System.Collections;

public class RequestHandler
{
    /// <summary>
    /// Simple request method with callback after the response has come.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IEnumerator SendRequest(string url, System.Action<UnityWebRequest> callback = null) {
        UnityWebRequest request = UnityWebRequest.Get(url);

        if (request.error != null) {
            callback(null);
            yield break;
        }
        
        yield return request.SendWebRequest();

        if (request.responseCode != 200L) {
            callback(null);
            yield break;
        }

        callback(request);
    }
}
