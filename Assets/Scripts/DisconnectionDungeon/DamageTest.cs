using Graphene.Acting;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class DamageTest : MonoBehaviour, IDamageble
    {
        public void DoDamage(int damage)
        {
            Debug.Log(damage);
        }
    }
}