using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Graphene.WebSocketsNetworking
{
    public class DispatchResponse<T>
    {
        public uint id;
        public Action<T> action;
        public uint objectId;
    }

    public class MessageDispatcher
    {
        private readonly Guid _uid;
        private Dictionary<uint, Action<Message>> _messagesToDispatch = new Dictionary<uint, Action<Message>>();

        private List<DispatchResponse<Vector3>> _dispatchVector3 = new List<DispatchResponse<Vector3>>();
        private List<DispatchResponse<Vector2>> _dispatchVector2 = new List<DispatchResponse<Vector2>>();
        private List<DispatchResponse<float>> _dispatchFloat = new List<DispatchResponse<float>>();
        private List<DispatchResponse<bool>> _dispatchBool = new List<DispatchResponse<bool>>();
        private List<DispatchResponse<int>> _dispatchInt = new List<DispatchResponse<int>>();

        public MessageDispatcher(Guid uid)
        {
            _uid = uid;
        }

        internal void Dispatch(Message msg)
        {
            if(!string.IsNullOrEmpty(msg.uid) && msg.uid == _uid.ToString()) return;
            
            if(_messagesToDispatch.ContainsKey(msg.id))
                _messagesToDispatch[msg.id].Invoke(msg);

            Dispatch(msg, _dispatchVector3);
            Dispatch(msg, _dispatchVector2);
            Dispatch(msg, _dispatchFloat);
            Dispatch(msg, _dispatchBool);
            Dispatch(msg, _dispatchInt);
        }

        void Dispatch<T>(Message msg, List<DispatchResponse<T>> list)
        {
            T res;
            try
            {
                res = JsonConvert.DeserializeObject<T>(msg.message);
            }
            catch (Exception e)
            {
                return;
            }
            
            foreach (var response in list)
            {
                if(response.objectId != msg.objectId) continue;
                
                response.action.Invoke(res);
            }
        }

        public void AddListener(uint id, Action<Message> response)
        {
            if (_messagesToDispatch.ContainsKey(id))
            {
                Debug.LogError("Already has listener");
                return;
            }

            _messagesToDispatch.Add(id, response);
        }

        public void RemoveListener(uint id)
        {
            if (!_messagesToDispatch.ContainsKey(id))
            {
                return;
            }

            _messagesToDispatch.Remove(id);
        }

        public void RemoveAll(uint objId)
        {
            _dispatchVector3.RemoveAll(x => x.objectId == objId);
            _dispatchVector2.RemoveAll(x => x.objectId == objId);
            _dispatchFloat.RemoveAll(x => x.objectId == objId);
            _dispatchBool.RemoveAll(x => x.objectId == objId);
            _dispatchInt.RemoveAll(x => x.objectId == objId);
        }

        public void AddListenerVector3(Action<Vector3> action, uint id)
        {
            _dispatchVector3.Add(new DispatchResponse<Vector3>()
            {
                id = (uint) NetworkManager.MessageId.Vector3,
                objectId = id,
                action = action
            });
        }

        public void AddListenerVector2(Action<Vector2> action, uint id)
        {
            _dispatchVector2.Add(new DispatchResponse<Vector2>()
            {
                id = (uint) NetworkManager.MessageId.Vector2,
                objectId = id,
                action = action
            });
        }

        public void AddListenerFloat(Action<float> action, uint id)
        {
            _dispatchFloat.Add(new DispatchResponse<float>()
            {
                id = (uint) NetworkManager.MessageId.Float,
                objectId = id,
                action = action
            });
        }

        public void AddListenerInt(Action<int> action, uint id)
        {
            _dispatchInt.Add(new DispatchResponse<int>()
            {
                id = (uint) NetworkManager.MessageId.Int,
                objectId = id,
                action = action
            });
        }

        public void AddListenerBool(Action<bool> action, uint id)
        {
            _dispatchBool.Add(new DispatchResponse<bool>()
            {
                id = (uint) NetworkManager.MessageId.Boolean,
                objectId = id,
                action = action
            });
        }
    }
}