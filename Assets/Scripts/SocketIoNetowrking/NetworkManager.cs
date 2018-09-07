using System;
using SocketIo;
using SocketIo.SocketTypes;
using UnityEngine;

namespace Graphene.Netowrking.Socketio
{
    public class NetworkManager : MonoBehaviour
    {
        public event Action OnConnect, OnDisconnect;
        
        private void Awake()
        {
            var socket = Io.Create("127.0.0.1", 5000, 5000, SocketHandlerType.Tcp);
            socket.Emit("connect");
            socket.On("connect", Connect);
            socket.On("disconnect", Disconnect);
        }

        private void Connect()
        {
            Debug.Log("Connected");
            OnConnect?.Invoke();
        }

        void Disconnect()
        {
            Debug.Log("Disconnected");
            OnDisconnect?.Invoke();
        }
    }
}