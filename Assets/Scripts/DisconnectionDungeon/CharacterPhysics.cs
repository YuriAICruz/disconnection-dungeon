using System;
using Graphene.Physics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DisconnectionDungeon
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
            
            var hit = Physics2D.Raycast(position, dir, 1);
            
            return hit.collider != null && !hit.collider.isTrigger;
        }

        private void CheckTrigger(Vector3 position, Vector2Int dir)
        {
            var hit = Physics2D.Raycast(position, dir, 1);

            if (hit.collider == null || !hit.collider.isTrigger) return;

            if (OnTriggerEnter != null) OnTriggerEnter(hit);
        }
    }
}