using Graphene.UiGenerics;
using UnityEngine;
using UnityEngine.UI;

namespace Graphene.WebSocketsNetworking.Presentation
{
    public class SendInputField : InputFieldView
    {
        private NetworkManager _networkManager;

        public Button Send;
        
        void Setup()
        {
            
        }

        private void Start()
        {
            _networkManager = FindObjectOfType<NetworkManager>();
            
            Send.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _networkManager.Send(InputField.text);
        }
    }
}