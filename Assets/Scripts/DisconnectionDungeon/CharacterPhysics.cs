using System;
using System.Collections;
using System.Xml.Schema;
using Graphene.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Graphene.DisconnectionDungeon
{
    public class CharacterPhysics : BasicPhysics
    {
        public CapsuleCollider _collider;
        private Transform _camera;

        private Vector3 _velocity;
        private float _gravity = 9.8f;

        private float _surroundRadius = 3;
        private float _stepAngle;

        public event Action OnEdge;
        public event Action<int> OnWallClose;

        public CharacterPhysics(Rigidbody rigidbody, CapsuleCollider collider, Transform camera)
        {
            _collider = collider;
            _camera = camera;
            Rigidbody = rigidbody;
            SetCollider(collider, rigidbody);
        }

        public void Move(Vector2 dir, float speed, bool transformDir = true)
        {
            CheckGround();

            dir = Vector2.ClampMagnitude(dir, 1);
            
            var wdir = transformDir ? _camera.TransformDirection(new Vector3(dir.x, 0, dir.y)) : new Vector3(dir.x, 0, dir.y);

            var moveDirection = GetGroundOrient(wdir).normalized;

            CheckSurround(wdir);

            _velocity.x = moveDirection.x * speed;
            _velocity.z = moveDirection.z * speed;

            if (!_grounded || _velocity.magnitude <= 0)
            {
                _velocity.x = wdir.x * speed;
                _velocity.z = wdir.z * speed;
            }

            if (_grounded)
            {
                if(!_jumping)
                _velocity.y = moveDirection.y * speed;

                _velocity.y = Mathf.Max(_velocity.y, 0);
            }
            else
            {
                _velocity.y -= _gravity * Time.deltaTime;

                _velocity.y = Mathf.Max(_velocity.y, -_gravity * 2);
            }

            Rigidbody.velocity = _velocity;
        }

        private Vector3 GetGroundOrient(Vector3 wdir)
        {
            if (wdir.magnitude <= 0) return Vector3.zero;

            var pos = _collider.transform.position;
            RaycastHit rayhit;

            UnityEngine.Physics.Raycast(pos+Vector3.up, -_collider.transform.up, out rayhit, 2f);

            if (rayhit.collider == null) return Vector3.zero;

            var distance = CheckBounds(rayhit);

            if (distance < 0.2f) OnEdge?.Invoke();

            var rot = Quaternion.AngleAxis(90, _collider.transform.up) * wdir;
            
            var cross = Vector3.Cross(rot, rayhit.normal);
            
            Debug.DrawRay(_collider.transform.position + Vector3.up, cross.normalized*2, Color.magenta);

            _stepAngle = Vector3.Angle(cross, Vector3.down);

//            Debug.DrawRay(rayhit.point, rayhit.normal, Color.blue);
//            Debug.DrawRay(rayhit.point + rayhit.normal, cross, Color.blue);

            return cross;
        }

        private float CheckBounds(RaycastHit rayhit)
        {
            var bounds = rayhit.collider.bounds;
            var corners = new Vector3[]
            {
                bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z),
                bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z),
                bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z),
                bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z),
            };

            var result = Mathf.Infinity;
            for (int i = 0, n = corners.Length; i < n; i++)
            {
                Debug.DrawLine(corners[i], corners[(i + 1)%n], Color.magenta);
                var line = -corners[i] + corners[(i + 1) % n];
                var value = Vector3.Cross(line, _collider.transform.position - corners[i]).magnitude;

                if (value < result)
                {
                    result = value / line.magnitude;
                }
            }
            
//            Debug.Log(result);
            return result;
        }

// TODO Bounds from mesh
//        Bounds CalculateFromMesh(Mesh mesh)
//        {
//            if (aObj == null)
//            {
//                Debug.LogError("CalculateBoundingBox: object is null");
//                return new Bounds(Vector3.zero, Vector3.one);
//            }
//            Transform myTransform = aObj.transform;
//            Mesh mesh = null;
//            MeshFilter mF = aObj.GetComponent<MeshFilter>();
//                if (mF != null)
//                mesh = mF.mesh;
//                else
//            {
//                SkinnedMeshRenderer sMR = aObj.GetComponent<SkinnedMeshRenderer>();
//                if (sMR != null)
//                    mesh = sMR.sharedMesh;
//            }
//        if (mesh == null)
//        {
//        Debug.LogError("CalculateBoundingBox: no mesh found on the given object");
//        return new Bounds(aObj.transform.position, Vector3.one);
//        }
//        Vector3[] vertices = mesh.vertices;
//        if (vertices.Length <=0)
//        {
//        Debug.LogError("CalculateBoundingBox: mesh doesn't have vertices");
//        return new Bounds(aObj.transform.position, Vector3.one);
//        }
//        Vector3 min, max;
//        min = max = myTransform.TransformPoint(vertices[0]);
//        for (int i = 1; i < vertices.Length; i++)
//        {
//        Vector3 V = myTransform.TransformPoint(vertices[i]);
//            for (int n = 0; n < 3; n++)
//        {
//            if (V[n] > max[n])
//                max[n] = V[n];
//            if (V[n] < min[n])
//                min[n] = V[n];
//        }
//        }
//        Bounds B = new Bounds();
//        B.SetMinMax(min, max);
//        return B;
//        }

        private void CheckSurround(Vector2 wdir)
        {
            var pos = _collider.transform.position + Vector3.up;
            RaycastHit rayhit;

            for (int i = 1, n = _sides.Length; i < n; i++)
            {
                if (!UnityEngine.Physics.Raycast(pos, _collider.transform.TransformDirection(_sides[i]), out rayhit, 2)) continue;


                if (rayhit.distance < 1f)
                {
                    OnWallClose?.Invoke(i);
                    Debug.DrawLine(pos, rayhit.point, Color.red);
                    return;
                }
                else
                {
                    Debug.DrawLine(pos, rayhit.point, Color.blue);
                }
            }
            
            OnWallClose?.Invoke(0);
        }

        public float Speed()
        {
            return new Vector3(_velocity.x, 0, _velocity.z).magnitude;
        }

        public void Jump(float speed)
        {
            if (_jumping || !_grounded) return;

            SetJumpState(true);
            _velocity.y = speed;
        }

        public void Dodge(float duration, float speed, Action callback)
        {
            var dir = _velocity.normalized;
            if (dir.magnitude == 0)
                dir = _collider.transform.forward;
            GlobalCoroutineManager.Instance.StartCoroutine(DodgeRoutine(dir, duration, speed, callback));
        }

        IEnumerator DodgeRoutine(Vector3 direction, float duration, float speed, Action callback)
        {
            var transform = Collider.transform;
            var time = 0f;
            while (time <= duration)
            {
                Rigidbody.velocity = direction * speed;
                
                yield return null;
                time += Time.deltaTime;
            }
            
            callback?.Invoke();
        }

        public void EnableRagdool()
        {
            Rigidbody.freezeRotation = false;
        }
    }
}