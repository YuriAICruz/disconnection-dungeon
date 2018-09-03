using System.Collections;
using System.Collections.Generic;
using Graphene.Acting;
using Graphene.Acting.Collectables;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.Collectable
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PushableBox : MonoBehaviour, ICollectable
    {
        private BoxCollider2D _collider;
        
        private bool _moving;

        public CharacterPhysics Physics;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _collider.isTrigger = false;

            Physics.SetCollider(_collider);
            Physics.OnTriggerEnter += OnTrigger;
        }

        public void Collect(Actor actor)
        {
            var player = (Player) actor;

            if (player == null) return;

            var dir = transform.position - player.transform.position;
            dir.Normalize();

            var dirInt = new Vector2Int((int) Mathf.Ceil(dir.x), (int) Mathf.Ceil(dir.y));

            if (_moving || Physics.CheckCollision(transform.position, dirInt)) return;

            StartCoroutine(Mover(dirInt, player.Physics.Speed));
        }


        IEnumerator Mover(Vector2Int dir, float speed)
        {
            _moving = true;

            var v3Dir = new Vector3(dir.x, dir.y);
            var finalPos = transform.position + v3Dir;
            while (true)
            {
                transform.position += v3Dir * Time.deltaTime * speed;

                yield return null;

                if ((finalPos - transform.position).magnitude < 0.1f)
                {
                    transform.position = finalPos;
                    break;
                }
            }

            _moving = false;
        }

        private void OnTrigger(RaycastHit2D hit)
        {
            var intreactible = hit.transform.GetComponent<IInteractible>();
            
            Debug.Log(intreactible);

            if (intreactible != null)
            {
                intreactible.Interact();
            }
        }
    }
}