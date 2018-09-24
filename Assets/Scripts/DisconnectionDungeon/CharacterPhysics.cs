using System;
using System.Xml.Schema;
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
        private float _stepAngle;

        public event Action OnEdge;

        public CharacterPhysics(Rigidbody rigidbody, CapsuleCollider collider, Transform camera)
        {
            _collider = collider;
            _camera = camera;
            Rigidbody = rigidbody;
            SetCollider(collider, rigidbody);
        }

        public void Move(Vector2 dir, float speed)
        {
            CheckGround();

            dir = Vector2.ClampMagnitude(dir, 1);

            var wdir = _camera.TransformDirection(new Vector3(dir.x, 0, dir.y));

            var moveDirection = GetGroundOrient(wdir).normalized;

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

            UnityEngine.Physics.Raycast(pos, -_collider.transform.up, out rayhit, 1f);

            if (rayhit.collider == null) return Vector3.zero;

            var distance = CheckBounds(rayhit);

            if (distance < 0.2f) OnEdge?.Invoke();

            var cross = Vector3.Cross(_collider.transform.right, rayhit.normal);

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

        private Vector3 CheckSurround(Vector2 wdir)
        {
            var pos = _collider.transform.position;
            RaycastHit rayhit;

            if (wdir.magnitude <= 0 && _standingCollider != null)
            {
                UnityEngine.Physics.Raycast(pos, _collider.transform.forward, out rayhit, 1);

                return CalculateBounds(rayhit, _standingCollider, pos, wdir);
            }

            var hits = UnityEngine.Physics.SphereCastAll(_collider.transform.position, _surroundRadius, Vector3.down, _surroundRadius);

            UnityEngine.Physics.Raycast(pos, wdir, out rayhit, 1);

            foreach (var hit in hits)
            {
                var res = CalculateBounds(rayhit, hit.collider, pos, wdir);
                if (res.magnitude > 0)
                    return res;
            }
            return Vector3.zero;
        }

        private Vector3 CalculateBounds(RaycastHit rayhit, Collider collider, Vector3 pos, Vector3 wdir)
        {
            if (rayhit.collider == collider)
            {
                Debug.DrawLine(pos, collider.transform.position, Color.blue);
                Debug.DrawRay(rayhit.point, rayhit.normal, Color.blue);

                var res = rayhit.normal + wdir.normalized;

                var cross = Vector3.Cross(_collider.transform.right, res);

                Debug.DrawRay(rayhit.point + rayhit.normal, cross, Color.blue);

                Debug.Log(rayhit.normal + wdir.normalized);
                return -cross;
            }

            Debug.DrawLine(pos, collider.transform.position, Color.magenta);
            return Vector3.zero;
        }

        public float Speed()
        {
            return new Vector3(_velocity.x, 0, _velocity.z).magnitude;
        }

        public void Jump(float speed)
        {
            if (_jumping || !_grounded) return;

            _jumping = true;
            _velocity.y = speed;
        }
    }
}