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

        private CharacterPhysics _physics;
        
        private InputManager _input;

        private AnimationManager _animation;

        public float Speed;

        private void Awake()
        {
            _input = new InputManager();

            _input.Direction += Move;
            _input.Interact += Interact;
            _input.Attack += Attack;
            
            _physics = new CharacterPhysics(GetComponent<Rigidbody>(), GetComponent<CapsuleCollider>());
            
            _animation = new AnimationManager(GetComponent<Animator>());
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