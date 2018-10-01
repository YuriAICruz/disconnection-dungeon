using UnityEditor;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    [CustomEditor(typeof(CameraBehaviorVolume))]
    public class CameraBehaviorVolumeInspector : Editor
    {
        private CameraBehaviorVolume _self;
        private bool _canEditCamera;

        private void Awake()
        {
            _self = (CameraBehaviorVolume) target;
        }

        private void OnSceneGUI()
        {
            DrawSize();
            DrawCameraPosition();
        }

        private void DrawCameraPosition()
        {
            var point = _self.transform.TransformPoint(_self.CamraOffset);
            var rotation = Quaternion.Euler(_self.CamraEulerAngles);
            if (Handles.Button(point, rotation, 1.2f, 1.2f, Handles.CubeHandleCap))
            {
                Debug.Log(Tools.current);
                _canEditCamera = true;
            }

            if (!_canEditCamera) return;

            if (Tools.current == Tool.Rotate)
            {
                EditorGUI.BeginChangeCheck();
                rotation = Handles.DoRotationHandle(rotation, point);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_self, "Rotate Camera");
                    EditorUtility.SetDirty(_self);
                    _self.CamraEulerAngles = rotation.eulerAngles;
                }

                DrawCameraView(point, rotation);
                return;
            }

            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_self, "Move Camera Point");
                EditorUtility.SetDirty(_self);
                _self.CamraOffset = _self.transform.InverseTransformPoint(point);
            }

            DrawCameraView(point, rotation);
        }

        private void DrawCameraView(Vector3 point, Quaternion rotation)
        {
            var color = Handles.color;
            var angle = (60 / 2f) * (Mathf.PI / 180);
            var dist = 10;

            Handles.color = Color.green;

            var dir = rotation * Vector3.forward * dist;
            var size = (Mathf.Tan(angle) * dir.magnitude) * 0.1f;
            var center = point + dir;
            var aspect = Screen.height / (float) Screen.width;

            var sideX = Vector3.Cross(dir, rotation * Vector3.up) * size;
            var sideY = Vector3.Cross(dir, rotation * Vector3.right) * aspect * size;

            var sqr = new Vector3[]
            {
                center + sideX + sideY,
                center + sideX - sideY,
                center - sideX - sideY,
                center - sideX + sideY,
            };

            Handles.DrawLine(point, center);

            Handles.color = Color.gray;
            for (int i = 0, n = sqr.Length; i < n; i++)
            {
                Handles.DrawLine(sqr[i], sqr[(i + 1) % n]);
                Handles.DrawLine(point, sqr[i]);
            }
            Handles.color = color;
        }

        private void DrawSize()
        {
            var color = Handles.color;
            Handles.color = new Color(1, 0.4f, 0);
            
            Handles.DrawWireDisc(_self.transform.position, Vector3.right, _self.Radius);
            Handles.DrawWireDisc(_self.transform.position, Vector3.up, _self.Radius);
            Handles.DrawWireDisc(_self.transform.position, Vector3.forward, _self.Radius);
            Handles.color = color;
        }
    }
}