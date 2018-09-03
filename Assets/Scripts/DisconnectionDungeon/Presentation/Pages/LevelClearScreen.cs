using UiGenerics;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.Presentation.Pages
{
    public class LevelClearScreen : CanvasGroupView
    {
        private DDManager _manager;

        public GameObject[] Stars;

        void Setup()
        {
            Hide();
        }

        void Start()
        {
            _manager = DDManager.Instance;

            _manager.GameOver += OnShow;
            _manager.LevelStart += Hide;
        }

        private void OnShow()
        {
            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].SetActive(_manager.IsCollected(i));
            }

            Show();
        }
    }
}