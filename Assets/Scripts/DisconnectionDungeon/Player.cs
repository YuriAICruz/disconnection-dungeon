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

        private CharacterPhysics _physics;
        
        private InputManager _input;

        private AnimationManager _animation;

        public float Speed;

        private void Awake()
        {
            
            _physics = new CharacterPhysics(GetComponent<Rigidbody>(), GetComponent<CapsuleCollider>());
            
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
        }

        private void OnEnable()
        {
            if(_input == null)
                return;
            
            _input.Direction += Move;
            _input.Interact += Interact;
            _input.Attack += Attack;
        }

        private void OnDisable()
        {
            _input.Direction -= Move;
            _input.Interact -= Interact;
            _input.Attack -= Attack;
        }

        private void Attack()
        {
            _animation.Attack();
        }

        private void Interact()
        {
            _animation.Interact();
        }

        private void Move(Vector2 dir)
        {
            _physics.Move(dir, Speed);

            _animation.SetSpeed(_physics.Speed());
        }
    }
}