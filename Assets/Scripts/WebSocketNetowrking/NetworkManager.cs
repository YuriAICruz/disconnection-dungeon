using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

namespace Graphene.WebSocketsNetworking
{
    public class NetworkManager : MonoBehaviour
    {
        public enum MessageId
        {
            Default = 0,
            Connect = 1,
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
        private WebSocket _socket;
        private Guid _uid;
        private Thread _listenThread;

        Queue<Action> _mainThreadStack = new Queue<Action>();

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
            _listenThread.Abort();
        }

        IEnumerator ConnectToSocket()
        {
            yield return StartCoroutine(_socket.Connect());

            _uid = Guid.NewGuid();
            Send((uint) MessageId.Connect, "NoName");

            Dispatcher = new MessageDispatcher(_uid);
            Instancer.SetUid(_uid);

            Dispatcher.AddListener((uint) MessageId.Instantiate, Instancer.Instantiate);

            CreatePlayer();

            _listenThread = new Thread(Listen);
            _listenThread.Start();
            StartCoroutine(MainThreadDispatcher());
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
            Send((uint) MessageId.Instantiate, JsonConvert.SerializeObject(0));
            Instancer.Instantiate(0, true);
        }    

        IEnumerator MainThreadDispatcher()
        {
            while (true)
            {
                if (_mainThreadStack.Count > 0)
                    _mainThreadStack.Dequeue()();

                yield return new WaitForChangedResult();
            }
        }

        void Listen()
        {
            while (true)
            {
                var reply = _socket.RecvString();
                if (reply != null)
                {
                    try
                    {
                        var msg = JsonConvert.DeserializeObject<Message>(reply);
                        _mainThreadStack.Enqueue(() => Dispatcher.Dispatch(msg));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(reply + "\n\n" + e);
                        throw;
                    }
                }
                if (_socket.error != null)
                {
                    Debug.LogError("Error: " + _socket.error);
                    break;
                }
            }

            Close();
        }

        void Close()
        {
            _socket.Close();
        }

        public uint GetBehaviourId()
        {
            return Instancer.GetBehavioursCount()+1;
        }
    }
}