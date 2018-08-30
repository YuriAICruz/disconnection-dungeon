using System;
using System.Collections;
using System.IO;
using Graphene.Utils;
using UnityEngine;

namespace DisconnectionDungeon.InputSystem
{
    public class InputManager
    {
        public event Action Interact;
        public event Action<Vector2> Direction; 
        
        public InputManager()
        {
            GlobalCoroutineManager.Instance.StartCoroutine(ReadInputs());
        }

        private void GetInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (Interact != null) Interact();
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
    }
}