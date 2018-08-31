using Graphene.DisconnectionDungeon.Presentation.Pages;
using UiGenerics;

namespace Graphene.DisconnectionDungeon.Presentation.Buttons
{
    public class LevelSelectionButton : ButtonView
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
            _mainMenu.OpenLevelSelection();
        }
    }
}