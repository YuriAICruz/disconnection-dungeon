using Graphene.DisconnectionDungeon;
using UiGenerics;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.Presentation.Pages.GameInterface
{
    public class InteractHint : CanvasGroupView
    {
        private PlayerLegacy _player;

        private bool _isShown;

        void Setup()
        {
        }

        private void Start()
        {
            FindPlayer();

            Hide();
            _isShown = false;
        }

        private void FindPlayer()
        {
            if(_player == null)
                _player = FindObjectOfType<PlayerLegacy>();
        }

        private void Update()
        {
            FindPlayer();
            
            
            if(_player == null)
                return;
            
            if (_player.CanInteract() && !_isShown)
            {
                Show();
                _isShown = true;
            }
            else if (!_player.CanInteract() && _isShown)
            {
                Hide();
                _isShown = false;
            }
        }
    }
}