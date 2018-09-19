using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine;

namespace Graphene.WebSocketsNetworking
{
    public class HttpCom
    {
        private readonly string _apiUrl;

        public HttpCom(string apiUrl)
        {
            _apiUrl = apiUrl+"/";
        }
        
        public IEnumerator Get<T>(string path, Action<T> response)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(_apiUrl + path))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError(www.error);
                    response(default(T));
                    yield break;
                }
                else
                {
                    var json = www.downloadHandler.text;
                    
                    try
                    {
                        response(JsonConvert.DeserializeObject<T>(json));
                        yield break;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        response(default(T));
                        yield break;
                    }
                }
            }
        }
    }
}