using Graphene.UiGenerics;

namespace Graphene.DisconnectionDungeon.Presentation.Pages
{
    public class MainMenuOnline : CanvasGroupView
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
            _mainMenu.OnOptions += Hide;
            _mainMenu.OnOnline += ShowMe;
        }

        private void ShowMe()
        {
            var grp = transform.GetComponentsInChildren<CanvasGroupView>();
            foreach (var groupView in grp)
            {
                groupView.Hide();
            }
            Show();
        }
    }
}