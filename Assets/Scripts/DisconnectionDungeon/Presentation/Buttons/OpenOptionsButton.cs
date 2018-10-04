using Graphene.DisconnectionDungeon.Presentation.Pages;
using Graphene.UiGenerics;

namespace Graphene.DisconnectionDungeon.Presentation.Buttons
{
    public class OpenOptionsButton : ButtonView
    {
        private MainMenu _mainMenu;
        private void Setup()
        {
            
        }

        private void Start()
        {
            _mainMenu = FindObjectOfType<MainMenu>();
        }

        protected override void OnClick()
        {
            _mainMenu.OpenOptions();
        }
    }
}