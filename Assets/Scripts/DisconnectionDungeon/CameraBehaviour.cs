using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class CameraBehaviour : MonoBehaviour
    {
        private DDManager _manager;

        private void Awake()
        {
            _manager = DDManager.Instance;
        }
    }
}