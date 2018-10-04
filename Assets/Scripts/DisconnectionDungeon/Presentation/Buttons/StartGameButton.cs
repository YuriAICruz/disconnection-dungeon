using Graphene.DisconnectionDungeon.Presentation.Pages;
using Graphene.UiGenerics;

namespace Graphene.DisconnectionDungeon.Presentation.Buttons
{
    public class StartGameButton : ButtonView
    {
        private DDManager _manager;
        private void Setup()
        {
            
        }

        private void Start()
        {
            _manager = DDManager.Instance;
        }

        protected override void OnClick()
        {
            _manager.StartGame();
        }
    }
}