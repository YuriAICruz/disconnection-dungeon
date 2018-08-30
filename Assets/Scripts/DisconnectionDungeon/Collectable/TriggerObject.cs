using UnityEngine;

namespace DisconnectionDungeon.Collectable
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TriggerObject : MonoBehaviour
    {
        private BoxCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _collider.isTrigger = true;
        }
        
    }
}