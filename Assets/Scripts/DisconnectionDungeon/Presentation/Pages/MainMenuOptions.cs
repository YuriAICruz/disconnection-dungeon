using Graphene.UiGenerics;

namespace Graphene.DisconnectionDungeon.Presentation.Pages
{
    public class MainMenuOptions : CanvasGroupView
    {
        private MainMenu _mainMenu;
        private void Setup()
        {
            Hide();
        }

        private void Start()
        {
            _mainMenu = FindObjectOfType<MainMenu>();

            _mainMenu.OnLevelSelection += Hide;
            _mainMenu.OnMainMenu += Hide;
            _mainMenu.OnOptions += Show;
            _mainMenu.OnOnline += Hide;
        }
    }
}