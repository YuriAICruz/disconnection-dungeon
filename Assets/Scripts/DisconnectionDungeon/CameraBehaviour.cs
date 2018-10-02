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

        private void Awake()
        {
            _manager = DDManager.Instance;
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
            
            UpdateVolume();
            FollowTarget();
            CheckOcclusion();
        }

        private void UpdateVolume()
        {
            if(_currentVolume == null) return;
            
            Offset = Vector3.Lerp(Offset, _currentVolume.CamraOffset, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_currentVolume.CamraEulerAngles), Time.deltaTime);
        }

        private void FollowTarget()
        {
            var dir = (_target.position - _position);
            
            if (dir.magnitude > 10)
            {
                _position = _target.position;
            }
            else
            {
                _position += dir * Speed * Time.deltaTime;
            }

            var pos = Offset + _position;
            CheckVolumes(_target.position);
            transform.position = pos;
        }

        private void CheckVolumes(Vector3 pos)
        {
            var res = _volumes.Find(x=>FilterVolume(x,pos));
            
            if(res == null) return;

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
            RaycastHit hit;
            var dir = transform.position - _target.position;
            if (!UnityEngine.Physics.Raycast(_target.position, dir, out hit, dir.magnitude)) return;
            
            Debug.DrawRay(_target.position, dir.normalized * hit.distance, Color.red);

            var rdr = hit.transform.GetComponent<Renderer>();
            rdr.material.SetFloat("TimeReset", 0);
        }
    }
}