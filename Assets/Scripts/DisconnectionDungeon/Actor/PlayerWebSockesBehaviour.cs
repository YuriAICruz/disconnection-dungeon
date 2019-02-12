using System;
using Graphene.Acting.ActorController;
using Graphene.Acting.Platformer;
using Graphene.WebSocketsNetworking;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class PlayerWebSockesBehaviour : WebSocketsBehaviour, IActorController
    {
        public event Action<Vector3> SetPosition;

        private Vector3 _lastPos;

        protected override void OnStart()
        {
            base.OnStart();
            
            AddListenerVector3(UpdatePosition);
        }

        internal void SendPositionUpdated(Vector3 position)
        {
            _lastPos = transform.position;
            Send((uint) NetworkManager.MessageId.Vector3, position);
        }

        private void UpdatePosition(Vector3 position)
        {
            if (isLocalPlayer) return;

            Debug.Log("Update");
            _lastPos = position;
            transform.position = _lastPos;
        }

        void Update()
        {
            if((_lastPos - transform.position).magnitude < 0.1f ) return;
            
            SendPositionUpdated(transform.position);
        }
    }
}