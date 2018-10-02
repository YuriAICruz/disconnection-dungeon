using System;
using System.Collections.Generic;
using System.Linq;
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

        public void BulkRemoveInstances(Message data)
        {
            var oData = JsonConvert.DeserializeObject<List<ObjectData>>(data.message);

            foreach (var objectData in oData)
            {
                Destroy(objectData);
            }
        }

        public void BulkInstantiate(Message data)
        {
            var oData = JsonConvert.DeserializeObject<List<ObjectData>>(data.message);

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

        private void Destroy(ObjectData oData)
        {
            _behaviours.RemoveAll(x => x.Id == oData.id);
        }

        public void Instantiate(ObjectData oData, bool local = false)
        {
            if(oData.index >= Prefabs.Count) return;

            var spawnPoints = Object.FindObjectsOfType<SpawnPoint>().ToList();
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

            var point = spawnPoints.Find(x => x.SpawnId == bhv.SpawnId);

            if (point != null)
            {
                bhv.transform.position = point.transform.position;
            }
            
            _behaviours.Add(bhv);
            _behavioursCount++;
        }
    }
}