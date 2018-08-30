using UiGenerics;
using UnityEngine;

namespace DisconnectionDungeon.Presentation
{
    public class InteractHint : CanvasGroupView
    {
        private Player _player;

        private bool _isShown;

        void Setup()
        {
        }

        private void Start()
        {
            _player = FindObjectOfType<Player>();

            Hide();
            _isShown = false;
        }

        private void Update()
        {
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