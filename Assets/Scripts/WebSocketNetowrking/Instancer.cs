using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Graphene.WebSocketsNetworking
{
    [Serializable]
    public class Instancer
    {
        private Guid _uid;
        public List<GameObject> Prefabs;
        
        private List<WebSocketsBehaviour> _behaviours = new List<WebSocketsBehaviour>(); 

        public void SetUid(Guid uid)
        {
            _uid = uid;
        }

        public void Instantiate(Message data)
        {
            if(data.uid == _uid.ToString())
                return;

            var index = JsonConvert.DeserializeObject<int>(data.message);
            
            Instantiate(index);
        }

        public void Instantiate(int index, bool local = false)
        {
            if(index >= Prefabs.Count) return;

            var obj = Object.Instantiate(Prefabs[index]);
            var bhv = obj.GetComponent<WebSocketsBehaviour>();
            if (bhv == null)
            {
                Debug.LogError("No WebSocketsBehaviour in object deleting");
                Object.Destroy(obj);
                return;
            }
            bhv.SetLocal(local);
            _behaviours.Add(bhv);
        }

        public uint GetBehavioursCount()
        {
            return (uint) _behaviours.Count;
        }
    }
}