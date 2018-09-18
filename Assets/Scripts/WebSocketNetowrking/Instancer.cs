using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Graphene.WebSocketsNetworking
{
    public class ObjectData
    {
        public int index;
        public uint id;
        public Guid owner;
    }
    
    [Serializable]
    public class Instancer
    {
        private Guid _uid;
        public List<GameObject> Prefabs;
        
        private List<WebSocketsBehaviour> _behaviours = new List<WebSocketsBehaviour>();
        private int _behavioursCount = 0;

        public void SetUid(Guid uid)
        {
            _uid = uid;
        }

        public void BulkInstantiate(Message data)
        {
            var oData = JsonConvert.DeserializeObject<List<ObjectData>>(data.message);
            
            Debug.Log(oData.Count);

            foreach (var objectData in oData)
            {
                if(objectData.owner == _uid) continue;
                Instantiate(objectData);
            }
        }

        public void Instantiate(Message data)
        {
            if(data.uid == _uid.ToString())
                return;

            var oData = JsonConvert.DeserializeObject<ObjectData>(data.message);
            
            Instantiate(oData);
        }

        public void Instantiate(ObjectData oData, bool local = false)
        {
            if(oData.index >= Prefabs.Count) return;

            var obj = Object.Instantiate(Prefabs[oData.index]);
            var bhv = obj.GetComponent<WebSocketsBehaviour>();
            if (bhv == null)
            {
                Debug.LogError("No WebSocketsBehaviour in object deleting");
                Object.Destroy(obj);
                return;
            }
            
            bhv.SetLocal(local);
            bhv.SetId(oData.id);
            
            _behaviours.Add(bhv);
            _behavioursCount++;
        }
    }
}