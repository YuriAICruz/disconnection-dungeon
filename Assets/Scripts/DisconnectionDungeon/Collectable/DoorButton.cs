using Graphene.Acting;
using UnityEngine;

namespace DisconnectionDungeon.Collectable
{
    public class DoorButton : TriggerObject, IInteractible
    {
        public Door door;

        [SerializeField] private bool _isOpen;

        void Start()
        {
            Interact();
        }

        public void Interact()
        {
            _isOpen = !_isOpen;
            
            if (_isOpen)
                door.Close();
            else
                door.Open();
        }
    }
}