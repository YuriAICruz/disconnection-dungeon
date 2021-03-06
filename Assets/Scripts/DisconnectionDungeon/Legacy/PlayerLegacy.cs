using System.Collections;
using Graphene.Acting;
using Graphene.Acting.Collectables;
using Graphene.Acting.Interfaces;
using Graphene.InputManager.Platformer;
using UnityEngine;
using Physics = UnityEngine.Physics;

namespace Graphene.DisconnectionDungeon
{
    [RequireComponent(typeof(PlatformerInputManager))]
    [RequireComponent(typeof(CharacterPhysicsLegacy))]
    public class PlayerLegacy : Actor
    {
        private DDManager _manager;
        
        public CharacterPhysicsLegacy Physics;

        private SpriteRenderer _renderer;

        private PlatformerInputManager _input;
        private bool _moving;
        private IInteractible _currentIntreactible;

        private bool _canClear;

        private void Awake()
        {
            Life.OnDie += Die;

            _input = GetComponent<PlatformerInputManager>();
            _input.Left_Axis += Move;
            _input.Interact += Interact;
            _input.Pause += Pause;

            _renderer = GetComponent<SpriteRenderer>();

            Physics = GetComponent<CharacterPhysicsLegacy>();
            
            Physics.SetCollider(GetComponent<Collider2D>());

            Physics.OnTriggerEnter += OnTriggered;
            Physics.OnCollisionEnter += OnCollided;
        }

        private void Start()
        {
            _manager = DDManager.Instance;
        }
        
        private void Move(Vector2 dir)
        {
            var dirInt = new Vector2Int((int) (dir.x), (int) (dir.y));

            _canClear = true;

            if (Physics.CheckCollision(transform.position, dirInt) || dirInt.magnitude <= 0 || _moving) return;

            StartCoroutine(Mover(dirInt));
        }

        IEnumerator Mover(Vector2Int dir)
        {
            if(_moving)
                yield break;
            
            _moving = true;
            
            if (dir.x > 0)
                _renderer.flipX = true;
            else if (dir.x < 0)
                _renderer.flipX = false;

            var v3Dir = new Vector3(dir.x, dir.y);
            var finalPos = transform.position + v3Dir;
            var time = 0f;
            while (true)
            {
                transform.position += v3Dir * Time.deltaTime * Physics.Speed;

                yield return null;

                time += Time.deltaTime;

                if ((finalPos - transform.position).magnitude < 0.1f)
                {
                    transform.position = finalPos;
                    break;
                }
                
                if(time > Physics.Speed)
                    break;
            }

            _moving = false;

            if (_canClear)
                _currentIntreactible = null;
        }

        public void Transport(Vector3Int destination, bool popup)
        {
            if (popup)
            {
                transform.position = destination;
                return;
            }
            
            if(_moving)
                return;

            var dir = destination - transform.position;
            StartCoroutine(Mover(new Vector2Int((int) dir.x, (int) dir.y)));
        }

        public bool CanInteract()
        {
            return _currentIntreactible != null;
        }

        private void Interact()
        {
            if (_currentIntreactible == null) return;

            _currentIntreactible.Interact();
        }

        private void Pause()
        {
            _manager.OnPause();
        }

        private void Die()
        {
            _manager.OnPlayerDie();

            Debug.LogError("Die");
        }

        public override void DoDamage(int damage, Vector3 from)
        {
            Life.ReceiveDamage(damage);
        }

        protected void OnCollided(RaycastHit2D hit)
        {
            var col = hit.transform.GetComponent<ICollectable>();
            if (col != null)
            {
                col.Collect(this);
            }
        }

        protected void OnTriggered(RaycastHit2D hit)
        {
            var col = hit.transform.GetComponent<ICollectable>();
            if (col != null)
            {
                col.Collect(this);
            }

            var intreactible = hit.transform.GetComponent<IInteractible>();

            if (intreactible != null)
            {
                _currentIntreactible = intreactible;
                _canClear = false;
            }
        }
    }
}