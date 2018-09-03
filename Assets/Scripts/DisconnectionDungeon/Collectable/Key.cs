using System;
using Graphene.Acting;
using UnityEngine;
using Graphene.Acting.Collectables;

namespace Graphene.DisconnectionDungeon.Collectable
{
    public class Key : TriggerObject, ICollectable
    {
        public void Collect(Actor player)
        {
            throw new NotImplementedException();
            Destroy(gameObject);
        }
    }
}