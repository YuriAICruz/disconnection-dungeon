using System;
using System.Collections.Generic;
using Graphene.InputManager.ComboSystem;
using Graphene.Utils;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.InputSystem
{
    [Serializable]
    public class DisconnectionDungeonInputManager : InputManager.InputSystem
    {
        private Coroutine _update;
        public event Action Interact, Pause, Attack, Jump, Dodge;

        protected override void CreateComboData()
        {
            base.CreateComboData();
            _comboAssembly = new Dictionary<ComboChecker, Action>()
            {
                //TODO
            };
        }
    }
}