using Graphene.Acting;
using Graphene.DisconnectionDungeon.InputSystem;
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

        private InputManager _input;

        private AnimationManager _animation;

        public float Speed;

        bool _canInteract = false;

        private void Awake()
        {
            _physics = new CharacterPhysics(GetComponent<Rigidbody>(), GetComponent<CapsuleCollider>(), Camera.main.transform);

            _animation = new AnimationManager(GetComponent<Animator>());
        }

        private void Start()
        {
            _actorController = Utils.InterfaceHelper.GetInterfaceComponent<IActorController>(this);

            if (_actorController.isLocalPlayer)
            {
                _input = new InputManager();
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

            _input.Direction += Move;
            _input.Interact += Interact;
            _input.Attack += Attack;
            _input.Jump += Jump;
        }

        private void OnDisable()
        {
            if (_input == null)
                return;

            _input.Direction -= Move;
            _input.Interact -= Interact;
            _input.Attack -= Attack;
            _input.Jump -= Jump;
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
    }
}