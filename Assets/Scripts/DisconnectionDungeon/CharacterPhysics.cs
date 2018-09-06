using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class CharacterPhysics
    {
        public CapsuleCollider Collider;
        public Rigidbody Rigidbody;

        private Vector3 _velocity;

        public CharacterPhysics(Rigidbody rigidbody, CapsuleCollider collider)
        {
            Collider = collider;
            Rigidbody = rigidbody;
        }
        
        public void Move(Vector2 dir, float speed)
        {
            _velocity.x = dir.x * speed;
            _velocity.y = dir.y * speed;
            
            Rigidbody.velocity = _velocity;
        }

        public float Speed()
        {
            return _velocity.magnitude;
        }
    }
}