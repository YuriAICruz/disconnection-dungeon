using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public abstract class BasicPhysics
    {
        public Collider Collider;
        public Rigidbody Rigidbody;
        protected bool _debug = true;
        protected bool _grounded;
        
        protected void CheckGround()
        {
            RaycastHit hit;

            if (UnityEngine.Physics.Raycast(Collider.transform.position, Vector3.down, out hit, 0.1f))
            {
                if (_debug)
                    Debug.DrawRay(Collider.transform.position, Vector3.down * 1.1f, Color.green);
                
                _grounded = true;
                return;
            }

            _grounded = false;
        }

        public virtual void SetCollider(Collider collider, Rigidbody rigidbody)
        {
            Collider = collider;
            Rigidbody = rigidbody;
        }
    }
}