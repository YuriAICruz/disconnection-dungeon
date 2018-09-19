using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Graphene.WebSocketsNetworking
{
    public class NetworkManager : MonoBehaviour
    {
        public enum MessageId
        {
            Default = 0,
            Connect = 1,
            BulkRemoveInstances = 47,
            BulkInstantiate = 48,
            Instantiate = 49,
            Vector3 = 50,
            Vector2 = 51,
            Float = 52,
            Int = 53,
            Boolean = 54,
            Action = 55,
        }

        public MessageDispatcher Dispatcher;
        public Instancer Instancer;

        [SerializeField] private string _url = "ws://127.0.0.1";
        [SerializeField] private string _apiUrl = "http://127.0.0.1";
        private WebSocket _socket;
        private Guid _uid;

        Queue<Action> _mainThreadStack = new Queue<Action>();
        private HttpCom _http;

        void Start()
        {
            Connect();
        }

        private void Connect()
        {
            _socket = new WebSocket(new Uri(_url));
            StartCoroutine(ConnectToSocket());
        }

        private void OnDestroy()
        {
        }

        IEnumerator ConnectToSocket()
        {
            yield return StartCoroutine(_socket.Connect());

            _uid = Guid.NewGuid();
            Send((uint) MessageId.Connect, "NoName");

            Dispatcher = new MessageDispatcher(_uid);
            Instancer.SetUid(_uid);

            _http = new HttpCom(_apiUrl);

            Dispatcher.AddListener((uint) MessageId.Instantiate, Instancer.Instantiate);
            Dispatcher.AddListener((uint) MessageId.BulkInstantiate, Instancer.BulkInstantiate);
            Dispatcher.AddListener((uint) MessageId.BulkRemoveInstances, Instancer.BulkRemoveInstances);

            CreatePlayer();

            StartCoroutine(MainThreadDispatcher());
            yield return StartCoroutine(Listen());
        }

        public void Send(uint id, string message, uint oId = 0)
        {
            var msg = new Message(id, message, _uid, oId);
            StartCoroutine(SendToSocket(JsonConvert.SerializeObject(msg)));
        }

        public void Send(string data)
        {
            Send((uint) MessageId.Default, data);
        }

        IEnumerator SendToSocket(string data)
        {
            // yield return StartCoroutine(_socket.Connect());

            _socket.SendString(data);

            yield return 0;
        }

        public void CreatePlayer()
        {
            StartCoroutine(_http.Get<int>("nextId", (id) =>
            {
                var objd = new ObjectData() {index = 0, id = (uint) id};

                Send((uint) MessageId.Instantiate, JsonConvert.SerializeObject(objd));
                Instancer.Instantiate(objd, true);
            }));
        }

        [Obsolete] // WebGl doesnot suport
        public async Task<T> Get<T>(string url)
        {
            Debug.Log("url: " + url);
            var uri = new Uri(url);
            var content = new MemoryStream();
            var webReq = (HttpWebRequest) WebRequest.Create(url);
            webReq.Method = "GET";
            var json = "";

            using (WebResponse response = await webReq.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    await responseStream.CopyToAsync(content);

                    content.Position = 0;

                    using (StreamReader reader = new StreamReader(content))
                    {
                        json = await reader.ReadToEndAsync();

                        try
                        {
                            return JsonConvert.DeserializeObject<T>(json);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            return default(T);
                        }
                    }
                }
            }
        }

        IEnumerator MainThreadDispatcher()
        {
            while (true)
            {
                if (_mainThreadStack.Count > 0)
                    for (int i = _mainThreadStack.Count - 1; i >= 0; i--)
                        _mainThreadStack.Dequeue()();

                yield return null;
            }
        }

        IEnumerator Listen()
        {
            while (true)
            {
                var reply = _socket.RecvString();
                
                if (reply != null)
                {
                    // Debug.Log(reply);
                    try
                    {
                        var msg = JsonConvert.DeserializeObject<Message>(reply);
                        _mainThreadStack.Enqueue(() => Dispatcher.Dispatch(msg));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(reply + "\n\n" + e);
                    }
                }
                if (_socket.error != null)
                {
                    Debug.LogError("Error: " + _socket.error);
                    break;
                }

                yield return null;
            }

            Close();
        }

        void Close()
        {
            _socket.Close();
        }
    }
}