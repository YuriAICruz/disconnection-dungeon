using System;
using UnityEngine;

namespace GameManagement
{
    public abstract class GameManager : MonoBehaviour
    {
        public event Action LevelStart, PlayerDie, GameOver;

        public virtual void OnGameOver()
        {
            if (GameOver != null) GameOver();
        }

        public virtual void OnPlayerDie()
        {
            if (PlayerDie != null) PlayerDie();
        }

        public virtual void OnLevelStart()
        {
            if (LevelStart != null) LevelStart();
        }
    }
}