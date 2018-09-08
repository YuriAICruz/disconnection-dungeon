using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Graphene.DisconnectionDungeon
{
    public class PlayerNetworkWrapper : NetworkBehaviour, IActorController
    {
        public event Action<Vector3> SetPosition;

        private Vector3 _lastPos;
        
        [Command]
        internal void CmdUpdatePosition(Vector3 position)
        {
            RpcUpdatePosition(position);
        }

        [ClientRpc]
        private void RpcUpdatePosition(Vector3 position)
        {
            if (isLocalPlayer) return;
            
            _lastPos = position;
            transform.position = _lastPos;
        }

        void Update()
        {
            CmdUpdatePosition(transform.position);
        }
    }
}