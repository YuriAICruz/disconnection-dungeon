using Boo.Lang.Environments;
using Graphene.Acting;
using Graphene.Acting.Collectables;
using UnityEngine;

namespace Graphene.DisconnectionDungeon.Collectable
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Door : MonoBehaviour
    {
        private BoxCollider2D _collider;
        
        [SerializeField]
        private GameObject _closed, _opened;

        public Vector3Int TransportPosition;

        private bool _isOpen;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _collider.isTrigger = false;
        }

        public void Open()
        {
            _collider.enabled = false;
            _isOpen = true;
            _closed.SetActive(false);            
            _opened.SetActive(true);            
        }
        
        public void Close()
        {
            _collider.enabled = true;
            _isOpen = false;
            _closed.SetActive(true);            
            _opened.SetActive(false);
        }
    }
}