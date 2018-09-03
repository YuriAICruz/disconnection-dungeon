using Graphene.Acting;
using Graphene.Acting.Collectables;

namespace Graphene.DisconnectionDungeon.Collectable
{
    public class LevelCollectable : TriggerObject, ICollectable
    {
        public int Id;
        
        public void Collect(Actor player)
        {
            DDManager.Instance.Collect(Id);
            
            Destroy(gameObject);
        }
    }
}