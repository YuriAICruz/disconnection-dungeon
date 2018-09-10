using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class CharacterPhysics
    {
        public CapsuleCollider Collider;
        private Transform _camera;
        public Rigidbody Rigidbody;

        private Vector3 _velocity;
        private float _gravity = 9.8f;

        private bool _grounded, _debug = true;
        private float _surroundRadius = 3;

        public CharacterPhysics(Rigidbody rigidbody, CapsuleCollider collider, Transform camera)
        {
            Collider = collider;
            _camera = camera;
            Rigidbody = rigidbody;
        }

        void CheckGround()
        {
            RaycastHit hit;

            if (UnityEngine.Physics.Raycast(Collider.transform.position, Vector3.down, out hit, 1.1f))
            {
                if (_debug)
                    Debug.DrawRay(Collider.transform.position, Vector3.down * 1.1f, Color.green);
                
                _grounded = true;
                return;
            }

            _grounded = false;
        }

        public void Move(Vector2 dir, float speed)
        {
            CheckSurround();
            CheckGround();

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
            var hits = UnityEngine.Physics.SphereCastAll(Collider.transform.position, _surroundRadius, Vector3.down, _surroundRadius);

            foreach (var hit in hits)
            {
                Debug.DrawLine(Collider.transform.position, hit.collider.transform.position, Color.magenta);
            }
        }

        public float Speed()
        {
            return _velocity.magnitude;
        }

        public void Jump(float speed)
        {
            if (!_grounded) return;

            _velocity.y = speed;
        }
    }
}