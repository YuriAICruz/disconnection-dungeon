using System;
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
            Send((uint) NetworkManager.MessageId.Vector3, position);
        }

        private void UpdatePosition(Vector3 position)
        {
            if (isLocalPlayer) return;

            _lastPos = position;
            transform.position = _lastPos;
        }

        void Update()
        {
            SendPositionUpdated(transform.position);
        }
    }
}