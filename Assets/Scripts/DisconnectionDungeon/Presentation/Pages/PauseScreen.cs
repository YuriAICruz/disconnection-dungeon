using Graphene.UiGenerics;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.Presentation.Pages
{
    public class PauseScreen : CanvasGroupView
    {
        private DDManager _manager;

        void Setup()
        {
            Hide();
        }

        void Start()
        {
            _manager = DDManager.Instance;

            _manager.GameOver += Hide;
            _manager.LevelStart += Hide;
            _manager.PlayerDie += Hide;
            _manager.Pause += Show;
            _manager.UnPause += Hide;
        }
    }
}