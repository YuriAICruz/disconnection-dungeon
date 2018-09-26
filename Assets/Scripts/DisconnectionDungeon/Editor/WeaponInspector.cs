﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    [CustomEditor(typeof(Weapon), true)]
    public class WeaponInspector : Editor
    {
        private Weapon _self;

        private void Awake()
        {
            _self = (Weapon) target;
        }

        private void OnSceneGUI()
        {
            Debug.DrawRay(_self.transform.TransformPoint(_self.Offset), _self.transform.forward * _self.Height, Color.magenta);

            EditorGUI.BeginChangeCheck();
            var offset = Handles.DoPositionHandle(_self.transform.TransformPoint(_self.Offset), Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(_self, "Move Offset");
                EditorUtility.SetDirty(_self);
                _self.Offset = _self.transform.InverseTransformPoint(offset);
            }
        }
    }
}