using Graphene.Acting;
using Graphene.Acting.Interfaces;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class DamageTest : MonoBehaviour, IDamageble
    {
        public Life life;

        private void Awake()
        {
            life.Reset();
            life.OnDie += Destroy;
        }

        private void Destroy()
        {
            Destroy(gameObject);
        }

        public void DoDamage(int damage, Vector3 from)
        {
            life.ReceiveDamage(damage);
            Debug.Log(damage);
        }
    }
}