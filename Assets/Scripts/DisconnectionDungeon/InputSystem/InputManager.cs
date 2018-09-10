using System;
using System.Collections;
using System.IO;
using Graphene.Utils;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.InputSystem
{
    public class InputManager
    {
        private Coroutine _update;
        public event Action Interact, Pause, Attack, Jump;
        public event Action<Vector2> Direction; 
        
        public InputManager()
        {
            _update = GlobalCoroutineManager.Instance.StartCoroutine(ReadInputs());
        }

        private void GetInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Interact?.Invoke();
            }

            if (Input.GetButtonDown("Pause"))
            {
                Pause?.Invoke();
            }

            if (Input.GetButtonDown("Jump"))
            {
                Jump?.Invoke();
            }

            if (Direction != null)
            {
                Direction(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
            }
        }

        IEnumerator ReadInputs()
        {
            while (true)
            {
                GetInputs();
                yield return new WaitForChangedResult();
            }
        }

        public void OnDestroy()
        {
            if (_update != null)
            {
                GlobalCoroutineManager.Instance.StopCoroutine(_update);
            }
        }
    }
}