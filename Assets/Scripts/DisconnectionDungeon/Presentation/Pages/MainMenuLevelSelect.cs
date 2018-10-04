using Graphene.UiGenerics;

namespace Graphene.DisconnectionDungeon.Presentation.Pages
{
    public class MainMenuLevelSelect : CanvasGroupView
    {
        private MainMenu _mainMenu;
        private void Setup()
        {
            Hide();
        }

        private void Start()
        {
            _mainMenu = FindObjectOfType<MainMenu>();

            _mainMenu.OnLevelSelection += Show;
            _mainMenu.OnMainMenu += Hide;
            _mainMenu.OnOptions += Hide;
            _mainMenu.OnOnline += Hide;
        }
    }
}