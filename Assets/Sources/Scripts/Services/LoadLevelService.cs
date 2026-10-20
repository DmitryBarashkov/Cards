using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Databases;
using UnityEngine;
using YG;
using Zenject;

namespace Cards.Services
{
    public class LoadLevelService
    {
        private const int LastEasyLevelNumber = 5;
        private const int LastMediumLevelNumber = 10;
        private const int LastHardLevelNumber = 16;

        private List<LevelConfig> _configs;

        private LevelConfig _currentConfig;

        [Inject]
        public void Construct(List<LevelConfig> configs)
        {
            _configs = configs;
        }

        public LevelConfig GetLevelConfig()
        {
            LevelConfig selectedConfig = null;
            int levelNumber = YG2.saves.Level;

            switch (levelNumber)
            {
                case <= LastEasyLevelNumber:
                    selectedConfig = GetConfig(Difficulty.Easy, levelNumber);
                    break;
                case <= LastMediumLevelNumber:
                    selectedConfig = GetConfig(Difficulty.Medium, levelNumber);
                    break;
                case <= LastHardLevelNumber:
                    selectedConfig = GetConfig(Difficulty.Hard, levelNumber);
                    break;
                default:
                    selectedConfig = GetRandomConfig();
                    break;
            }

            if (selectedConfig != null)
            {
                _currentConfig = selectedConfig;
                Debug.Log($"[LevelProvider] Загружен уровень игры {levelNumber}.");
            }
            else
            {
                _currentConfig = _configs.FirstOrDefault();
                Debug.LogError($"[LevelProvider] Критическая ошибка: Не найден конфиг для уровня {levelNumber}! Взят дефолтный.");
            }

            return _currentConfig;
        }

        private LevelConfig GetRandomConfig()
        {
            List<LevelConfig> newLevelConfig;

            if (_currentConfig.LevelDifficulty == Difficulty.Medium)
                newLevelConfig = _configs.Where(c => c.LevelDifficulty == Difficulty.Hard).ToList();
            else
                newLevelConfig = _configs.Where(c => c.LevelDifficulty == Difficulty.Medium).ToList();

            if (newLevelConfig.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, newLevelConfig.Count);

            return newLevelConfig[randomIndex];
        }

        private LevelConfig GetConfig(Difficulty difficulty, int internalNumber)
        {
            LevelConfig config = _configs[internalNumber - 1];

            if (difficulty == config.LevelDifficulty)
                return config;

            throw new ArgumentException("Неправильный конфиг уровня или он не найден");
        }
    }
}