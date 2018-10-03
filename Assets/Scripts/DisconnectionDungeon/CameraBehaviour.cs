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
        private List<Renderer> _renderers = new List<Renderer>();
        private float _fade;
        private Coroutine _routine;

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

            UpdateVolume();
            FollowTarget();
            CheckOcclusion();
        }

        private void UpdateVolume()
        {
            if (_currentVolume == null) return;

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
                if(_routine == null)
                    _routine = StartCoroutine(ResetRenderers());
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

                _renderers.Add(rdr);
            }
        }

        private IEnumerator ResetRenderers()
        {
            var t = 0f;
            while (t<1)
            {
                foreach (var renderer in _renderers)
                {
                    renderer.material.SetFloat("_Opaque", 0);
                }
                yield return null;
                t += Time.deltaTime;
            }
            foreach (var renderer in _renderers)
            {
                renderer.material.SetFloat("_Opaque", 1);
            }
            _renderers = new List<Renderer>();
            _routine = null;
        }
    }
}