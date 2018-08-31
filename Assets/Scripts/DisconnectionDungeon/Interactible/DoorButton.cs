using Graphene.Acting;
using Graphene.DisconnectionDungeon.Collectable;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.Interactible
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