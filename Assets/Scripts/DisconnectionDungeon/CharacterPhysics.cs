using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class CharacterPhysics : BasicPhysics
    {
        public CapsuleCollider _collider;
        private Transform _camera;

        private Vector3 _velocity;
        private float _gravity = 9.8f;

        private float _surroundRadius = 3;

        public CharacterPhysics(Rigidbody rigidbody, CapsuleCollider collider, Transform camera)
        {
            _collider = collider;
            _camera = camera;
            Rigidbody = rigidbody;
            SetCollider(collider, rigidbody);
        }

        public void Move(Vector2 dir, float speed)
        {
            CheckSurround();
            CheckGround();

            dir = Vector2.ClampMagnitude(dir, 1);

            var wdir = _camera.TransformDirection(new Vector3(dir.x, 0, dir.y));

            _velocity.x = wdir.x * speed;
            _velocity.z = wdir.z * speed;

            if (_grounded)
            {
                _velocity.y = Mathf.Max(_velocity.y, 0);
            }
            else
            {
                _velocity.y -= _gravity * Time.deltaTime;

                _velocity.y = Mathf.Max(_velocity.y, -_gravity * 2);
            }

            Rigidbody.velocity = _velocity;
        }

        private void CheckSurround()
        {
            var hits = UnityEngine.Physics.SphereCastAll(_collider.transform.position, _surroundRadius, Vector3.down, _surroundRadius);

            foreach (var hit in hits)
            {
                Debug.DrawLine(_collider.transform.position, hit.collider.transform.position, Color.magenta);
            }
        }

        public float Speed()
        {
            return new Vector3(_velocity.x, 0, _velocity.z).magnitude;
        }

        public void Jump(float speed)
        {
            if (!_grounded) return;

            _velocity.y = speed;
        }
    }
}