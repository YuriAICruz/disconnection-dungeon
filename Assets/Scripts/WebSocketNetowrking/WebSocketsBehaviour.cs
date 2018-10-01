using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Graphene.WebSocketsNetworking
{
    public class WebSocketsBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public uint Id;

        public int SpawnId;
        
        public bool isServer { get; private set; }
        public bool isClient { get; private set; }
        public bool isLocalPlayer { get; private set; }

        private NetworkManager _manager;
        private MessageDispatcher _dispacher;

        void Start()
        {
            _manager = FindObjectOfType<NetworkManager>();
            _dispacher = _manager.Dispatcher;
            
            OnStart();
        }

        protected virtual void OnStart()
        {
            
        }

        protected void AddListenerVector3(Action<Vector3> updatePosition)
        {
            _dispacher.AddListenerVector3(updatePosition, Id);
        }
        protected void AddListenerVector2(Action<Vector2> updatePosition)
        {
            _dispacher.AddListenerVector2(updatePosition, Id);
        }
        protected void AddListenerFloat(Action<float> updatePosition)
        {
            _dispacher.AddListenerFloat(updatePosition, Id);
        }
        protected void AddListenerInt(Action<int> updatePosition)
        {
            _dispacher.AddListenerInt(updatePosition, Id);
        }
        protected void AddListenerBool(Action<bool> updatePosition)
        {
            _dispacher.AddListenerBool(updatePosition, Id);
        }

        protected void Send(uint id, object obj)
        {
            _manager.Send(id, JsonConvert.SerializeObject(obj), Id);
        }

        public void SetLocal(bool local)
        {
            isLocalPlayer = local;
            isClient = true;
            isServer = false;
        }

        public void SetId(uint oId)
        {
            Id = oId;
        }
    }
}