using System.Collections;
using System.Collections.Generic;
using Graphene.Acting;
using Graphene.Acting.Collectables;
using Graphene.Acting.Interfaces;
using Graphene.Acting.Platformer;
using Graphene.Physics.Platformer;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.PuzzleObjects
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PushableBox : MonoBehaviour, ICollectable
    {
        private BoxCollider _collider;
        
        private bool _moving;

        public EnviromentPhysics Physics;
        [SerializeField] private float Dist;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = false;

            //Physics.SetCollider(_collider, GetComponent<Rigidbody>());
        }

        public void Collect(Actor actor)
        {
            var player = (Player) actor;

            if (player == null) return;

            var dir = transform.position - player.transform.position;
            
            if (Mathf.Abs(dir.x) < Mathf.Abs(dir.z))
                dir.x = 0;
            else
                dir.z = 0;
            
            dir.y = 0;
            dir.Normalize();

            if (_moving || Physics.CheckCollision(transform.position, dir)) return;

            StartCoroutine(Mover(dir, player.Speed));
        }


        IEnumerator Mover(Vector3 dir, float speed)
        {
            _moving = true;

            var finalPos = transform.position + dir;
            while (true)
            {
                transform.position += dir * Time.deltaTime * speed;

                yield return null;

                if ((finalPos - transform.position).magnitude < 0.1f)
                {
                    transform.position = finalPos;
                    break;
                }
            }

            _moving = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            var intreactible = other.transform.GetComponent<IInteractible>();

            if (intreactible != null)
            {
                intreactible.Interact();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
        }

        private void OnCollisionEnter(Collision other)
        {
            
        }

        private void OnCollisionExit(Collision other)
        {
            
        }
    }
}