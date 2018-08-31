using Graphene.Acting;
using Graphene.DisconnectionDungeon.Collectable;

namespace Graphene.DisconnectionDungeon.Interactible
{
    public class Exit : TriggerObject, IInteractible
    {
        private DDManager _manager;
        private bool _once;

        void Start()
        {
            _manager = DDManager.Instance;
        }

        public void Interact()
        {
            if (_once) return;
            
            _once = true;
            _manager.EndLevel();
        }
    }
}