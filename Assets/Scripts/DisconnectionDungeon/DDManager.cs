using System.Collections.Generic;
using GameManagement;
using Graphene.Grid;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Graphene.DisconnectionDungeon
{
    internal class LevelProgress
    {
        public int CurrentLevel;

        public List<int> LevelScores;
    }

    public class DDManager : GameManager
    {
        private NetworkManagerWrapper _networkManager;
        
        private int _selectedLevel = 0;
        private LevelProgress _progress;

        private bool[] _collected = new bool[3];


        private static DDManager _instance;

        public static DDManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("DDManager", new System.Type[]
                        {
                            typeof(DDManager),
                            typeof(GridManager)
                        }
                    ).GetComponent<DDManager>();
                    
                    var tmp = Instantiate(Resources.Load<GameObject>("EssentialCanvas"));
                    var tmpB = Instantiate(Resources.Load<GameObject>("EventSystem"));

                    DontDestroyOnLoad(_instance.gameObject);
                    DontDestroyOnLoad(tmp);
                    DontDestroyOnLoad(tmpB);
                }

                return _instance;
            }
        }

        private void Awake()
        {
            Load();
        }

        private void Start()
        {
            _networkManager = FindObjectOfType<NetworkManagerWrapper>();

            _networkManager.OnClientStarted += LoadOnlineLimbo;
        }

        private void LoadOnlineLimbo()
        {
            SceneManager.LoadScene("Limbo");
        }

        private void Load()
        {
            _progress = SaveData.Load<LevelProgress>("progressdata");

            if (_progress == null)
            {
                _progress = new LevelProgress();

                _progress.CurrentLevel = 0;
                _progress.LevelScores = new List<int>();
                Save();
            }
        }

        private void Save()
        {
            SaveData.Save(_progress, "progressdata");
        }

        private void UpdateProgress()
        {
            var count = 0;

            for (var i = 0; i < _collected.Length; i++)
            {
                if (_collected[i])
                    count++;
            }

            if (_progress.LevelScores.Count <= _selectedLevel)
            {
                for (var i = 0; i <= _selectedLevel - _progress.LevelScores.Count; i++)
                {
                    _progress.LevelScores.Add(0);
                }
            }

            if (count > _progress.LevelScores[_selectedLevel])
                _progress.LevelScores[_selectedLevel] = count;

            if (_selectedLevel < _progress.CurrentLevel) return;

            _progress.CurrentLevel = _selectedLevel + 1;
            Save();
        }

        public int GetCurrentLevel()
        {
            return _progress.CurrentLevel;
        }

        public void StartGame()
        {
            ResetCollectables();

            OnLevelStart();

            SceneManager.LoadScene("Level_" + _selectedLevel);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(0);
        }

        private void ResetCollectables()
        {
            for (int i = 0; i < _collected.Length; i++)
            {
                _collected[i] = false;
            }
        }

        public void SelectLevel(int id)
        {
            _selectedLevel = id;

            StartGame();
        }

        public int LevelRate()
        {
            return _progress.LevelScores[_selectedLevel];
        }

        public bool IsCollected(int index)
        {
            if (index >= _collected.Length)
                return false;

            return _collected[index];
        }

        public void Collect(int index)
        {
            if (index >= _collected.Length)
                return;

            _collected[index] = true;
        }

        public int LevelRate(int index)
        {
            return _progress.LevelScores[index];
        }

        public void EndLevel()
        {
            UpdateProgress();

            _selectedLevel++;

            OnGameOver();
        }
    }
}