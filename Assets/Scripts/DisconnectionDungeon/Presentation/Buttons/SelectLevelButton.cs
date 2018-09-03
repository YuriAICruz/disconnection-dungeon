using GameManagement;
using UiGenerics;
using UnityEngine.UI;

namespace Graphene.DisconnectionDungeon.Presentation.Buttons
{
    public class SelectLevelButton : ButtonView
    {
        private DDManager _manager;
        private Text _text;
        
        private void Setup()
        {
        }

        private void Start()
        {
            _manager = DDManager.Instance;

            _text = transform.GetChild(0).GetComponent<Text>();

            _text.text = transform.GetSiblingIndex().ToString("00");

            if (transform.GetSiblingIndex() > _manager.GetCurrentLevel())
                Button.interactable = false;
        }

        protected override void OnClick()
        {
            _manager.SelectLevel(transform.GetSiblingIndex());
        }
    }
}