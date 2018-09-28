using System.Collections;
using Graphene.Acting;
using Graphene.Acting.Collectables;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public abstract class DActor : Actor
    {
        protected IActorController _actorController;

        protected CameraBehaviour _camera;

        protected CharacterPhysics _physics;

        protected AnimationManager _animation;

        public float Speed;
        [SerializeField] protected float _dodgeDuration;

        protected bool _canInteract = false;
        protected IInteractible _currentIntreactible;
        protected bool _canClear;
        protected Weapon _weapon;

        private void Awake()
        {
            _physics = new CharacterPhysics(GetComponent<Rigidbody>(), GetComponent<CapsuleCollider>(), Camera.main.transform);

            _animation = new AnimationManager(GetComponent<Animator>());

            _weapon = transform.GetComponentInChildren<Weapon>();
            if (_weapon != null)
            {
                _weapon.SetOwner(this);
            }

            Life.Reset();
            Life.OnDie += OnDie;

            OnAwake();
        }
        
        protected virtual void OnAwake()
        {
            
        }

        private void Start()
        {
            _actorController = Utils.InterfaceHelper.GetInterfaceComponent<IActorController>(this);

            if (_actorController.isLocalPlayer)
            {
                OnEnable();
            }

            _camera = FindObjectOfType<CameraBehaviour>();
            
            if (_actorController.isLocalPlayer)
                _camera.SetTarget(this.transform);

            OnStart();
        }

        protected virtual void OnStart()
        {
            
        }

        protected virtual void OnEnabled()
        {
            
        }
        
        protected virtual void OnDisabled()
        {
            
        }


        private void OnEnable()
        {
            if (_actorController == null || !_actorController.isLocalPlayer) return;
            
            _physics.OnEdge += Jump;
            _physics.OnWallClose += TouchWall;
            _physics.JumpState += _animation.Jump;
            _physics.GroundState += _animation.SetGroundState;

            OnEnabled();
        }

        private void OnDisable()
        {
            if (_actorController == null || !_actorController.isLocalPlayer) return;
            
            _physics.OnEdge -= Jump;
            _physics.OnWallClose -= TouchWall;
            _physics.JumpState -= _animation.Jump;
            _physics.GroundState -= _animation.SetGroundState;
            
            OnDisabled();
        }

        protected void Look(Vector2 dir)
        {
            if (dir.magnitude <= 0) return;

            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));
        }

        private void OnTriggerEnter(Collider other)
        {
            var col = other.transform.GetComponent<ICollectable>();
            if (col != null)
            {
                col.Collect(this);
            }

            var intreactible = other.transform.GetComponent<IInteractible>();

            if (intreactible != null)
            {
                _currentIntreactible = intreactible;
                _canClear = false;
            }
        }

        private void TouchWall(int side)
        {
            _animation.TouchWall(
                new Vector2Int(
                    side % 2 * ((side + 1) % 2 - side + 2),
                    (side + 1) % 2 * Mathf.Min(1, side) * ((side + 1) % 2 - side + 2))
            );
        }
        
        protected virtual void Jump()
        {
            _physics.Jump(Speed);
        }

        public override void DoDamage(int damage, Vector3 from)
        {
            StartCoroutine(ReceiveDamage());
            Life.ReceiveDamage(damage);
            _animation.ReceiveDamage();
        }

        protected virtual IEnumerator ReceiveDamage()
        {
            yield break;
        }

        protected virtual void OnDie()
        {
            Debug.Log(gameObject + "Died");
            _animation.Die();
        }

        private void OnTriggerExit(Collider other)
        {
        }

        private void OnCollisionEnter(Collision other)
        {
            var col = other.transform.GetComponent<ICollectable>();
            if (col != null)
            {
                col.Collect(this);
            }
        }

        private void OnCollisionExit(Collision other)
        {
        }

        public void FootR()
        {
        }

        public void FootL()
        {
        }

        public void Hit()
        {
        }
        
        public void Land()
        {
        }
    }
}