using UnityEditorInternal;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class CameraBehaviour : MonoBehaviour
    {
        private DDManager _manager;
        private Transform _target;

        public float Speed;
        private Vector3 _position;
        public Vector3 Offset;

        private void Awake()
        {
            _manager = DDManager.Instance;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            _position = _target.position;
        }

        private void Update()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            if (_target == null) return;

            var dir = (_target.position - _position);
            
            if (dir.magnitude > 10)
            {
                _position = _target.position;
            }
            else
            {
                _position += dir * Speed * Time.deltaTime;
            }

            transform.position = Offset + _position;
        }
    }
}