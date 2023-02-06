using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using Cysharp.Threading.Tasks;

public class RequestHandler
{
    /// <summary>
    /// Simple request method with callback after the response has come
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

    /// <summary>
    /// Async method for byte[] data download task realization.
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public static async UniTask<byte[]> GetBytesAsync(UnityWebRequest req)
    {
        var response = await req.SendWebRequest();

        if (response.responseCode != 200L) {
            return null;
        }

        return response.downloadHandler.data;
    }
}
