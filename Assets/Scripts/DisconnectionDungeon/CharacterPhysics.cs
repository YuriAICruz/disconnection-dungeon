using System;
using Graphene.Physics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Graphene.DisconnectionDungeon
{
    [Serializable]
    public class CharacterPhysics : PhysycsBase
    {
        public Tilemap Level;

        private bool[,] _collisionMask;

        public void CalculateCollisionMask()
        {
            _collisionMask = new bool[Level.size.x + 2, Level.size.y + 2];

            for (int x = 0; x <= Level.size.x + 1; x++)
            {
                for (int y = 0; y <= Level.size.y + 1; y++)
                {
                    _collisionMask[x, y] = Level.GetTile(new Vector3Int(x + Level.cellBounds.x - 1, y + Level.cellBounds.y - 1, 0)) != null;
                }
            }
        }

        public bool CheckCollision(Vector3 position, Vector2Int dir)
        {
            if (_collisionMask == null)
                CalculateCollisionMask();

            CheckTrigger(position, dir);

            var pos = new Vector2Int((int) (position.x + dir.x) - Level.cellBounds.x, (int) (position.y + dir.y) - Level.cellBounds.y);

            if (!_collisionMask[pos.x, pos.y])
                return true;

            _collider.enabled = false;
            var hit = Physics2D.Raycast(position, dir, 1, _layerMask);
            _collider.enabled = true;
            
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                if (OnCollisionEnter != null) OnCollisionEnter(hit);
                return true;
            }
            
            return false;
        }

        private void CheckTrigger(Vector3 position, Vector2Int dir)
        {
            var mask = _layerMask & ~(1 << LayerMask.NameToLayer("Boxes"));
            var hit = Physics2D.Raycast(position, dir, 1, mask);
            
            Debug.Log(hit.collider);

            if (hit.collider == null || !hit.collider.isTrigger) return;

            if (OnTriggerEnter != null) OnTriggerEnter(hit);
        }
    }
}