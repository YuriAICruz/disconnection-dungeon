using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private List<CameraBehaviorVolume> _volumes;
        private CameraBehaviorVolume _currentVolume;
        private int _levelMask;
        private float _fade;
        private Coroutine _routine;
        private float _maxDistance = 200; 

        private void Awake()
        {
            _manager = DDManager.Instance;
            _levelMask |= 1 << LayerMask.NameToLayer("Level");
        }

        private void Start()
        {
            _volumes = FindObjectsOfType<CameraBehaviorVolume>().ToList();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            _position = _target.position;
        }

        private void Update()
        {
            if (_target == null) return;

            CheckVolumes(_target.position);
            UpdateVolume();
            CheckOcclusion();
        }

        private void UpdateVolume()
        {
            if (_currentVolume == null)
            {
                FollowTarget();
                return;
            }

            if (_currentVolume.Fixed)
            {
                transform.rotation = Quaternion.LookRotation(_target.position-transform.position);
                MoveToVolume();
                Offset = Vector3.Lerp(Offset, Vector3.zero, Time.deltaTime);
            }
            else
            {
                Offset = Vector3.Lerp(Offset, _currentVolume.CamraOffset, Time.deltaTime);
                
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_currentVolume.CamraEulerAngles), Time.deltaTime);
                
                FollowTarget();
            }
        }

        void MoveToVolume()
        {
            var point = _currentVolume.transform.TransformPoint(_currentVolume.CamraOffset);
            var dir = (point - _position);

            if (dir.magnitude > _maxDistance)
            {
                _position = point;
            }
            else
            {
                _position += dir * Speed * Time.deltaTime;
            }

            var pos = Offset + _position;
            transform.position = pos;
        }
        
        private void FollowTarget()
        {
            var dir = (_target.position - _position);

            if (dir.magnitude > _maxDistance)
            {
                _position = _target.position;
            }
            else
            {
                _position += dir * Speed * Time.deltaTime;
            }

            var pos = Offset + _position;
            transform.position = pos;
        }

        private void CheckVolumes(Vector3 pos)
        {
            var res = _volumes.Find(x => FilterVolume(x, pos));

            if (res == null) return;

            _currentVolume = res;
        }

        private bool FilterVolume(CameraBehaviorVolume volume, Vector3 pos)
        {
            var distance = volume.transform.position - pos;

            if (distance.magnitude <= volume.Radius)
                return true;

            return false;
        }

        private void CheckOcclusion()
        {
            var dir = transform.position - _target.position;

            var hits = UnityEngine.Physics.RaycastAll(_target.position, dir, dir.magnitude, _levelMask);

            if (hits.Length == 0)
            {
                _fade = 1;
                return;
            }

            var tm = Shader.GetGlobalVector("_Time");

            foreach (var hit in hits)
            {
                var rdr = hit.transform.GetComponent<Renderer>();

                if (rdr == null) return;

                rdr.material.SetFloat("_TimeReset", tm.y + _fade);
                rdr.material.SetFloat("_Opaque", 0);

                _fade = Mathf.Max(0, _fade - Time.deltaTime);
            }
        }
    }
}