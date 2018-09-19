using UnityEngine;

namespace Graphene.DisconnectionDungeon
{
    public class AnimationManager
    {
        private Animator _animator;

        public AnimationManager(Animator animator)
        {
            _animator = animator;
        }
        
        public void Interact()
        {
            _animator.SetTrigger("Interact");            
        }

        public void Attack()
        {
            _animator.SetTrigger("Attack");   
        }

        public void SetSpeed(float speed)
        {
            _animator.SetFloat("Speed", speed);   
        }
    }
}