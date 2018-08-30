using System.Collections;
using DisconnectionDungeon.InputSystem;
using Graphene.Acting;
using UnityEngine;
using Physics = UnityEngine.Physics;

namespace DisconnectionDungeon
{
    public class Player : Actor
    {
        private DDManager _manager;

        public CharacterPhysics Physics;

        private InputManager _input;
        private bool _moving;

        private void Awake()
        {
            Life.OnDie += Die;

            _input = new InputManager();
            _input.Direction += Move;
            _input.Interact += Interact;
        }

        private void Move(Vector2 dir)
        {
            var dirInt = new Vector2Int((int)(dir.x), (int)(dir.y));

            if (Physics.CheckCollision(transform.position, dirInt) || dirInt.magnitude <=0 || _moving) return;
            
            StartCoroutine(Mover(dirInt));
        }

        IEnumerator Mover(Vector2Int dir)
        {
            _moving = true;

            var v3Dir = new Vector3(dir.x, dir.y);
            var finalPos = transform.position + v3Dir;
            while (true)
            {
                transform.position += v3Dir * Time.deltaTime * Physics.Speed;

                yield return null;

                if ((finalPos - transform.position).magnitude < 0.1f)
                {
                    transform.position = finalPos;
                    break;
                }
            }
            
            _moving = false;
        }

        private void Interact()
        {
            
        }

        private void Start()
        {
            _manager = FindObjectOfType<DDManager>();
        }

        private void Die()
        {
            _manager.OnPlayerDie();

            Debug.LogError("Die");
        }

        public override void DoDamage(int damage)
        {
            Life.ReceiveDamage(damage);
        }
    }
}