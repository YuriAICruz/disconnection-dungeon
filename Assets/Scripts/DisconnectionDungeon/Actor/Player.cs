using System.Collections;
using Graphene.Acting;
using Graphene.Acting.Collectables;
using Graphene.DisconnectionDungeon.InputSystem;
using Graphene.InputManager;
using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class Player : Actor
    {
        private IActorController _actorController;

        private CameraBehaviour _camera;

        private CharacterPhysics _physics;

        [HideInInspector]
        [SerializeField] 
        private DisconnectionDungeonInputManager _input;

        private AnimationManager _animation;

        public float Speed;
        [SerializeField] private float _dodgeDuration;

        bool _canInteract = false;
        private IInteractible _currentIntreactible;
        private bool _canClear;
        private Weapon _weapon;

        private void Awake()
        {
            _physics = new CharacterPhysics(GetComponent<Rigidbody>(), GetComponent<CapsuleCollider>(), Camera.main.transform);

            _animation = new AnimationManager(GetComponent<Animator>());

            _weapon = transform.GetComponentInChildren<Weapon>();

            Life.Reset();
            Life.OnDie += OnDie;
        }

        private void Start()
        {
            _actorController = Utils.InterfaceHelper.GetInterfaceComponent<IActorController>(this);

            if (_actorController.isLocalPlayer)
            {
                _input.Init();
                OnEnable();
            }

            _camera = FindObjectOfType<CameraBehaviour>();
            if (_actorController.isLocalPlayer)
                _camera.SetTarget(this.transform);
        }

        private void OnEnable()
        {
            if (_input == null)
                return;

            _input.Left_Axis += Move;
            _input.Interact += Interact;
            _input.Attack += Attack;
            _input.AttackSeq += AttackSeq;
            _input.Jump += Jump;
            _input.Dodge += Dodge;

            _physics.OnEdge += Jump;
            _physics.OnWallClose += TouchWall;
            _physics.JumpState += _animation.Jump;
            _physics.GroundState += _animation.SetGroundState;
        }

        private void Dodge()
        {
            _animation.Dodge();
            _input.BlockInputs();
            _physics.Dodge(_dodgeDuration, Speed*2, () =>
            {
                _input.UnblockInputs();
            });
        }

        private void TouchWall(int side)
        {
            _animation.TouchWall(
                new Vector2Int(
                    side % 2 * ((side + 1) % 2 - side + 2),
                    (side + 1) % 2 * Mathf.Min(1, side) * ((side + 1) % 2 - side + 2))
            );
        }

        private void OnDisable()
        {
            if (_input == null)
                return;

            _input.Left_Axis -= Move;
            _input.Interact -= Interact;
            _input.Attack -= Attack;
            _input.AttackSeq -= AttackSeq;
            _input.Jump -= Jump;

            _physics.OnEdge -= Jump;
            _physics.OnWallClose -= TouchWall;
            _physics.JumpState -= _animation.Jump;
            _physics.GroundState -= _animation.SetGroundState;
        }

        private void Jump()
        {
            _physics.Jump(Speed);
        }

        private void Attack()
        {
            if (_canInteract) return;
            _animation.Attack();
        }

        private void AttackSeq()
        {
            if (_canInteract) return;
            _animation.AttackSeq();
        }

        private void Interact()
        {
            if (!_canInteract) return;
            _animation.Interact();
        }

        private void Move(Vector2 dir)
        {
            _physics.Move(dir, Speed);

            Look(dir);

            _animation.SetSpeed(_physics.Speed());
        }

        private void Look(Vector2 dir)
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

        public override void DoDamage(int damage)
        {
            StartCoroutine(ReceiveDamage());
            Life.ReceiveDamage(damage);
            _animation.ReceiveDamage();
        }

        IEnumerator ReceiveDamage()
        {
            _input.BlockInputs();
            Move(Vector2.zero);
            yield return new WaitForSeconds(0.6f);
            _input.UnblockInputs();
        }

        private void OnDie()
        {
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
            _weapon.SetEnabled(true);
        }
        public void HitEnd()
        {
            _weapon.SetEnabled(false);
        }

        public void Land()
        {
        }
    }
}