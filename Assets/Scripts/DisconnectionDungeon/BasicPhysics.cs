using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public abstract class BasicPhysics
    {
        public Collider Collider;
        public Rigidbody Rigidbody;
        protected bool _debug = true;
        protected bool _grounded;
        protected bool _jumping;

        protected Collider _standingCollider;
        
        Vector3[] _sides = new Vector3[]
        {
            new Vector3(0,0,0), 
            new Vector3(1,0,0), 
            new Vector3(0,0,1), 
            new Vector3(-1,0,0), 
            new Vector3(0,0,-1), 
        };

        protected void CheckGround()
        {
            RaycastHit hit;

            for (int i = 0; i < _sides.Length; i++)
            {
                var pos = Collider.transform.position + _sides[i] + Vector3.up;
                if (UnityEngine.Physics.Raycast(pos , Vector3.down, out hit, 1.1f))
                {
                    if (_debug)
                        Debug.DrawRay(pos, Vector3.down * 1.1f, Color.green);

                    _standingCollider = hit.collider;

                    if (!_grounded)
                    {
                        Collider.transform.position = new Vector3(Collider.transform.position.x, hit.point.y + 0.1f, Collider.transform.position.z);
                        _jumping = false;
                    }

                    _grounded = true;

                    return;
                }
            }

            _standingCollider = null;
            _grounded = false;
        }

        public virtual void SetCollider(Collider collider, Rigidbody rigidbody)
        {
            Collider = collider;
            Rigidbody = rigidbody;
        }
    }
}