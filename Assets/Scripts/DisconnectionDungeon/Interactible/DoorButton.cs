using System.Collections.Generic;
using Graphene.Acting;
using Graphene.DisconnectionDungeon.Collectable;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.Interactible
{
    public class DoorButton : TriggerObject, IInteractible
    {
        public Door door;

        public bool IsOpen;

        public List<DoorButton> CoButtons;

        void Start()
        {
            if (!CoButtons.Contains(this))
                CoButtons.Add(this);
            
            IsOpen = !IsOpen;
            Interact();
        }

        public void Interact()
        {
            IsOpen = !IsOpen;

            if (CoButtons.FindAll(x => x.IsOpen).Count == CoButtons.Count)
            {
                door.Open();
            }
            else
            {
                door.Close();
            }
        }
    }
}