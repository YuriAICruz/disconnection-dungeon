using System;
using UiGenerics;

namespace Graphene.DisconnectionDungeon.Presentation.Pages
{
    public class MainMenu : CanvasGroupView
    {
        public event Action OnMainMenu, OnOptions, OnLevelSelection, OnOnline;
        
        private void Setup()
        {
            Show();
        }

        public void OpenLevelSelection()
        {
            if (OnLevelSelection != null) OnLevelSelection();
            Hide();
        }

        public void OpenMainMenu()
        {
            if (OnMainMenu != null) OnMainMenu();
            
            Show();
        }

        public void OpenOptions()
        {
            if (OnOptions != null) OnOptions();
            Hide();
        }

        public void OpenOnline()
        {
            if (OnOnline != null) OnOnline();
            Hide();
        }
    }
}