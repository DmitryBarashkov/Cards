using Cards.Gameplay;
using UnityEngine;

namespace Cards.Databases
{
    [CreateAssetMenu(fileName = "NewLevelConfig", menuName = "Configs/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Общие настройки")]
        public Difficulty LevelDifficulty;
        public int LevelNumber;

        [Header("Параметры баланса карт")]
        public int TotalTriplets = 10;
        public int UniqueTypesCount = 4;
        public int BankSize = 7;

        [Header("Размеры сетки и форма")]
        public int GridWidth = 12;
        public int GridHeight = 12;
        public int MaxLayers = 4;
        public LevelShape Shape = LevelShape.Rectangle;
    }
}