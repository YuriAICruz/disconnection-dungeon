using Graphene.Acting;
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

        public void DoDamage(int damage)
        {
            life.ReceiveDamage(damage);
            Debug.Log(damage);
        }
    }
}