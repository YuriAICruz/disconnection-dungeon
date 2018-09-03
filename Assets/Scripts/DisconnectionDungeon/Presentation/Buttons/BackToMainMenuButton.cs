using Graphene.DisconnectionDungeon.Presentation.Pages;
using UiGenerics;

namespace Graphene.DisconnectionDungeon.Presentation.Buttons
{
    public class BackToMainMenuButton : ButtonView
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
            _manager.BackToMenu();
        }
    }
}