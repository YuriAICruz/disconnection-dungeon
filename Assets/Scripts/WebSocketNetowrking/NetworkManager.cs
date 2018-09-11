using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Graphene.WebSocketsNetworking
{
    public class NetworkManager : MonoBehaviour
    {
        public enum MessageId
        {
            Default = 0,
            Handshake = 1,
        }
        
        [SerializeField] private string _url = "ws://127.0.0.1";
        private WebSocket _socket;
        private Guid _uid;

        void Start()
        {
            Connect();
        }

        private void Connect()
        {
            _socket = new WebSocket(new Uri(_url));
            StartCoroutine(ConnectToSocket());
        }

        IEnumerator ConnectToSocket()
        {
            yield return StartCoroutine(_socket.Connect());
            
            _uid = Guid.NewGuid();
            Send((uint)MessageId.Handshake, "NoName");
            
            StartCoroutine(Listen());
        }

        public void Send(uint id, string message)
        {
            var msg = new Message(id, message, _uid);
            StartCoroutine(SendToSocket(JsonConvert.SerializeObject(msg)));
        }

        public void Send(string data)
        {
            Send((uint)MessageId.Default, data);
        }

        IEnumerator SendToSocket(string data)
        {
            // yield return StartCoroutine(_socket.Connect());
            
            _socket.SendString(data);
            
            yield return 0;
        }

        IEnumerator Listen()
        {
            while (true)
            {
                var reply = _socket.RecvString();
                if (reply != null)
                {
                    Debug.Log("Received: " + reply);
                }
                if (_socket.error != null)
                {
                    Debug.LogError("Error: " + _socket.error);
                    break;
                }
                yield return new WaitForChangedResult();
            }
        }
    }
}